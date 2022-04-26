// Quarrel © 2022

using Quarrel.Attributes;
using System.Reflection;

namespace System
{
    /// <summary>
    /// A static class containing extensions on enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the string value of an <see cref="Enum"/> that has a <see cref="StringValueAttribute"/>.
        /// </summary>
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute attribute = fieldInfo.GetCustomAttribute<StringValueAttribute>();
            return attribute.StringValue;
        }
    }
}
