// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Channels;
using Discord.API.JsonConverters;
using Discord.API.Models.Json.Voice;
using Discord.API.Voice.Models;
using Discord.API.Voice.Models.Enums;
using Discord.API.Voice.Models.Handshake;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Webrtc;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private readonly JsonVoiceServerUpdate _voiceConfig;
        public readonly JsonVoiceState _state;
        private WebrtcManager _manager;

        private uint _ssrc;
        
        private string? _connectionUrl;

        private VoiceConnectionStatus _voiceConnectionStatus;

        private VoiceConnectionStatus VoiceConnectionStatus
        {
            get => _voiceConnectionStatus;
            set
            {
                _voiceConnectionStatus = value;
                VoiceConnectionStatusChanged(_voiceConnectionStatus);
            }
        }

        public VoiceConnection(
            JsonVoiceServerUpdate voiceConfig,
            JsonVoiceState state,
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<int> unknownOperationEncountered,
            Action<VoiceOperation> knownOperationEncountered,
            Action<VoiceOperation> unhandledOperationEncountered,
            Action<VoiceConnectionStatus> voiceConnectionStatusChanged,
            Action<VoiceReady> ready,
            Action<VoiceSessionDescription> sessionDescription,
            Action<Speaker> speaking)
        {
            UnhandledMessageEncountered = unhandledMessageEncountered;
            VoiceConnectionStatusChanged = voiceConnectionStatusChanged;
            KnownOperationEncountered = knownOperationEncountered;
            UnhandledOperationEncountered = unhandledOperationEncountered;
            UnknownOperationEncountered = unknownOperationEncountered;
            
            Ready = ready;
            SessionDescription = sessionDescription;
            Speaking = speaking;

            _voiceConfig = voiceConfig;
            _state = state;

            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.AddContext<JsonModelsContext>();

            _deserializeOptions = new JsonSerializerOptions { Converters = { new VoiceSocketFrameConverter() } };
            try
            {
                _manager = new WebrtcManager();
                _manager.IpAndPortObtained = OnIpAndPortObtained;
                _manager.Speaking = (bool speaking) => { _ = SendSpeaking(speaking); };
                _manager.AudioInData = (IList<float> data) => { };
                _manager.AudioOutData = (IList<float> data) => { };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async void OnIpAndPortObtained(string ip, ushort port)
        {
            await SelectProtocol(ip, port);
        }

        public async Task SendSpeaking(bool speaking)
        {
            var payload = new Speaking
            {
                Delay = 0,
                SSRC = _ssrc,
                IsSpeaking = speaking ? 1 : 0
            };

            await SendMessageAsync(VoiceOperation.Speaking, payload);
        }

        public void Connect(string ip, string port, uint src)
        {
            _manager.Connect(ip, port, src);
        }

        public void SetKey(byte[] array)
        {
            _manager.SetKey(array);
        }

        public void SetSpeaking(uint ssrc, int speaking)
        {
            _manager.SetSpeaking(ssrc, speaking);
        }
    }
}
