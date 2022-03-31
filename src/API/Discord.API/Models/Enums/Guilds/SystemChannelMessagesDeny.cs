// Adam Dernis © 2022

using System;

namespace Discord.API.Models.Enums.Guilds
{
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
