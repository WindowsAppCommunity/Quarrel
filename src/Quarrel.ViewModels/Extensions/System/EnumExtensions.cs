// Quarrel © 2022

using Quarrel.Attributes;
using System.Reflection;

namespace System
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute attribute = fieldInfo.GetCustomAttribute<StringValueAttribute>();
            return attribute.StringValue;
        }
    }
}
