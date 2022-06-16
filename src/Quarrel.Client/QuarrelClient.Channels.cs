// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s channels.
        /// </summary>
        public class QuarrelClientChannels
        {
            private readonly QuarrelClient _client;
            private readonly ConcurrentDictionary<ulong, Channel> _channelMap;
            private readonly HashSet<ulong> _privateChannels;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientChannels"/> class.
            /// </summary>
            internal QuarrelClientChannels(QuarrelClient client)
            {
                _client = client;

                _channelMap = new();
                _privateChannels = new();
            }

            public IPrivateChannel[] GetPrivateChannels()
            {
                IPrivateChannel[] privateChannels = new IPrivateChannel[_privateChannels.Count];
                int i = 0;
                foreach (var channel in _privateChannels.Select(GetChannel))
                {
                    if (channel is IPrivateChannel directChannel)
                    {
                        privateChannels[i] = directChannel;
                        i++;
                    }
                }

                // Nullability is improperly accessed here
#pragma warning disable CS8629
                Array.Resize(ref privateChannels, i);
                Array.Sort(privateChannels, Comparer<IPrivateChannel>.Create((item1, item2) =>
                {
                    bool i1Null = !item1.LastMessageId.HasValue;
                    bool i2Null = !item2.LastMessageId.HasValue;

                    if (i1Null && i2Null) return 0;
                    if (i2Null) return -1;
                    if (i1Null) return 1;

                    long compare = (long)item2.LastMessageId.Value - (long)item1.LastMessageId.Value;
                    return compare switch
                    {
                        < 0 => -1,
                        > 0 => 1,
                        _ => 0
                    };
                }));
#pragma warning restore CS8629

                return privateChannels;
            }

            /// <summary>
            /// Starts a call in a private channel.
            /// </summary>
            /// <param name="channelId">The id of the audio channel.</param>
            public async Task StartCall(ulong channelId)
            {
                await _client.Voice.RequestStartCall(channelId);
            }

            /// <summary>
            /// Joins a call in an audio channel.
            /// </summary>
            /// <param name="channelId">The id of the audio channel.</param>
            /// <param name="guildId">The id of the guild of the audio channel.</param>
            public async Task JoinCall(ulong channelId, ulong? guildId = null)
            {
                await _client.Voice.RequestJoinVoice(channelId, guildId);
            }

            /// <summary>
            /// Leaves any active call.
            /// </summary>
            public async Task LeaveCall()
            {
                await _client.Voice.RequestJoinVoice(null, null);
            }

            internal Channel? GetChannel(ulong channelId)
            {
                if (_channelMap.TryGetValue(channelId, out Channel channel))
                {
                    return channel;
                }

                return null;
            }

            internal bool AddReadState(JsonReadState jsonReadState)
            {
                Channel? channel = GetChannel(jsonReadState.ChannelId);

                if (channel is not IMessageChannel messageChannel) return false;

                messageChannel.MentionCount = jsonReadState.MentionCount;
                messageChannel.LastReadMessageId = jsonReadState.LastMessageId;
                return true;

            }

            internal Channel? AddChannel(JsonChannel jsonChannel, ulong? guildId = null)
            {
                guildId = jsonChannel.GuildId ?? guildId;
                Channel? channel = Channel.FromJsonChannel(jsonChannel, _client, guildId);
                if (channel == null || !_channelMap.TryAdd(channel.Id, channel)) return null;

                if (guildId.HasValue)
                {
                    _client.Guilds.AddChannel(guildId.Value, channel.Id);
                }
                else if (jsonChannel.Recipients is not null)
                {
                    foreach (var recipient in jsonChannel.Recipients)
                    {
                        _client.Users.AddUser(recipient);
                    }

                    _privateChannels.Add(channel.Id);
                }

                return channel;

            }

            internal bool UpdateChannel(JsonChannel jsonChannel, out Channel channel)
            {
                if (!_channelMap.TryGetValue(jsonChannel.Id, out channel)) return false;

                channel.UpdateFromJsonChannel(jsonChannel);
                return true;

            }

            internal Channel? RemoveChannel(ulong channelId)
            {
                if (_channelMap.TryRemove(channelId, out Channel channel))
                {
                    if (channel is IGuildChannel guildChannel)
                    {
                        _client.Guilds.RemoveChannel(guildChannel.GuildId, channelId);
                    }
                    else
                    {
                        _privateChannels.Remove(channelId);
                    }

                    return channel;
                }

                return null;
            }
        }
    }
}
