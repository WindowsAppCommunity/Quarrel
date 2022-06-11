// Quarrel © 2022

using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    /// <summary>
    /// A model for creating a message.
    /// </summary>
    internal record JsonMessageUpsert
    {
        public JsonMessageUpsert(string content, bool tts, string nonce)
        {
            Content = content;
            TTS = tts;
            Nonce = nonce;
        }

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is to be read TTS.
        /// </summary>
        [JsonPropertyName("tts")]
        public bool TTS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating this uniqueness of this message in order to prevent duplicates.
        /// </summary>
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }
    }
}
