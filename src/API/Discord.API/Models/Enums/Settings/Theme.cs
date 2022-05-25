// Quarrel © 2022

using Quarrel.Attributes;

namespace Discord.API.Models.Enums.Settings
{
    /// <summary>
    /// The theme of the UI according to Discord's preferences.
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// Dark theme.
        /// </summary>
        [StringValue("dark")]
        Dark,

        /// <summary>
        /// Light theme.
        /// </summary>
        [StringValue("light")]
        Light,
    }
}
