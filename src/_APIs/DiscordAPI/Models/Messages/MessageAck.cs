// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.Models.Messages
{
    /// <summary>
    /// A model for marking a message as read.
    /// </summary>
    public class MessageAck
    {
        /// <summary>
        /// The id of the message.
        /// </summary>
        [JsonProperty("message_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the message's channel.
        /// </summary>
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
