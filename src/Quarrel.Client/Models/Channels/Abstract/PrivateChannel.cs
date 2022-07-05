// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Voice;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.Client.Models.Channels.Abstract
{
    /// <summary>
    /// The base class for private channels.
    /// </summary>
    public abstract class PrivateChannel : Channel, IPrivateChannel
    {
        private readonly HashSet<ulong> _users;

        internal PrivateChannel(JsonChannel restChannel, QuarrelClient context) :
            base(restChannel, context)
        {
            LastMessageId = restChannel.LastMessageId;
            RTCRegion = restChannel.RTCRegion;

            _users = new HashSet<ulong>();
        }

        /// <inheritdoc/>
        public int? MentionCount { get; private set; }

        /// <inheritdoc/>
        public ulong? LastMessageId { get; private set; }

        /// <inheritdoc/>
        public ulong? LastReadMessageId { get; private set; }

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

        /// <inheritdoc/>
        public bool IsUnread => LastMessageId > LastReadMessageId;

        /// <inheritdoc/>
        public override string Url => $"https://discord.com/channels/@me/{Id}";

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
    }
}
