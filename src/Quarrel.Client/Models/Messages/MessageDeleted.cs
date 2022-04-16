// Quarrel © 2022

using Discord.API.Gateways.Models.Messages;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Messages
{
    /// <summary>
    /// A class containing data for when a message is deleted.
    /// </summary>
    public class MessageDeleted : DiscordItem
    {
        internal MessageDeleted(JsonMessageDeleted jsonMessageDeleted, DiscordClient context) : base(context)
        {
            ChannelId = jsonMessageDeleted.ChannelId;
            MessageId = jsonMessageDeleted.MessageId;
        }

        /// <summary>
        /// Gets the id of the channel where the message was deleted.
        /// </summary>
        public ulong ChannelId { get; }

        /// <summary>
        /// Gets the id of deleted message.
        /// </summary>
        public ulong MessageId { get; }
    }
}
