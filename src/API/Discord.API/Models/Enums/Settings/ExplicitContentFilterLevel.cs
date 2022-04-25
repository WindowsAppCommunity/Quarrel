// Quarrel © 2022

namespace Discord.API.Models.Enums.Settings
{
    /// <summary>
    /// The content filter level.
    /// </summary>
    public enum ExplicitContentFilterLevel : int
    {
        /// <summary>
        /// Filter content from nobody.
        /// </summary>
        None,

        /// <summary>
        /// Filter content not from my friends
        /// </summary>
        Public,

        /// <summary>
        /// Filter content from everyone.
        /// </summary>
        All,
    }
}
