// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum representing a guild's Multi-factor authentication requirements.
    /// </summary>
    public enum MfaLevel
    {
        /// <summary>
        /// Multi-factor authentication is not required.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Multi-factor authentication is required.
        /// </summary>
        Enabled = 1
    }
}
