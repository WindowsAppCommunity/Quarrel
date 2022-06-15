// Quarrel © 2022

using Discord.API.Gateways.Models.Channels;
using Discord.API.Models.Json.Voice;
using Discord.API.Voice;
using Quarrel.Client.Models.Voice;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public partial class QuarrelClient
    {
        /// <summary>
        /// A class managing the <see cref="QuarrelClient"/>'s voice connection.
        /// </summary>
        public class QuarrelClientVoice
        {
            private readonly QuarrelClient _client;
            private VoiceConnection? _voiceConnection;
            private JsonVoiceState _voiceState;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientVoice"/> class.
            /// </summary>
            internal QuarrelClientVoice(QuarrelClient client)
            {
                _client = client;
                _voiceState = new JsonVoiceState();
            }

            internal async Task ConnectToCall(VoiceServerConfig config)
            {
                _voiceConnection = new VoiceConnection(config.Json, _voiceState,
                    unhandledMessageEncountered: _client.OnUnhandledVoiceMessageEncountered,
                    unknownEventEncountered: _client.OnUnknownVoiceEventEncountered,
                    unknownOperationEncountered: _client.OnUnknownVoiceOperationEncountered,
                    knownEventEncountered: _client.OnKnownVoiceEventEncountered,
                    unhandledOperationEncountered: _client.OnUnhandledVoiceOperationEncountered,
                    unhandledEventEncountered: _client.OnUnhandledVoiceEventEncountered,
                    voiceConnectionStatusChanged: _ => { },
                    ready: _ => { });

                await _voiceConnection.ConnectAsync(config.ConnectionUrl);
            }
        }
    }
}
