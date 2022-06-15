// Quarrel © 2022

using CommunityToolkit.Diagnostics;
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
            private JsonVoiceState? _voiceState;
            private VoiceServerConfig? _voiceServerConfig;
            private readonly object _stateLock = new();
            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientVoice"/> class.
            /// </summary>
            internal QuarrelClientVoice(QuarrelClient client)
            {
                _client = client;
            }

            internal void UpdateVoiceServerConfig(VoiceServerConfig config)
            {
                lock(_stateLock)
                {
                    _voiceServerConfig = config;
                    if (_voiceState != null)
                    {
                        _ = ConnectToVoice();
                    }
                }
            }

            internal void UpdateVoiceState(JsonVoiceState state)
            {
                lock (_stateLock)
                {
                    if (_voiceState == null && _voiceServerConfig != null)
                    {
                        _voiceState = state;
                        _ = ConnectToVoice();
                    }
                    else
                    {
                        _voiceState = state;
                    }
                }

            }

            internal async Task RequestStartCall(ulong channelId, ulong? guildId = null)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(_client.ChannelService));
                Guard.IsNotNull(_client.Gateway, nameof(_client.Gateway));

                await _client.MakeRefitRequest(() => _client.ChannelService.StartCall(channelId, new JsonRecipients()));
                await RequestJoinVoice(channelId);
            }

            internal async Task RequestJoinVoice(ulong channelId, ulong? guildId = null)
            {
                Guard.IsNotNull(_client.Gateway, nameof(_client.Gateway));
                _voiceState = null;
                _voiceConnection = null;
                await _client.Gateway.VoiceStatusUpdateAsync(channelId, guildId);
            }

            private async Task ConnectToVoice()
            {
                if (_voiceServerConfig == null || _voiceState == null && _voiceConnection == null)
                {
                    return;
                }

                _voiceConnection = new VoiceConnection(_voiceServerConfig.Json, _voiceState,
                    unhandledMessageEncountered: _client.OnUnhandledVoiceMessageEncountered,
                    unknownOperationEncountered: _client.OnUnknownVoiceOperationEncountered,
                    unhandledOperationEncountered: _client.OnUnhandledVoiceOperationEncountered,
                    voiceConnectionStatusChanged: _ => { },
                    ready: _ => { });

                await _voiceConnection.ConnectAsync(_voiceServerConfig.ConnectionUrl);
            }
        }
    }
}
