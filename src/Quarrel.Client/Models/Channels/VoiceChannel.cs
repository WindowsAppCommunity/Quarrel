// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Settings;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Voice;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.Client.Models.Channels
{
    /// <summary>
    /// A voice channel in a guild managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class VoiceChannel : GuildChannel, IGuildVoiceChannel
    {
        private readonly HashSet<ulong> _users;

        internal VoiceChannel(JsonChannel restChannel,
            ulong? guildId, 
            ChannelSettings? settings,
            QuarrelClient context) :
            base(restChannel, guildId, context)
        {
            Guard.IsNotNull(restChannel.Bitrate, nameof(restChannel.Bitrate));

            Bitrate = restChannel.Bitrate.Value;
            UserLimit = restChannel.UserLimit;
            RTCRegion = restChannel.RTCRegion;
            CategoryId = restChannel.CategoryId;

            IsMuted = settings?.Muted ?? false;

            _users = new HashSet<ulong>();
        }

        /// <inheritdoc/>
        public int Bitrate { get; private set; }

        /// <inheritdoc/>
        public int? UserLimit { get; private set; }

        /// <inheritdoc/>
        public ulong? CategoryId { get; private set; }

        /// <inheritdoc cref="IMessageChannel.MentionCount"/>
        public int? MentionCount { get; internal set; }

        /// <inheritdoc cref="IMessageChannel.LastMessageId"/>
        public ulong? LastMessageId { get; internal set; }

        /// <inheritdoc cref="IMessageChannel.LastReadMessageId"/>
        public ulong? LastReadMessageId { get; internal set; }
        
        /// <inheritdoc/>
        public bool IsMuted { get; private set; }

        /// <inheritdoc/>
        public bool IsUnread => LastMessageId > LastReadMessageId;

        /// <inheritdoc/>
        public string? RTCRegion { get; private set; }
        
        /// <inheritdoc/>
        public VoiceState[] GetVoiceStates()
        {
            VoiceState[] states = new VoiceState[_users.Count];
            int i = 0;
            foreach (var user in _users)
            {
                states[i] = Context.Voice.GetVoiceState(user)!;
                i++;
            }

            return states;
        }
        
        /// <inheritdoc/>
        public void AddVoiceMember(ulong userId)
            => _users.Add(userId);
        
        /// <inheritdoc/>
        public void RemoveVoiceMember(ulong userId)
            => _users.Remove(userId);

        int? IMessageChannel.MentionCount
        {
            get => MentionCount;
            set => MentionCount = value;
        }

        ulong? IMessageChannel.LastMessageId
        {
            get => LastMessageId;
            set => LastMessageId = value;
        }

        ulong? IMessageChannel.LastReadMessageId
        {
            get => LastReadMessageId;
            set => LastReadMessageId = value;
        }

        internal override void PrivateUpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            base.PrivateUpdateFromJsonChannel(jsonChannel);
            Bitrate = jsonChannel.Bitrate ?? Bitrate;
            UserLimit = jsonChannel.UserLimit ?? UserLimit;
            CategoryId = jsonChannel.CategoryId ?? CategoryId;
            RTCRegion = jsonChannel.RTCRegion ?? RTCRegion;
        }

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            restChannel.Bitrate = Bitrate;
            restChannel.UserLimit = UserLimit;
            restChannel.CategoryId = CategoryId;
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            restChannel.RTCRegion = RTCRegion;
            return restChannel;
        }
    }
}
