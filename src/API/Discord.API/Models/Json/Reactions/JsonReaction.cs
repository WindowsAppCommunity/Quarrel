// Adam Dernis © 2022

using Discord.API.Models.Json.Emojis;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Reactions
{
    internal class JsonReaction
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("me")]
        public bool Me { get; set; }

        [JsonPropertyName("emoji")]
        public JsonEmoji Emoji { get; set; }
    }
}
