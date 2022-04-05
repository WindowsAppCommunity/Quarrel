// Adam Dernis © 2022

using Discord.API.Gateways;
using Discord.API.Models.Messages;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Messages;

namespace Discord.API
{
    /// <inheritdoc/>
    public partial class DiscordClient
    {
        private void OnMessageCreated(object sender, GatewayEventArgs<JsonMessage> e)
        {
            JsonMessage? message = e.EventData;
            if (message is not null)
            {
                if (_channelMap.TryGetValue(message.ChannelId, out Channel channel))
                {
                    if (channel is IMessageChannel messageChannel)
                    {
                        messageChannel.LastMessageId = message.Id;
                    }
                }
            }
        }

        private void OnMessageUpdated(object sender, GatewayEventArgs<JsonMessage> e)
        {
            JsonMessage? message = e.EventData;
            if (message is not null)
            {
                MessageUpdated?.Invoke(this, new Message(message, this));
            }
        }

        private void OnMessageAck(object sender, GatewayEventArgs<MessageAck> e)
        {
            MessageAck? messageAck = e.EventData;
            if (messageAck is not null)
            {
                if (_channelMap.TryGetValue(messageAck.ChannelId, out Channel channel))
                {
                    if (channel is IMessageChannel messageChannel)
                    {
                        messageChannel.LastMessageId = messageAck.MessageId;
                    }
                }
            }
        }
    }
}
