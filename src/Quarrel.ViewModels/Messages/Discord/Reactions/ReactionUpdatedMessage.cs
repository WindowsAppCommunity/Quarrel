// Quarrel © 2022

using Quarrel.Client.Models.Emojis;

namespace Quarrel.Messages.Discord.Reactions
{
    public class ReactionUpdatedMessage
    {
        public ReactionUpdatedMessage(
            ulong userId,
            ulong channelId,
            ulong messageId,
            Emoji emoji,
            bool removed)
        {
            UserId = userId;
            ChannelId = channelId;
            MessageId = messageId;
            Emoji = emoji;
            Removed = removed;
        }

        public ulong UserId { get; }

        public ulong ChannelId { get; }

        public ulong MessageId { get; }

        public Emoji Emoji { get; }

        public bool Removed { get; }
    }
}
