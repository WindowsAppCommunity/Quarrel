// Quarrel © 2022

using System.Text.Json.Serialization;

namespace Discord.API
{
    /// <summary>
    /// A class containing constant values used in the Discord API.
    /// </summary>
    internal class Constants
    {
        /// <summary>
        /// A <see cref="JsonNumberHandling"/> value containing flags for both read and write as string.
        /// </summary>
        public const JsonNumberHandling ReadWriteAsString = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
    }
}
