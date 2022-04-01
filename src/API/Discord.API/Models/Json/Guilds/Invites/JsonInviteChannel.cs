// Adam Dernis © 2022

using Discord.API.Models.Enums.Channels;
using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Guilds.Invites
{
    internal class JsonInviteChannel
    {
        [JsonPropertyName("id"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }
    }
}
