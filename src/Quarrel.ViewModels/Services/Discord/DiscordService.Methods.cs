// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Channels;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Messages;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Settings;
using Quarrel.Client.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        /// <inheritdoc/>
        public BindableSelfUser? GetMe()
        {
            SelfUser? user = _quarrelClient.GetMe();
            if (user is null)
            {
                return null;
            }

            return new BindableSelfUser(_dispatcherService, user);
        }
        
        /// <inheritdoc/>
        public BindableGuild[] GetMyGuilds()
        {
            Guild[] rawGuilds = _quarrelClient.GetMyGuilds();
            BindableGuild[] guilds = new BindableGuild[rawGuilds.Length];
            for (int i = 0; i < rawGuilds.Length; i++)
            {
                guilds[i] = new BindableGuild(_dispatcherService, rawGuilds[i]);
            }

            return guilds;
        }
        
        /// <inheritdoc/>
        public BindableGuildFolder[] GetMyGuildFolders()
        {
            GuildFolder[] rawFolders = _quarrelClient.GetMyGuildFolders();
            BindableGuildFolder[] folders = new BindableGuildFolder[rawFolders.Length];
            for (int i = 0; i < rawFolders.Length; i++)
            {
                folders[i] = new BindableGuildFolder(_dispatcherService, rawFolders[i]);
            }

            return folders;
        }
        
        /// <inheritdoc/>
        public async Task<BindableMessage[]> GetChannelMessagesAsync(IBindableMessageChannel channel)
        {
            var rawMessages = await _quarrelClient.GetMessagesAsync(channel.Id);
            Guard.IsNotNull(rawMessages, nameof(rawMessages));
            BindableMessage[] messages = new BindableMessage[rawMessages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = new BindableMessage(_dispatcherService, rawMessages[i]);
            }

            return messages;
        }
        
        /// <inheritdoc/>
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
                else if (is2Voice && !is1Voice)
                {
                    return -1;
                }

                return item1.Position.CompareTo(item2.Position);
            }));

            GuildMember? member = _quarrelClient.GetMyGuildMember(guild.Guild.Id);
            Guard.IsNotNull(member, nameof(member));
            BindableGuildChannel?[] channels = new BindableGuildChannel[rawChannels.Length];
            var categories = new Dictionary<ulong, BindableCategoryChannel>();
            
            // Once for categories
            for (int i = 0; i < rawChannels.Length; i++)
            {
                var channel = rawChannels[i];
                if (channel is CategoryChannel categoryChannel)
                {
                    var bindableCategoryChannel = new BindableCategoryChannel(_dispatcherService, categoryChannel, member);
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

                    channel = BindableGuildChannel.Create(_dispatcherService, nestedChannel, member, category);

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
        public IEnumerable<BindableChannelGroup>? GetGuildChannelsGrouped(BindableGuild guild, out IBindableSelectableChannel? selectedChannel)
        {
            var channels = GetGuildChannels(guild, out selectedChannel);

            var groups = new Dictionary<ulong?, BindableChannelGroup>
            {
                { 0, new BindableChannelGroup(_dispatcherService, null) }
            };

            foreach (var channel in channels)
            {
                if (channel is BindableCategoryChannel bindableCategory)
                {
                    groups.Add(channel.Channel.Id, new BindableChannelGroup(_dispatcherService, bindableCategory));
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
