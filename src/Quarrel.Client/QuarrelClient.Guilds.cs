// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Guilds;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Guilds;
using Quarrel.Client.Models.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s guilds.
        /// </summary>
        public class QuarrelClientGuilds
        {
            private readonly QuarrelClient _client;
            private readonly ConcurrentDictionary<ulong, Guild> _guildMap;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientGuilds"/> class.
            /// </summary>
            internal QuarrelClientGuilds(QuarrelClient client)
            { 
                _client = client;
                _guildMap = new ConcurrentDictionary<ulong, Guild>();
            }

            /// <summary>
            /// Gets the user's guild according to their order in settings.
            /// </summary>
            public Guild[] GetMyGuilds()
            {
                Guard.IsNotNull(_client.Self.Settings, nameof(_client.Self.Settings));

                ulong[] order = _client.Self.Settings.GuildOrder;
                Guild[] guildArray = new Guild[order.Length];

                int realCount = 0;
                for (int i = 0; i < order.Length; i++)
                {
                    Guild? guild = GetGuild(order[realCount]);
                    if (guild is not null)
                    {
                        guildArray[i] = guild;
                        realCount++;
                    }
                }

                Array.Resize(ref guildArray, realCount);

                return guildArray;
            }

            /// <summary>
            /// Modifies a guild.
            /// </summary>
            /// <param name="id">The id of the guild.</param>
            /// <param name="modifyGuild">The guild modifications.</param>
            public async Task ModifyGuild(ulong id, ModifyGuild modifyGuild)
            {
                Guard.IsNotNull(_client.GuildService, nameof(_client.GuildService));
                await _client.GuildService.ModifyGuild(id, modifyGuild.ToJsonModel());
            }

            /// <summary>
            /// Gets the guild folders for the current user.
            /// </summary>
            /// <remarks>
            /// Folders with null id should be handled as if they didn't exist and their children are actually in the root list.
            /// </remarks>
            public GuildFolder[] GetMyGuildFolders()
            {
                Guard.IsNotNull(_client.Self.Settings, nameof(_client.Self.Settings));
                return _client.Self.Settings.Folders;
            }

            internal Guild? GetGuild(ulong guildId)
            {
                return _guildMap.TryGetValue(guildId, out Guild guild) ? guild : null;
            }

            internal bool AddGuild(JsonGuild jsonGuild, GuildSettings? settings = null, Dictionary<ulong, ChannelSettings>? channelSettings = null)
            {
                var guild = new Guild(jsonGuild, settings, _client);
                if (_guildMap.TryAdd(guild.Id, guild))
                {
                    foreach (var jsonChannel in jsonGuild.Channels)
                    {
                        ChannelSettings? thisChannelSettings = null;
                        channelSettings?.TryGetValue(jsonChannel.Id, out thisChannelSettings);

                        var channel = _client.Channels.AddChannel(jsonChannel, guild.Id, thisChannelSettings);
                        if (channel is not null)
                        {
                            guild.AddChannel(jsonChannel.Id);
                        }
                    }

                    foreach (var member in jsonGuild.Members)
                    {
                        _client.Members.AddGuildMember(guild.Id, member);
                    }

                    foreach (var voiceState in jsonGuild.VoiceStates)
                    {
                        _client.Voice.UpdateVoiceState(voiceState);
                    }

                    return true;
                }

                return false;
            }

            internal bool UpdateGuild(JsonGuild jsonGuild)
            {
                if (!_guildMap.TryGetValue(jsonGuild.Id, out Guild guild)) return false;

                guild.UpdateFromRestGuild(jsonGuild);
                return true;

            }

            internal bool RemoveGuild(ulong guildId)
            {
                if (!_guildMap.TryRemove(guildId, out Guild guild)) return false;

                foreach (var channelId in guild.ChannelIds)
                {
                    _client.Channels.RemoveChannel(channelId);
                }

                return true;

            }

            internal bool AddChannel(ulong guildId, ulong channelId)
            {
                var guild = GetGuild(guildId);
                if (guild is null) return false;

                guild.RemoveChannel(channelId);
                return true;

            }

            internal bool RemoveChannel(ulong guildId, ulong channelId)
            {
                var guild = GetGuild(guildId);
                if (guild is null) return false;

                guild.RemoveChannel(channelId);
                return true;

            }
        }
    }
}
