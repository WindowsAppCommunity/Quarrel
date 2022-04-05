// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum representing the NSFW level of a guild.
    /// </summary>
    public enum NsfwLevel
    {
        /// <summary>
        /// Unspecified.
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// NSFW content is present.
        /// </summary>
        Explicit = 1,

        /// <summary>
        /// NSFW content is not present.
        /// </summary>
        Safe = 2,

        /// <summary>
        /// The guild is age restricted.
        /// </summary>
        AgeRestricted = 3
    }
}
