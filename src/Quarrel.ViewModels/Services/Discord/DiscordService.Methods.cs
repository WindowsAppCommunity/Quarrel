// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Enums.Channels;
using Discord.API.Models.Guilds;
using Discord.API.Models.Settings;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
        public SelfUser? GetMe()
        {
            return _discordClient.GetMe();
        }

        public BindableGuild[] GetMyGuilds()
        {
            Guild[] rawGuilds = _discordClient.GetMyGuilds();
            BindableGuild[] guilds = new BindableGuild[rawGuilds.Length];
            for (int i = 0; i < rawGuilds.Length; i++)
            {
                guilds[i] = new BindableGuild(rawGuilds[i]);
            }

            return guilds;
        }

        public BindableGuildFolder[] GetMyGuildFolders()
        {
            GuildFolder[] rawFolders = _discordClient.GetMyGuildFolders();
            BindableGuildFolder[] folders = new BindableGuildFolder[rawFolders.Length];
            for (int i = 0; i < rawFolders.Length; i++)
            {
                folders[i] = new BindableGuildFolder(rawFolders[i]);
            }

            return folders;
        }

        public async Task<BindableMessage[]> GetChannelMessagesAsync(Channel channel)
        {
            var rawMessages = await _discordClient.GetMessagesAsync(channel.Id);
            BindableMessage[] messages = new BindableMessage[rawMessages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = new BindableMessage(rawMessages[i]);
            }

            return messages;
        }

        public BindableChannel?[] GetGuildChannels(Guild guild)
        {
            IGuildChannel[] rawChannels = guild.GetChannels();
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

            GuildMember? member = _discordClient.GetMyGuildMember(guild.Id);
            Guard.IsNotNull(member, nameof(member));
            BindableChannel?[] channels = new BindableChannel[rawChannels.Length];
            var categories = new Dictionary<ulong, BindableCategoryChannel>();
            
            // Once for categories
            for (int i = 0; i < rawChannels.Length; i++)
            {
                var channel = rawChannels[i];
                if (channel is CategoryChannel categoryChannel)
                {
                    var bindableCategoryChannel = new BindableCategoryChannel(categoryChannel, member);
                    categories.Add(channel.Id, bindableCategoryChannel);
                    channels[i] = bindableCategoryChannel;
                }
            }

            for (int i = 0; i < rawChannels.Length; i++)
            {
                ref BindableChannel? channel = ref channels[i];
                if (channel is null && rawChannels[i] is INestedChannel nestedChannel)
                {
                    BindableCategoryChannel? category = null;
                    if (nestedChannel.CategoryId.HasValue)
                    {
                        category = categories[nestedChannel.CategoryId.Value];
                    }

                    channel = BindableChannel.Create(nestedChannel, member, category);
                }
            }

            return channels;
        }

        public IEnumerable<BindableChannelGroup>? GetGuildChannelsGrouped(Guild guild)
        {
            var channels = GetGuildChannels(guild);

            var groups = new Dictionary<ulong?, BindableChannelGroup>
            {
                { 0, new BindableChannelGroup(null) }
            };

            foreach (var channel in channels)
            {
                if (channel is BindableCategoryChannel bindableCategory)
                {
                    groups.Add(channel.Channel.Id, new BindableChannelGroup(bindableCategory));
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
