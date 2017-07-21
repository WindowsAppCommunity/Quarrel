using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Globalization;

namespace Discord_UWP
{
    internal class DiscordContractResolver : DefaultContractResolver
    {
        private static readonly TypeInfo _ienumerable = typeof(IEnumerable<ulong[]>).GetTypeInfo();
        private static readonly MethodInfo _shouldSerialize = typeof(DiscordContractResolver).GetTypeInfo().GetDeclaredMethod("ShouldSerialize");

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.Ignored)
                return property;

            if (member is PropertyInfo propInfo)
            {
                var converter = GetConverter(property, propInfo, propInfo.PropertyType, 0);
                if (converter != null)
                {
                    property.Converter = converter;
                    property.MemberConverter = converter;
                }
            }
            else
                throw new InvalidOperationException($"{member.DeclaringType.FullName}.{member.Name} is not a property.");
            return property;
        }

        private static JsonConverter GetConverter(JsonProperty property, PropertyInfo propInfo, Type type, int depth)
        {
            if (type.IsArray)
                return MakeGenericConverter(property, propInfo, typeof(ArrayConverter<>), type.GetElementType(), depth);
            if (type.IsConstructedGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (depth == 0 && genericType == typeof(Optional<>))
                {
                    var typeInput = propInfo.DeclaringType;
                    var innerTypeOutput = type.GenericTypeArguments[0];

                    var getter = typeof(Func<,>).MakeGenericType(typeInput, type);
                    var getterDelegate = propInfo.GetMethod.CreateDelegate(getter);
                    var shouldSerialize = _shouldSerialize.MakeGenericMethod(typeInput, innerTypeOutput);
                    var shouldSerializeDelegate = (Func<object, Delegate, bool>)shouldSerialize.CreateDelegate(typeof(Func<object, Delegate, bool>));
                    property.ShouldSerialize = x => shouldSerializeDelegate(x, getterDelegate);

                    return MakeGenericConverter(property, propInfo, typeof(OptionalConverter<>), innerTypeOutput, depth);
                }
                else if (genericType == typeof(Nullable<>))
                    return MakeGenericConverter(property, propInfo, typeof(NullableConverter<>), type.GenericTypeArguments[0], depth);
                else if (genericType == typeof(EntityOrId<>))
                    return MakeGenericConverter(property, propInfo, typeof(UInt64EntityOrIdConverter<>), type.GenericTypeArguments[0], depth);
            }

            //Primitives
            bool hasInt53 = propInfo.GetCustomAttribute<Int53Attribute>() != null;
            if (!hasInt53)
            {
                if (type == typeof(ulong))
                    return UInt64Converter.Instance;
            }

            //Enums
            if (type == typeof(PermissionTarget))
                return PermissionTargetConverter.Instance;
            if (type == typeof(UserStatus))
                return UserStatusConverter.Instance;

            //Special
            if (type == typeof(Image))
                return ImageConverter.Instance;

            //Entities
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<ulong>)))
                return UInt64EntityConverter.Instance;
            if (typeInfo.ImplementedInterfaces.Any(x => x == typeof(IEntity<string>)))
                return StringEntityConverter.Instance;

            return null;
        }

        private static bool ShouldSerialize<TOwner, TValue>(object owner, Delegate getter)
        {
            return (getter as Func<TOwner, Optional<TValue>>)((TOwner)owner).IsSpecified;
        }

        private static JsonConverter MakeGenericConverter(JsonProperty property, PropertyInfo propInfo, Type converterType, Type innerType, int depth)
        {
            var genericType = converterType.MakeGenericType(innerType).GetTypeInfo();
            var innerConverter = GetConverter(property, propInfo, innerType, depth + 1);
            return genericType.DeclaredConstructors.First().Invoke(new object[] { innerConverter }) as JsonConverter;
        }
    }

    internal class StringEntityConverter : JsonConverter
    {
        public static readonly StringEntityConverter Instance = new StringEntityConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue((value as IEntity<string>).Id);
            else
                writer.WriteNull();
        }
    }

    internal class UInt64EntityOrIdConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public UInt64EntityOrIdConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Integer:
                    return new EntityOrId<T>(ulong.Parse(reader.ReadAsString()));
                default:
                    T obj;
                    if (_innerConverter != null)
                        obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                    else
                        obj = serializer.Deserialize<T>(reader);
                    return new EntityOrId<T>(obj);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }
    }

    internal struct EntityOrId<T>
    {
        public ulong Id { get; }
        public T Object { get; }

        public EntityOrId(ulong id)
        {
            Id = id;
            Object = default(T);
        }
        public EntityOrId(T obj)
        {
            Id = 0;
            Object = obj;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    internal class Int53Attribute : Attribute { }

    internal class ArrayConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public ArrayConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new List<T>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    T obj;
                    if (_innerConverter != null)
                        obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                    else
                        obj = serializer.Deserialize<T>(reader);
                    result.Add(obj);
                    reader.Read();
                }
            }
            return result.ToArray();
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteStartArray();
                var a = (T[])value;
                for (int i = 0; i < a.Length; i++)
                {
                    if (_innerConverter != null)
                        _innerConverter.WriteJson(writer, a[i], serializer);
                    else
                        serializer.Serialize(writer, a[i], typeof(T));
                }

                writer.WriteEndArray();
            }
            else
                writer.WriteNull();
        }
    }

    public enum UserStatus
    {
        Offline,
        Online,
        Idle,
        AFK,
        DoNotDisturb,
        Invisible,
    }

    public enum PermissionTarget
    {
        Role,
        User
    }

    internal struct Image
    {
        public Stream Stream { get; }
        public string Hash { get; }

        public Image(Stream stream)
        {
            Stream = stream;
            Hash = null;
        }
        public Image(string hash)
        {
            Stream = null;
            Hash = hash;
        }
    }

    internal class ImageConverter : JsonConverter
    {
        public static readonly ImageConverter Instance = new ImageConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var image = (Image)value;

            if (image.Stream != null)
            {
                byte[] bytes = new byte[image.Stream.Length - image.Stream.Position];
                image.Stream.Read(bytes, 0, bytes.Length);

                string base64 = Convert.ToBase64String(bytes);
                writer.WriteValue($"data:image/jpeg;base64,{base64}");
            }
            else if (image.Hash != null)
                writer.WriteValue(image.Hash);
        }
    }

    internal class UInt64Converter : JsonConverter
    {
        public static readonly UInt64Converter Instance = new UInt64Converter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ulong.Parse((string)reader.Value, NumberStyles.None, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ulong)value).ToString(CultureInfo.InvariantCulture));
        }
    }

    internal class NullableConverter<T> : JsonConverter
        where T : struct
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public NullableConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object value = reader.Value;
            if (value == null)
                return null;
            else
            {
                T obj;
                if (_innerConverter != null)
                    obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
                else
                    obj = serializer.Deserialize<T>(reader);
                return obj;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                var nullable = (T?)value;
                if (_innerConverter != null)
                    _innerConverter.WriteJson(writer, nullable.Value, serializer);
                else
                    serializer.Serialize(writer, nullable.Value, typeof(T));
            }
        }
    }

    internal class UserStatusConverter : JsonConverter
    {
        public static readonly UserStatusConverter Instance = new UserStatusConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "online":
                    return UserStatus.Online;
                case "idle":
                    return UserStatus.Idle;
                case "dnd":
                    return UserStatus.DoNotDisturb;
                case "invisible":
                    return UserStatus.Invisible; //Should never happen
                case "offline":
                    return UserStatus.Offline;
                default:
                    throw new JsonSerializationException("Unknown user status");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((UserStatus)value)
            {
                case UserStatus.Online:
                    writer.WriteValue("online");
                    break;
                case UserStatus.Idle:
                case UserStatus.AFK:
                    writer.WriteValue("idle");
                    break;
                case UserStatus.DoNotDisturb:
                    writer.WriteValue("dnd");
                    break;
                case UserStatus.Invisible:
                    writer.WriteValue("invisible");
                    break;
                case UserStatus.Offline:
                    writer.WriteValue("offline");
                    break;
                default:
                    throw new JsonSerializationException("Invalid user status");
            }
        }
    }

    internal class PermissionTargetConverter : JsonConverter
    {
        public static readonly PermissionTargetConverter Instance = new PermissionTargetConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "member":
                    return PermissionTarget.User;
                case "role":
                    return PermissionTarget.Role;
                default:
                    throw new JsonSerializationException("Unknown permission target");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((PermissionTarget)value)
            {
                case PermissionTarget.User:
                    writer.WriteValue("member");
                    break;
                case PermissionTarget.Role:
                    writer.WriteValue("role");
                    break;
                default:
                    throw new JsonSerializationException("Invalid permission target");
            }
        }
    }

    internal class UInt64EntityConverter : JsonConverter
    {
        public static readonly UInt64EntityConverter Instance = new UInt64EntityConverter();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
                writer.WriteValue((value as IEntity<ulong>).Id.ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteNull();
        }
    }

    public interface IEntity<TId>
        where TId : IEquatable<TId>
    {
        ///// <summary> Gets the IDiscordClient that created this object. </summary>
        //IDiscordClient Discord { get; }

        /// <summary> Gets the unique identifier for this object. </summary>
        TId Id { get; }

    }

    internal class OptionalConverter<T> : JsonConverter
    {
        private readonly JsonConverter _innerConverter;

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public OptionalConverter(JsonConverter innerConverter)
        {
            _innerConverter = innerConverter;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            T obj;
            if (_innerConverter != null)
                obj = (T)_innerConverter.ReadJson(reader, typeof(T), null, serializer);
            else
                obj = serializer.Deserialize<T>(reader);
            return new Optional<T>(obj);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            value = ((Optional<T>)value).Value;
            if (_innerConverter != null)
                _innerConverter.WriteJson(writer, value, serializer);
            else
                serializer.Serialize(writer, value, typeof(T));
        }
    }
}
