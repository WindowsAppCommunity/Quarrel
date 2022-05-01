// Quarrel © 2022

using Quarrel.Client.Models.Channels.Abstract;

namespace Quarrel.Messages.Discord.Channels
{
    public class ChannelUpdatedMessage
    {
        public ChannelUpdatedMessage(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }
    }
}
