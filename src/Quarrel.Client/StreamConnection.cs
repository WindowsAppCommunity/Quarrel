// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models;
using Discord.API.Voice;
using Discord.API.Voice.Models;
using Discord.API.Voice.Models.Handshake;
using Quarrel.Client.Logger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.Client
{
    public class StreamConnection
    {
        private VoiceConnection _voiceConnection;
        private ulong _serverId;
        private string _sessionId;
        private string _token;
        private ulong _userId;
        private string _connectionUrl;

        private readonly QuarrelClient _client;
        public Action<string, ushort, uint>? Ready { get; set; }
        public Action<string?, string, string, byte[], string?>? SessionDescription { get; set; }
        public Action<string, uint, int>? Speaking { get; set; }
        public Action<ulong, uint>? Video { get; set; }
        public Action? Disconnected { get; set; }

        internal StreamConnection(QuarrelClient client, ulong serverId, string sessionId, ulong userId)
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

        internal void UpdateServer(StreamServerUpdate serverUpdate)
        {
            _token = serverUpdate.Token;
            _connectionUrl = $"wss://{serverUpdate.Endpoint.Substring(0, serverUpdate.Endpoint.LastIndexOf(':'))}?v=7";
            _ = ConnectToStream();
        }

        private void OnReady(VoiceReady ready)
        {
            Ready?.Invoke(ready.IP, (ushort)ready.Port, ready.SSRC);
        }

        private void OnSessionDescription(VoiceSessionDescription session)
        {
            SessionDescription?.Invoke(session.AudioCodec, session.MediaSessionId, session.Mode, session.SecretKey.Select(x => (byte)x).ToArray(), session.VideoCodec);
        }

        private void OnSpeaking(Speaker speaking)
        {
            Speaking?.Invoke(speaking.UserId, speaking.SSRC, speaking.IsSpeaking);
        }

        private void OnVideo(Video video)
        {
            Video?.Invoke(video.UserId, video.VideoSSRC);
        }
    }
}
