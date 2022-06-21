// Quarrel © 2022

using Discord.API.Gateways.Models.Channels;

namespace Quarrel.Client.Models.Voice
{
    /// <summary>
    /// A model for a voice server's config info.
    /// </summary>
    public class VoiceServerConfig
    {
        private const int Version = 7;

        internal VoiceServerConfig(JsonVoiceServerUpdate json)
        {
            GuildId = json.GuildId;
            ChannelId = json.ChannelId;
            Endpoint = json.Endpoint;
            Token = json.Token;

            Json = json;
        }

        /// <summary>
        /// Gets the id of the guild the audio channel is in.
        /// </summary>
        public ulong? GuildId { get; }

        /// <summary>
        /// Gets the id of the audio channel.
        /// </summary>
        public ulong? ChannelId { get; }
        
        /// <summary>
        /// Gets the voice server's endpoint.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// Gets the voice server's token.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Gets the connection url for the voice server websocket.
        /// </summary>
        public string ConnectionUrl => $"wss://{Endpoint.Substring(0, Endpoint.LastIndexOf(':'))}?v={Version}";

        internal JsonVoiceServerUpdate Json { get; }
    }
}
