// Quarrel © 2022

using Discord.API.Gateways;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Messages;

namespace Quarrel.Client
{
    /// <inheritdoc/>
    public partial class QuarrelClient
    {
        private void OnMessageCreated(JsonMessage message)
        {
            if (_channelMap.TryGetValue(message.ChannelId, out Channel channel))
            {
                if (channel is IMessageChannel messageChannel)
                {
                    messageChannel.LastMessageId = message.Id;
                }
            }

            // TODO: Channel registration
            MessageCreated?.Invoke(this, new Message(message, this));
        }

        private void OnMessageUpdated(JsonMessage message)
        {
            MessageUpdated?.Invoke(this, new Message(message, this));
        }

        private void OnMessageAck(JsonMessageAck messageAck)
        {
            if (_channelMap.TryGetValue(messageAck.ChannelId, out Channel channel))
            {
                if (channel is IMessageChannel messageChannel)
                {
                    messageChannel.LastMessageId = messageAck.MessageId;
                }
            }

            // TODO: Channel registration
            MessageAck?.Invoke(this, new MessageAck(messageAck, this));
        }
    }
}
