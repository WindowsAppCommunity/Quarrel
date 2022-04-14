// Quarrel © 2022

using System;

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum containing flags for which system channel messages are dislayed.
    /// </summary>
    /// <remarks>
    /// If a value is true the message will NOT be sent.
    /// </remarks>
    [Flags]
    public enum SystemChannelMessageDeny
    {
        /// <summary>
        /// All system messages will be sent.
        /// </summary>
        None = 0,

        /// <summary>
        /// Messages won't be sent when a user joins the guild.
        /// </summary>
        WelcomeMessage = 0x1,
        
        /// <summary>
        /// Messages won't be sent when a user boosts the guild.
        /// </summary>
        GuildBoost = 0x2,
        
        /// <summary>
        /// Guild setup tip messages will not be sent.
        /// </summary>
        GuildSetupTip = 0x4,

        /// <summary>
        /// Welcome reply messages will not be sent.
        /// </summary>
        WelcomeMessageReply = 0x8
    }
}
