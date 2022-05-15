// Quarrel © 2022

using Quarrel.Client.Models.Channels.Abstract;

namespace Quarrel.Messages.Discord.Channels
{
    /// <summary>
    /// A message sent when a channel is updated.
    /// </summary>
    public class ChannelUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelUpdatedMessage"/> class.
        /// </summary>
        /// <param name="channel">The channel updated.</param>
        public ChannelUpdatedMessage(Channel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Gets the updated channel.
        /// </summary>
        public Channel Channel { get; }
    }
}
