// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Gateway.Models.Handshake
{
    internal class InvalidSession
    {
        [JsonPropertyName("d")]
        public bool ConnectedState { get; set; }
    }
}
