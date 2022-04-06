// Adam Dernis © 2022

using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Base;

namespace Discord.API.Models.Managed.Messages
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
