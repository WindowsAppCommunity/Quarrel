// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Voice;
using Discord.API.Voice;
using Quarrel.Client.Logger;
using Quarrel.Client.Models.Voice;
using System.Collections.Generic;
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
            private readonly object _stateLock = new();
            private readonly Dictionary<ulong, VoiceState> _stateDictionary;
            private JsonVoiceState? _selfState;
            private VoiceConnection? _voiceConnection;
            private VoiceServerConfig? _voiceServerConfig;

            /// <summary>
            /// Initializes a new instance of the <see cref="QuarrelClientVoice"/> class.
            /// </summary>
            internal QuarrelClientVoice(QuarrelClient client)
            {
                _client = client;
                _stateDictionary = new Dictionary<ulong, VoiceState>();
            }

            internal void UpdateVoiceServerConfig(VoiceServerConfig config)
            {
                lock(_stateLock)
                {
                    _voiceServerConfig = config;
                    if (_selfState != null)
                    {
                        _ = ConnectToVoice();
                    }
                }
            }

            internal void UpdateSelfVoiceState(JsonVoiceState state)
            {
                lock (_stateLock)
                {
                    _selfState = state;

                    if (_voiceServerConfig != null)
                    {
                        _ = ConnectToVoice();
                    }
                }
            }

            internal void UpdateVoiceState(JsonVoiceState state)
            {
                if (_stateDictionary.TryGetValue(state.UserId, out VoiceState voiceState))
                {
                    // The channel was not added
                    bool channelChanged = state.ChannelId != voiceState.Channel?.Id;

                    voiceState.Update(state);
                    if (channelChanged)
                    {
                        // The channel was moved or removed.
                        _stateDictionary.Remove(state.UserId);
                        _client.VoiceStateRemoved?.Invoke(_client, voiceState);
                    }
                }

                voiceState = new VoiceState(state, _client);

                if (voiceState.Channel is not null)
                {
                    _stateDictionary.Add(state.UserId, voiceState);
                    _client.VoiceStateAdded?.Invoke(_client, voiceState);
                }

                _client.VoiceStateUpdated?.Invoke(_client, voiceState);
            }

            internal async Task RequestStartCall(ulong channelId)
            {
                Guard.IsNotNull(_client.ChannelService, nameof(_client.ChannelService));
                Guard.IsNotNull(_client.Gateway, nameof(_client.Gateway));

                await _client.MakeRefitRequest(() => _client.ChannelService.StartCall(channelId, new JsonRecipients()));
                await RequestJoinVoice(channelId);
            }

            internal async Task RequestJoinVoice(ulong? channelId, ulong? guildId = null)
            {
                Guard.IsNotNull(_client.Gateway, nameof(_client.Gateway));
                _selfState = null;
                _voiceConnection = null;
                await _client.Gateway.VoiceStatusUpdateAsync(channelId, guildId);
            }

            private async Task ConnectToVoice()
            {
                if (_voiceServerConfig == null || _selfState == null && _voiceConnection == null)
                {
                    return;
                }

                _voiceConnection = new VoiceConnection(_voiceServerConfig.Json, _selfState!,
                    unhandledMessageEncountered: e => _client.LogException(ClientLogEvent.VoiceExceptionHandled, e),
                    knownOperationEncountered: e => _client.LogOperation(ClientLogEvent.KnownVoiceOperationEncountered, (int)e),
                    unhandledOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnhandledVoiceOperationEncountered, (int)e),
                    unknownOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnknownVoiceOperationEncountered, e),
                    voiceConnectionStatusChanged: _ => { },
                    ready: _ => { });

                await _voiceConnection.ConnectAsync(_voiceServerConfig.ConnectionUrl);
            }
        }
    }
}
