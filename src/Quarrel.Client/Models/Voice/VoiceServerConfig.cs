// Quarrel © 2022

using Discord.API.Gateways.Models.Channels;

namespace Quarrel.Client.Models.Voice
{
    public class VoiceServerConfig
    {
        // TODO: Investigate upgrade to version 7
        private const int Version = 7;

        internal VoiceServerConfig(JsonVoiceServerUpdate json)
        {
            GuildId = json.GuildId;
            ChannelId = json.ChannelId;
            Endpoint = json.Endpoint;
            Token = json.Token;

            Json = json;
        }

        public ulong? GuildId { get; }

        public ulong ChannelId { get; }

        public string Endpoint { get; }

        public string Token { get; }

        public string ConnectionUrl => $"wss://{Endpoint.Substring(0, Endpoint.LastIndexOf(':'))}?v={Version}";

        internal JsonVoiceServerUpdate Json { get; }
    }
}
