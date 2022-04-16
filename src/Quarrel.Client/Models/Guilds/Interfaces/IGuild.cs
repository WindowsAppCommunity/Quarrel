// Quarrel © 2022

using Discord.API.Models.Enums.Guilds;
using Quarrel.Client.Models.Base.Interfaces;

namespace Quarrel.Client.Models.Guilds.Interfaces
{
    internal interface IGuild : ISnowflakeItem
    {
        /// <summary>
        /// Gets the name of the guild.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the AFK timeout duration of the guild.
        /// </summary>
        AFKTimeout? AFKTimeout { get; }

        /// <summary>
        /// Gets if the guild's widget is enabled.
        /// </summary>
        bool IsWidgetEnabled { get; }

        /// <summary>
        /// Gets the default message notification settings for the guild.
        /// </summary>
        DefaultMessageNotifications DefaultMessageNotifications { get; }

        /// <summary>
        /// Gets the multi-factor authentication level required for the guild.
        /// </summary>
        MfaLevel MfaLevel { get; }

        /// <summary>
        /// Gets the required member verification level for guild participation.
        /// </summary>
        VerificationLevel VerificationLevel { get; }

        /// <summary>
        /// Gets the guild content filter level.
        /// </summary>
        ExplicitContentFilterLevel ExplicitContentFilter { get; }

        /// <summary>
        /// Gets the number premium subscription users for the guild.
        /// </summary>
        int PremiumSubscriptionCount { get; }

        /// <summary>
        /// Gets the icon id for the guild.
        /// </summary>
        /// <remarks>
        /// Used with icon url template.
        /// </remarks>
        string IconId { get; }

        /// <summary>
        /// Gets the id of the splash image for the guild.
        /// </summary>
        /// <remarks>
        /// Used with splash url template.
        /// </remarks>
        string SplashId { get; }

        /// <summary>
        /// Gets the id of the discovery splash image for the guild.
        /// </summary>
        /// <remarks>
        /// Used with splash url template.
        /// </remarks>
        string DiscoverySplashId { get; }

        /// <summary>
        /// Gets if the guild is available.
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// Gets the id of the afk channel.
        /// </summary>
        ulong? AFKChannelId { get; }

        /// <summary>
        /// Gets the id of the widget channel.
        /// </summary>
        ulong? WidgetChannelId { get; }

        /// <summary>
        /// Gets the id of the channel for system messages.
        /// </summary>
        ulong? SystemChannelId { get; }

        /// <summary>
        /// Gets the id of the rules channel in the guild.
        /// </summary>
        ulong? RulesChannelId { get; }

        /// <summary>
        /// Gets the id of the public updates channel in the guild.
        /// </summary>
        ulong? PublicUpdatesChannelId { get; }

        /// <summary>
        /// Gets the id of the owning user.
        /// </summary>
        ulong OwnerId { get; }

        /// <summary>
        /// Gets the id of the voice region.
        /// </summary>
        string VoiceRegionId { get; }

        /// <summary>
        /// Gets the nsfw level of the guild.
        /// </summary>
        NsfwLevel NsfwLevel { get; }
    }
}
