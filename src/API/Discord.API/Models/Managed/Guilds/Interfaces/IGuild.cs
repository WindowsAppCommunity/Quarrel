// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Guilds;

namespace Discord.API.Models.Guilds.Interfaces
{
    public interface IGuild : ISnowflakeItem
    {
        string Name { get; }

        int AFKTimeout { get; }

        bool IsWidgetEnabled { get; }

        DefaultMessageNotifications DefaultMessageNotifications { get; }

        MfaLevel MfaLevel { get; }

        VerificationLevel VerificationLevel { get; }

        ExplicitContentFilterLevel ExplicitContentFilter { get; }

        int PremiumSubscriptionCount { get; }

        string IconId { get; }

        string SplashId { get; }

        string DiscoverySplashId { get; }

        bool Available { get; }

        ulong? AFKChannelId { get; }

        ulong? WidgetChannelId { get; }

        ulong? SystemChannelId { get; }

        ulong? RulesChannelId { get; }

        ulong? PublicUpdatesChannelId { get; }

        ulong OwnerId { get; }

        string VoiceRegionId { get; }

        int MaxBitrate { get; }

        string PreferredLocale { get; }

        NsfwLevel NsfwLevel { get; }

        ulong MaxUploadLimit { get; }
    }
}
