// Copyright (c) Quarrel. All rights reserved.

using Newtonsoft.Json;

namespace DiscordAPI.API.Channel.Models
{
    /// <summary>
    /// A pending message model.
    /// </summary>
    public class MessageUpsert
    {
        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is to be read TTS.
        /// </summary>
        [JsonProperty("tts")]
        public bool TTS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating this uniqueness of this message in order to prevent duplicates
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
