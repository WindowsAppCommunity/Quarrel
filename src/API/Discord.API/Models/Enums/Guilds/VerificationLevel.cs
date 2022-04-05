// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum representing the member verification level for a guild before sending messages.
    /// </summary>
    public enum VerificationLevel
    {
        /// <summary>
        /// Members require no verification.
        /// </summary>
        None = 0,

        /// <summary>
        /// Members must have a verified email account.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Members must also have been on discord for more than 5 minutes.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Members must also be a member of the guild for more than 10 munutes.
        /// </summary>
        High = 3,

        /// <summary>
        /// Members must have a verified phone number for their Discord account.
        /// </summary>
        Extreme = 4
    }
}
