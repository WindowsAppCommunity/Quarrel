// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Channels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using System;
using System.Collections.Generic;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Guilds.Guild"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuild : SelectableItem, IBindableSelectableGuildItem, IBindableGuildListItem
    {
        private IClipboardService _clipboardService;
        private ILocalizationService _localizationService;
            
        [AlsoNotifyChangeFor(nameof(IconUrl))]
        [AlsoNotifyChangeFor(nameof(IconUri))]
        [ObservableProperty]
        private Guild _guild;

        internal BindableGuild(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            IClipboardService clipboardService,
            ILocalizationService localizationService,
            Guild guild) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
            _clipboardService = clipboardService;
            _localizationService = localizationService;
            
            _guild = guild;
        }

        /// <inheritdoc/>
        public ulong? SelectedChannelId { get; set; }

        /// <summary>
        /// Gets the url of the guild's icon.
        /// </summary>
        public string IconUrl => $"https://cdn.discordapp.com/icons/{Guild.Id}/{Guild.IconId}.png?size=128";

        /// <summary>
        /// Gets the url of the guild's icon as a <see cref="Uri"/>.
        /// </summary>
        public Uri IconUri => new(IconUrl);

        /// <inheritdoc/>
        public string? Name => Guild.Name;

        /// <summary>
        /// Gets the channels in a guild.
        /// </summary>
        /// <param name="guild">The guild to get the channels for.</param>
        /// <param name="selectedChannel">The selected channel as an <see cref="IBindableSelectableChannel"/>.</param>
        /// <returns>An array of <see cref="BindableGuildChannel"/>s from the guild.</returns>
        public BindableGuildChannel?[] GetGuildChannels(BindableGuild guild, out IBindableSelectableChannel? selectedChannel)
        {
            selectedChannel = null;
            IGuildChannel[] rawChannels = guild.Guild.GetChannels();
            Array.Sort(rawChannels, Comparer<IGuildChannel>.Create((item1, item2) =>
            {
                bool is1Voice = item1.Type is ChannelType.GuildVoice or ChannelType.StageVoice;
                bool is2Voice = item2.Type is ChannelType.GuildVoice or ChannelType.StageVoice;
                if (is1Voice && !is2Voice)
                {
                    return 1;
                }
                if (is2Voice && !is1Voice)
                {
                    return -1;
                }

                return item1.Position.CompareTo(item2.Position);
            }));

            GuildMember? member = _quarrelClient.Members.GetMyGuildMember(guild.Guild.Id);
            Guard.IsNotNull(member, nameof(member));
            BindableGuildChannel?[] channels = new BindableGuildChannel[rawChannels.Length];
            var categories = new Dictionary<ulong, BindableCategoryChannel>();

            // Once for categories
            for (int i = 0; i < rawChannels.Length; i++)
            {
                var channel = rawChannels[i];
                if (channel is CategoryChannel categoryChannel)
                {
                    var bindableCategoryChannel = new BindableCategoryChannel(_messenger, _clipboardService, _discordService, _quarrelClient, _dispatcherService, categoryChannel, member);
                    categories.Add(channel.Id, bindableCategoryChannel);
                    channels[i] = bindableCategoryChannel;
                }
            }

            for (int i = 0; i < rawChannels.Length; i++)
            {
                ref BindableGuildChannel? channel = ref channels[i];
                if (channel is null && rawChannels[i] is INestedChannel nestedChannel)
                {
                    BindableCategoryChannel? category = null;
                    if (nestedChannel.CategoryId.HasValue)
                    {
                        category = categories[nestedChannel.CategoryId.Value];
                    }

                    channel = BindableGuildChannel.Create(_messenger, _clipboardService, _discordService, _quarrelClient, _localizationService, _dispatcherService, nestedChannel, member, category);

                    if (channel is not null && (channel.Channel.Id == guild.SelectedChannelId || (selectedChannel is null && channel.IsAccessible)) &&
                        channel is IBindableSelectableChannel messageChannel)
                    {
                        selectedChannel = messageChannel;
                    }
                }
            }

            return channels;
        }

        /// <inheritdoc/>
        public IEnumerable<BindableChannelGroup>? GetGroupedChannels(out IBindableSelectableChannel? selectedChannel)
        {
            var channels = GetGuildChannels(this, out selectedChannel);

            var groups = new Dictionary<ulong?, BindableChannelGroup>
            {
                { 0, new BindableChannelGroup(_messenger, _discordService, _quarrelClient, _dispatcherService, null) }
            };

            foreach (var channel in channels)
            {
                if (channel is BindableCategoryChannel bindableCategory)
                {
                    groups.Add(channel.Channel.Id, new BindableChannelGroup(_messenger, _discordService, _quarrelClient, _dispatcherService, bindableCategory));
                }
            }

            foreach (var channel in channels)
            {
                if (channel is not null && channel is not BindableCategoryChannel)
                {
                    ulong parentId = 0;
                    if (channel.Channel is INestedChannel nestedChannel)
                    {
                        parentId = nestedChannel.CategoryId ?? 0;
                    }

                    if (groups.TryGetValue(parentId, out var group))
                    {
                        group.AddChild(channel);
                    }
                }
            }

            if (groups[0].Children.Count == 0)
            {
                groups.Remove(0);
            }

            return groups.Values;
        }
    }
}
