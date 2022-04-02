// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Base;
using Discord.API.Models.Enums.Guilds;
using Discord.API.Models.Guilds.Interfaces;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Managed.Roles;
using System.Collections.Generic;

namespace Discord.API.Models.Managed.Guilds
{
    public class Guild : SnowflakeItem, IGuild
    {
        private HashSet<ulong> _channelIds;
        private Dictionary<ulong, Role> _roles;

        internal Guild(JsonGuild restGuild, DiscordClient context)
            : base(context)
        {
            Id = restGuild.Id;
            Name = restGuild.Name;

            AFKChannelId = restGuild.AFKChannelId;
            AFKTimeout = restGuild.AFKTimeout;

            DefaultMessageNotifications = restGuild.DefaultMessageNotifications;
            MfaLevel = restGuild.MfaLevel;
            VerificationLevel = restGuild.VerificationLevel;
            ExplicitContentFilter = restGuild.ExplicitContentFilter;
            NsfwLevel = restGuild.NsfwLevel;

            OwnerId = restGuild.OwnerId;

            PremiumSubscriptionCount = restGuild.PremiumSubscriptionCount ?? 0;
            IconId = restGuild.Icon;
            SplashId = restGuild.Splash;
            DiscoverySplashId = restGuild.DiscoverySplash;

            WidgetChannelId = restGuild.WidgetChannelId;
            SystemChannelId = restGuild.SystemChannelId;
            RulesChannelId = restGuild.RulesChannelId;
            PublicUpdatesChannelId = restGuild.PublicUpdatesChannelId;

            IsWidgetEnabled = restGuild.IsWidgetEnabled ?? false;
            Available = true;

            _channelIds = new HashSet<ulong>();
            _roles = new Dictionary<ulong, Role>();

            foreach (var role in restGuild.Roles)
            {
                _roles.Add(role.Id, new Role(role, context));
            }
        }

        public string Name { get; private set; }

        public ulong? AFKChannelId { get; private set; }

        public int? AFKTimeout { get; private set; }

        public DefaultMessageNotifications DefaultMessageNotifications { get; private set; }

        public MfaLevel MfaLevel { get; private set; }

        public VerificationLevel VerificationLevel { get; private set; }

        public ExplicitContentFilterLevel ExplicitContentFilter { get; private set; }

        public NsfwLevel NsfwLevel { get; private set; }

        public int PremiumSubscriptionCount { get; private set; }

        public string IconId { get; private set; }

        public string SplashId { get; private set; }

        public string DiscoverySplashId { get; private set; }

        public bool Available { get; private set; }

        public ulong? WidgetChannelId { get; private set; }

        public ulong? SystemChannelId { get; private set; }

        public ulong? RulesChannelId { get; private set; }

        public ulong? PublicUpdatesChannelId { get; private set; }

        public bool IsWidgetEnabled { get; private set; }

        public ulong OwnerId { get; private set; }

        public string VoiceRegionId { get; private set; }

        internal void UpdateFromRestGuild(JsonGuild jsonGuild)
        {
            Guard.IsEqualTo(Id, jsonGuild.Id, nameof(Id));

            Name = jsonGuild.Name;
            AFKChannelId = jsonGuild.AFKChannelId ?? AFKChannelId;
            AFKTimeout = jsonGuild.AFKTimeout ?? AFKTimeout;
            DefaultMessageNotifications = jsonGuild.DefaultMessageNotifications;
            MfaLevel = jsonGuild.MfaLevel;
            VerificationLevel = jsonGuild.VerificationLevel;
            ExplicitContentFilter = jsonGuild.ExplicitContentFilter;
            NsfwLevel = jsonGuild.NsfwLevel;
            PremiumSubscriptionCount = jsonGuild.PremiumSubscriptionCount ?? 0;
            IconId = jsonGuild.Icon;
            SplashId = jsonGuild.Splash;
            DiscoverySplashId = jsonGuild.Splash;
            WidgetChannelId = jsonGuild.WidgetChannelId;
            SystemChannelId = jsonGuild.SystemChannelId;
            RulesChannelId = jsonGuild.RulesChannelId;
            PublicUpdatesChannelId = jsonGuild.PublicUpdatesChannelId;
            IsWidgetEnabled = jsonGuild.IsWidgetEnabled ?? false;
            OwnerId = jsonGuild.OwnerId;
            VoiceRegionId = jsonGuild.Region;

            Available = true;
        }

        internal bool AddChannel(ulong channelId)
        {
            return _channelIds.Add(channelId);
        }

        internal bool ContainsChannel(ulong channelId)
        {
            return _channelIds.Contains(channelId);
        }

        internal bool RemoveChannel(ulong channelId)
        {
            return _channelIds.Remove(channelId);
        }

        internal IEnumerable<ulong> ChannelIds => _channelIds;
    }
}
