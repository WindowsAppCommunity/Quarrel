// Quarrel © 2022

using Discord.API.Gateways.Models.Messages;
using Quarrel.Client.Models.Base;

namespace Quarrel.Client.Models.Messages
{
    /// <summary>
    /// A class containing data for a channel read state being updated.
    /// </summary>
    public class MessageAck : DiscordItem
    {
        internal MessageAck(JsonMessageAck jsonMessageAck, DiscordClient context) : base(context)
        {
            ChannelId = jsonMessageAck.ChannelId;
            MessageId = jsonMessageAck.MessageId;
        }

        /// <summary>
        /// The id of the channel where the read state is being updated.
        /// </summary>
        public ulong ChannelId { get; }

        /// <summary>
        /// The id of the latest message marked read in the channel.
        /// </summary>
        public ulong MessageId { get; }
    }
}
