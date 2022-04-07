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

        public BindableChannel?[] GetGuildChannelsHierarchy(Guild guild)
        {
            // Get raw channels
            IGuildChannel[] rawChannels = guild.GetChannels();
            Array.Sort(rawChannels, Comparer<IGuildChannel>.Create((item1, item2) =>
            {
                return item1.Position.CompareTo(item2.Position);
            }));
            
            // Scan for categories
            var categories = new Dictionary<ulong, BindableCategoryChannel>();
            var rootChannels = new List<BindableChannel>();
            foreach (var channel in rawChannels)
            {
                if (channel is CategoryChannel category)
                {
                    BindableCategoryChannel bindableChannel = new BindableCategoryChannel(category);
                    categories.Add(category.Id, bindableChannel);
                    rootChannels.Add(bindableChannel);
                }
            }

            // Add remaining channels either as children or to the root
            foreach (var channel in rawChannels)
            {
                BindableChannel? bindableChannel = BindableChannel.Create(channel);
                if (bindableChannel is null || categories.ContainsKey(channel.Id))
                {
                    continue;
                }

                if (channel is INestedChannel nestedChannel && nestedChannel.CategoryId is not null)
                {
                    if (categories.TryGetValue(nestedChannel.CategoryId.Value, out var nestedCategory))
                    {
                        nestedCategory.AddChild(bindableChannel);
                        continue;
                    }
                }

                rootChannels.Add(bindableChannel);
            }

            return rootChannels.ToArray();
        }
    }
}
