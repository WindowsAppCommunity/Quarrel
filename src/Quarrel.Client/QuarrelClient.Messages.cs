// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Messages;
using System;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s messages.
        /// </summary>
        public class QuarrelClientMessages
        {
            private readonly QuarrelClient _client;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientUsers"/> class.
            /// </summary>
            public QuarrelClientMessages(QuarrelClient client)
            {
                _client = client;
            }

            /// <summary>
            /// Gets messages in a channel.
            /// </summary>
            /// <param name="channelId">The channel's id.</param>
            /// <param name="guildId">The id of the guild the channel belongs to.</param>
            /// <param name="beforeId">The message to get the messages from before.</param>
            public async Task<Message[]> GetMessagesAsync(ulong channelId, ulong? guildId = null, ulong? beforeId = null)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(_client.ChannelService));

                System.Func<Task<JsonMessage[]>> request = () => _client.ChannelService.GetChannelMessages(channelId);
                if (beforeId.HasValue)
                {
                    request = () => _client.ChannelService.GetChannelMessagesBefore(channelId, beforeId.Value);
                }

                JsonMessage[] jsonMessages = await _client.MakeRefitRequest(request) ?? Array.Empty<JsonMessage>();

                Message[] messages = new Message[jsonMessages.Length];
                for (int i = 0; i < messages.Length; i++)
                {
                    jsonMessages[i].GuildId = jsonMessages[i].GuildId ?? guildId;
                    messages[i] = new Message(jsonMessages[i], _client);
                }

                return messages;
            }

            /// <summary>
            /// Sends a message.
            /// </summary>
            /// <param name="channelId">The channel to send the message in.</param>
            /// <param name="content">The content of the message.</param>
            public async Task SendMessage(ulong channelId, string content)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(_client.ChannelService));
                ulong nonce = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() << 22;
                var message = new JsonMessageUpsert(content, false, $"{nonce}");
                await _client.MakeRefitRequest(() => _client.ChannelService.CreateMessage(channelId, message));
            }

            /// <summary>
            /// Deletes a message.
            /// </summary>
            /// <param name="channelId">The id of channel the message belongs to.</param>
            /// <param name="messageId">The id of the message to delete.</param>
            public async Task DeleteMessage(ulong channelId, ulong messageId)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(_client.ChannelService));
                await _client.MakeRefitRequest(() => _client.ChannelService.DeleteMessage(channelId, messageId));
            }

            /// <summary>
            /// Marks a message as the last read message in the channel.
            /// </summary>
            /// <param name="channelId">The id of the channel.</param>
            /// <param name="messageId">The id of the message.</param>
            /// <returns></returns>
            public async Task MarkRead(ulong channelId, ulong messageId)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(ChannelService));
                await _client.MakeRefitRequest(() => _client.ChannelService.MarkRead(channelId, messageId));
            }
        }
    }
}
