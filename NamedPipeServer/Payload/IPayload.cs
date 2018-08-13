using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordPipeImpersonator.Payload
{
    public class EnumValueAttribute : Attribute
    {
        public string Value { get; set; }
        public EnumValueAttribute(string value)
        {
            this.Value = value;
        }
    }
    public class EnumSnakeCaseConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            object val = null;
            if (TryParseEnum(objectType, (string)reader.Value, out val))
                return val;

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumtype = value.GetType();
            var name = Enum.GetName(enumtype, value);

            //Get each member and look for hte correct one
            var members = enumtype.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                if (m.Name.Equals(name))
                {
                    var attributes = m.GetCustomAttributes(typeof(EnumValueAttribute), true);
                    if (attributes.Length > 0)
                    {
                        name = ((EnumValueAttribute)attributes[0]).Value;
                    }
                }
            }

            writer.WriteValue(name);
        }


        public bool TryParseEnum(Type enumType, string str, out object obj)
        {
            //Make sure the string isn;t null
            if (str == null)
            {
                obj = null;
                return false;
            }

            //Get the real type
            Type type = enumType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments().First();

            //Make sure its actually a enum
            if (!type.IsEnum)
            {
                obj = null;
                return false;
            }


            //Get each member and look for hte correct one
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                var attributes = m.GetCustomAttributes(typeof(EnumValueAttribute), true);
                foreach (var a in attributes)
                {
                    var enumval = (EnumValueAttribute)a;
                    if (str.Equals(enumval.Value))
                    {
                        obj = Enum.Parse(type, m.Name, ignoreCase: true);

                        return true;
                    }
                }
            }

            //We failed
            obj = null;
            return false;
        }

    }
    internal class EventPayload : IPayload
    {
        /// <summary>
        /// The data the server sent too us
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Data { get; set; }

        /// <summary>
        /// The type of event the server sent
        /// </summary>
        [JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseConverter))]
        public ServerEvent? Event { get; set; }

        public EventPayload() : base() { Data = null; }
        public EventPayload(long nonce) : base(nonce) { Data = null; }

        /// <summary>
        /// Sets the obejct stored within the data.
        /// </summary>
        /// <param name="obj"></param>
        public void SetObject(object obj)
        {
            Data = JObject.FromObject(obj);
        }

        /// <summary>
        /// Gets the object stored within the Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObject<T>()
        {
            if (Data == null) return default(T);
            return Data.ToObject<T>();
        }

        public override string ToString()
        {
            return "Event " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
        }
    }
    /// <summary>
    /// The possible commands that can be sent and received by the server.
    /// </summary>
    enum Command
    {
        /// <summary>
        /// event dispatch
        /// </summary>
        [EnumValue("DISPATCH")]
        Dispatch,

        /// <summary>
        /// Called to set the activity
        /// </summary>
        [EnumValue("SET_ACTIVITY")]
        SetActivity,

        /// <summary>
        /// used to subscribe to an RPC event
        /// </summary>
        [EnumValue("SUBSCRIBE")]
        Subscribe,

        /// <summary>
        /// used to unsubscribe from an RPC event
        /// </summary>
        [EnumValue("UNSUBSCRIBE")]
        Unsubscribe,

        /// <summary>
        /// Used to accept join requests.
        /// </summary>
        [EnumValue("SEND_ACTIVITY_JOIN_INVITE")]
        SendActivityJoinInvite,

        /// <summary>
        /// Used to reject join requests.
        /// </summary>
        [EnumValue("CLOSE_ACTIVITY_JOIN_REQUEST")]
        CloseActivityJoinRequest,

        /// <summary>
        /// used to authorize a new client with your app
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        Authorize,

        /// <summary>
        /// used to authenticate an existing client with your app
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        Authenticate,

        /// <summary>
        /// used to retrieve guild information from the client
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetGuild,

        /// <summary>
        /// used to retrieve a list of guilds from the client
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetGuilds,

        /// <summary>
        /// used to retrieve channel information from the client
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetChannel,

        /// <summary>
        /// used to retrieve a list of channels for a guild from the client
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetChannels,


        /// <summary>
        /// used to change voice settings of users in voice channels
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SetUserVoiceSettings,

        /// <summary>
        /// used to join or leave a voice channel, group dm, or dm
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SelectVoiceChannel,

        /// <summary>
        /// used to get the current voice channel the client is in
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetSelectedVoiceChannel,

        /// <summary>
        /// used to join or leave a text channel, group dm, or dm
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SelectTextChannel,

        /// <summary>
        /// used to retrieve the client's voice settings
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GetVoiceSettings,

        /// <summary>
        /// used to set the client's voice settings
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SetVoiceSettings,

        /// <summary>
        /// used to capture a keyboard shortcut entered by the user RPC Events
        /// </summary>
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        CaptureShortcut
    }
    enum ServerEvent
    {
        /// <summary>
        /// Sent when the server is ready to accept messages
        /// </summary>
        [EnumValue("READY")]
        Ready,

        /// <summary>
        /// Sent when something bad has happened
        /// </summary>
        [EnumValue("ERROR")]
        Error,

        /// <summary>
        /// Join Event 
        /// </summary>
        [EnumValue("ACTIVITY_JOIN")]
        ActivityJoin,

        /// <summary>
        /// Spectate Event
        /// </summary>
        [EnumValue("ACTIVITY_SPECTATE")]
        ActivitySpectate,

        /// <summary>
        /// Request Event
        /// </summary>
        [EnumValue("ACTIVITY_JOIN_REQUEST")]
        ActivityJoinRequest,

        #region RPC Protocols
        //Old things that are obsolete
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GuildStatus,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        GuildCreate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        ChannelCreate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceChannelSelect,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceStateCreated,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceStateUpdated,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceStateDelete,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceSettingsUpdate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        VoiceConnectionStatus,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SpeakingStart,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        SpeakingStop,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        MessageCreate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        MessageUpdate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        MessageDelete,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        NotificationCreate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        CaptureShortcutChange
        #endregion
    }
    internal abstract class IPayload
    {
        /// <summary>
        /// The type of payload
        /// </summary>
        [JsonProperty("cmd"), JsonConverter(typeof(EnumSnakeCaseConverter))]
        public Command Command { get; set; }

        /// <summary>
        /// A incremental value to help identify payloads
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        public IPayload() { }
        public IPayload(long nonce)
        {
            Nonce = nonce.ToString();
        }

        public override string ToString()
        {
            return "Payload || Command: " + Command.ToString() + ", Nonce: " + (Nonce != null ? Nonce.ToString() : "NULL");
        }
    }
}
