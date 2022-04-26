// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System;
using System.Collections.Generic;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Guilds.Guild"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuild : SelectableItem, IBindableSelectableGuildItem, IBindableGuildListItem
    {
        [AlsoNotifyChangeFor(nameof(IconUrl))]
        [AlsoNotifyChangeFor(nameof(IconUri))]
        [ObservableProperty]
        private Guild _guild;

        internal BindableGuild(IDiscordService discordService, IDispatcherService dispatcherService, Guild guild) :
            base(discordService, dispatcherService)
        {
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

        /// <inheritdoc/>
        public IEnumerable<BindableChannelGroup>? GetGroupedChannels(out IBindableSelectableChannel? selectedChannel)
        {
            var channels = _discordService.GetGuildChannels(this, out selectedChannel);

            var groups = new Dictionary<ulong?, BindableChannelGroup>
            {
                { 0, new BindableChannelGroup(_discordService, _dispatcherService, null) }
            };

            foreach (var channel in channels)
            {
                if (channel is BindableCategoryChannel bindableCategory)
                {
                    groups.Add(channel.Channel.Id, new BindableChannelGroup(_discordService, _dispatcherService, bindableCategory));
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
