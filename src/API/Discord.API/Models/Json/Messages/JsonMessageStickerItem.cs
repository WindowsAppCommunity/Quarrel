// Quarrel © 2022

using Discord.API.Models.Enums.Stickers;
using System.Text.Json.Serialization;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Discord.API.Models.Json.Messages
{
    internal class JsonMessageStickerItem
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("format_type")]
        public StickerFormatType FormatType { get; set; }
    }
}
