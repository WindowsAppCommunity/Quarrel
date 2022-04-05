// Adam Dernis © 2022

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
        None = 0,
        WelcomeMessage = 0x1,
        GuildBoost = 0x2,
        GuildSetupTip = 0x4,
        WelcomeMessageReply = 0x8
    }
}
