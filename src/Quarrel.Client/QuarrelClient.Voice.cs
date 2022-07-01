// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models;
using Discord.API.Models.Json.Voice;
using Discord.API.Voice;
using Discord.API.Voice.Models;
using Discord.API.Voice.Models.Handshake;
using Quarrel.Client.Logger;
using Quarrel.Client.Models.Voice;
using System;
using System.Collections.Generic;
using System.Linq;
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
            private VoiceServerConfig? _voiceServerConfig;
            private VoiceConnection? _voiceConnection;
            private ulong? _serverId;

            public Dictionary<string, StreamConnection> StreamConnections { get; } = new Dictionary<string, StreamConnection>();
            
            public Action<string, ushort, uint>? Ready { get; set; }
            public Action<string?, string, string, byte[], string?>? SessionDescription { get; set; }
            public Action<string, uint, int>? Speaking { get; set; }
            public Action? Disconnected { get; set; }

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
                lock (_stateLock)
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

                    if (_selfState.ChannelId == null)
                    {
                        _voiceConnection?.Disconnect();
                        Disconnected?.Invoke();
                        _voiceConnection = null;
                        _selfState = null;
                        _voiceServerConfig = null;
                    }
                    else if (_voiceServerConfig != null)
                    {
                        _ = ConnectToVoice();
                    }
                }
            }

            internal void UpdateVoiceState(JsonVoiceState state)
            {
                bool added = true;
                if (_stateDictionary.TryGetValue(state.UserId, out VoiceState voiceState))
                {
                    added = false;
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

                if (added && voiceState.Channel is not null)
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
                lock (_stateLock)
                {
                    _selfState = null;
                    _voiceConnection = null;
                }

                await _client.Gateway.VoiceStatusUpdateAsync(channelId, guildId);
            }

            internal async Task RequestJoinStream(ulong userId)
            {
                string key = _selfState!.ChannelId == _serverId ? 
                    $"call:{_serverId}:{userId}" :
                    $"guild:{_serverId}:{_selfState!.ChannelId}:{userId}";

                await _client.Gateway!.StreamWatchAsync(key);
            }
            
            internal void StreamCreate(StreamCreate streamCreate)
            {
                StreamConnections[streamCreate.StreamKey] = new StreamConnection(_client, streamCreate.RtcServerId, _selfState!.SessionId, _selfState!.UserId);
            }

            internal void StreamServerUpdate(StreamServerUpdate streamServerUpdate)
            {
                if (StreamConnections.TryGetValue(streamServerUpdate.StreamKey, out StreamConnection streamConnection))
                {
                    streamConnection.UpdateServer(streamServerUpdate);
                }
            }

            private async Task ConnectToVoice()
            {
                if (_voiceServerConfig == null || _selfState == null)
                {
                    return;
                }

                _voiceConnection = new VoiceConnection(
                    unhandledMessageEncountered: e => _client.LogException(ClientLogEvent.VoiceExceptionHandled, e),
                    knownOperationEncountered: e => _client.LogOperation(ClientLogEvent.KnownVoiceOperationEncountered, (int)e),
                    unhandledOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnhandledVoiceOperationEncountered, (int)e),
                    unknownOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnknownVoiceOperationEncountered, e),
                    voiceConnectionStatusChanged: _ => { },
                    ready: OnReady,
                    sessionDescription: OnSessionDescription,
                    speaking: OnSpeaking,
                    video: _ => {});

                _serverId = _voiceServerConfig.GuildId ?? _voiceServerConfig.ChannelId;
                
                await _voiceConnection.ConnectAsync(_voiceServerConfig.ConnectionUrl);
                await _voiceConnection.IdentifySelfToVoiceConnection(_serverId.GetValueOrDefault(), _selfState.SessionId, _voiceServerConfig.Token, _selfState.UserId, true, 
                    new VoiceIdentity.VoiceIdentityStream[]
                    {
                        new() { Quality = 100, Rid = "100", Type = "video" },
                        new() { Quality = 50, Rid = "50", Type = "video" }
                    });
            }

            public void SelectProtocol(string ip, ushort port)
            {
                Guard.IsNotNull(_voiceConnection, nameof(_voiceConnection));

                _ = _voiceConnection.SelectProtocol(ip, port);
            }

            public void SendSpeaking(bool speaking)
            {
                Guard.IsNotNull(_voiceConnection, nameof(_voiceConnection));

                _ = _voiceConnection.SendSpeaking(speaking);
            }                


            private void OnReady(VoiceReady ready)
            {
                Ready?.Invoke(ready.IP, (ushort)ready.Port, ready.SSRC);
                
                _ = _voiceConnection!.SendVideo(ready.SSRC, ready.Streams);
            }

            private void OnSessionDescription(VoiceSessionDescription session)
            {
                SessionDescription?.Invoke(session.AudioCodec, session.MediaSessionId, session.Mode, session.SecretKey.Select(x => (byte)x).ToArray(), session.VideoCodec);
            }

            private void OnSpeaking(Speaker speaking)
            {
                Speaking?.Invoke(speaking.UserId, speaking.SSRC, speaking.IsSpeaking);
            }
        }
    }
}
