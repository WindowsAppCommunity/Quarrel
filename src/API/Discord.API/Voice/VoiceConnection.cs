// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Channels;
using Discord.API.JsonConverters;
using Discord.API.Models.Json.Voice;
using Discord.API.Voice.Models;
using Discord.API.Voice.Models.Enums;
using Discord.API.Voice.Models.Handshake;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private readonly JsonVoiceServerUpdate _voiceConfig;
        public readonly JsonVoiceState _state;

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
            Action<VoiceReady> ready)
        {
            UnhandledMessageEncountered = unhandledMessageEncountered;
            VoiceConnectionStatusChanged = voiceConnectionStatusChanged;
            KnownOperationEncountered = knownOperationEncountered;
            UnhandledOperationEncountered = unhandledOperationEncountered;
            UnknownOperationEncountered = unknownOperationEncountered;
            
            Ready = ready;

            _voiceConfig = voiceConfig;
            _state = state;

            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.AddContext<JsonModelsContext>();

            _deserializeOptions = new JsonSerializerOptions { Converters = { new VoiceSocketFrameConverter() } };
        }

        public async Task SendSpeaking(SpeakingState state)
        {
            var payload = new Speaking
            {
                State = state,
                Delay = 0,
                SSRC = _ssrc,
            };

            await SendMessageAsync(VoiceOperation.Speaking, payload);
        }
    }
}
