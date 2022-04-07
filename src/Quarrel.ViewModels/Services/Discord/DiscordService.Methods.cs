// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Guilds;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.Services.Discord
{
    public partial class DiscordService
    {
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

        public BindableChannel?[] GetGuildChannels(Guild guild)
        {
            IGuildChannel[] rawChannels = guild.GetChannels();
            Array.Sort(rawChannels, Comparer<IGuildChannel>.Create((item1, item2) =>
            {
                return item1.Position.CompareTo(item2.Position);
            }));

            BindableChannel?[] channels = new BindableChannel[rawChannels.Length];
            for (int i = 0; i < rawChannels.Length; i++)
            {
                channels[i] = BindableChannel.Create(rawChannels[i]);
            }

            return channels;
        }

        public IEnumerable<BindableChannelGroup>? GetGuildChannelsGrouped(Guild guild)
        {
            var channels = GetGuildChannels(guild);

            Dictionary<ulong?, BindableChannelGroup> groups = new Dictionary<ulong?, BindableChannelGroup>();
            groups.Add(0, new BindableChannelGroup(null));
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
