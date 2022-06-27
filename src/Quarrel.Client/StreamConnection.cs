// Quarrel © 2022

using Discord.API.Gateways.Models;
using Discord.API.Voice;
using Discord.API.Voice.Models;
using Discord.API.Voice.Models.Handshake;
using Quarrel.Client.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    internal class StreamConnection
    {
        private VoiceConnection _voiceConnection;
        private ulong _serverId;
        private string _sessionId;
        private string _token;
        private ulong _userId;
        private string _connectionUrl;

        private readonly QuarrelClient _client;

        public StreamConnection(QuarrelClient client, ulong serverId, string sessionId, ulong userId)
        {
            _client = client;
            _serverId = serverId;
            _sessionId = sessionId;
            _userId = userId;
        }
        
        internal async Task ConnectToStream()
        {
            _voiceConnection = new VoiceConnection(
                unhandledMessageEncountered: e => _client.LogException(ClientLogEvent.VoiceExceptionHandled, e),
                knownOperationEncountered: e => _client.LogOperation(ClientLogEvent.KnownVoiceOperationEncountered, (int)e),
                unhandledOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnhandledVoiceOperationEncountered, (int)e),
                unknownOperationEncountered: e => _client.LogOperation(ClientLogEvent.UnknownVoiceOperationEncountered, e),
                voiceConnectionStatusChanged: _ => { },
                ready: OnReady,
                sessionDescription: OnSessionDescription,
                speaking: OnSpeaking,
                video: OnVideo);

            await _voiceConnection.ConnectAsync(_connectionUrl);
            await _voiceConnection.IdentifySelfToVoiceConnection(_serverId, _sessionId, _token, _userId, true, new VoiceIdentity.VoiceIdentityStream[] { new() { Quality = 100, Rid = "100", Type = "video" }});
        }

        internal void UpdateServer(StreamServerUpdate serverUpdate)
        {
            _token = serverUpdate.Token;
            _connectionUrl = $"wss://{serverUpdate.Endpoint.Substring(0, serverUpdate.Endpoint.LastIndexOf(':'))}?v=7";
            _ = ConnectToStream();
        }


        private void OnReady(VoiceReady ready)
        {
            _voiceConnection!.Connect(ready.IP, ready.Port.ToString(), ready.SSRC);

            _ = _voiceConnection.SendVideo(ready.SSRC, ready.Streams);
        }

        private void OnSessionDescription(VoiceSessionDescription session)
        {
            _voiceConnection!.SetKey(session.SecretKey.Select(x => (byte)x).ToArray());
        }

        private void OnSpeaking(Speaker speaking)
        {
            _voiceConnection!.SetSpeaking(speaking.SSRC, speaking.IsSpeaking);
        }

        private void OnVideo(Video video)
        {
            _voiceConnection.CreateVideoStream(video.VideoSSRC);
        }
    }
}
