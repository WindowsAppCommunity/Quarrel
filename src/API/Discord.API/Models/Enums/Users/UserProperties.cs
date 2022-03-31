// Adam Dernis © 2022

using System;

namespace Discord.API.Models.Enums.Users
{
    [Flags]
    public enum UserProperties
    {
        None = 0,
        Staff = 1 << 0,
        Partner = 1 << 1,
        HypeSquadEvents = 1 << 2,
        BugHunterLevel1 = 1 << 3,
        HypeSquadBravery = 1 << 6,
        HypeSquadBrilliance = 1 << 7,
        HypeSquadBalance = 1 << 8,
        EarlySupporter = 1 << 9,
        TeamUser = 1 << 10,
        System = 1 << 12,
        BugHunterLevel2 = 1 << 14,
        VerifiedBot = 1 << 16,
        EarlyVerifiedBotDeveloper = 1 << 17,
        DiscordCertifiedModerator = 1 << 18,
        BotHTTPInteractions = 1 << 19,
    }
}
