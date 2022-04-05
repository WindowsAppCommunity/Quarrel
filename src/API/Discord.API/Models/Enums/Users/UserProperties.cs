// Adam Dernis © 2022

using System;

namespace Discord.API.Models.Enums.Users
{
    [Flags]
    public enum UserProperties
    {
        /// <summary>
        /// The user is not special.
        /// </summary>
        None = 0,

        /// <summary>
        /// The user is a Discord staff member.
        /// </summary>
        Staff = 1 << 0,

        /// <summary>
        /// The user is a Discord partner.
        /// </summary>
        Partner = 1 << 1,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        HypeSquadEvents = 1 << 2,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        BugHunterLevel1 = 1 << 3,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        HypeSquadBravery = 1 << 6,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        HypeSquadBrilliance = 1 << 7,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        HypeSquadBalance = 1 << 8,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        EarlySupporter = 1 << 9,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        TeamUser = 1 << 10,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        System = 1 << 12,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        BugHunterLevel2 = 1 << 14,

        /// <summary>
        /// The user is a verified bot.
        /// </summary>
        VerifiedBot = 1 << 16,

        /// <summary>
        /// The user is a verified bot and an early user.
        /// </summary>
        EarlyVerifiedBotDeveloper = 1 << 17,

        /// <summary>
        /// The user is a user certified moderator.
        /// </summary>
        DiscordCertifiedModerator = 1 << 18,

        /// <summary>
        /// TODO: Investigate
        /// </summary>
        BotHTTPInteractions = 1 << 19,
    }
}
