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

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
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
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<int> unknownOperationEncountered,
            Action<VoiceOperation> knownOperationEncountered,
            Action<VoiceOperation> unhandledOperationEncountered,
            Action<VoiceConnectionStatus> voiceConnectionStatusChanged,
            Action<VoiceReady> ready,
            Action<VoiceSessionDescription> sessionDescription,
            Action<Speaker> speaking,
            Action<Video> video)
        {
            UnhandledMessageEncountered = unhandledMessageEncountered;
            VoiceConnectionStatusChanged = voiceConnectionStatusChanged;
            KnownOperationEncountered = knownOperationEncountered;
            UnhandledOperationEncountered = unhandledOperationEncountered;
            UnknownOperationEncountered = unknownOperationEncountered;
            
            Ready = ready;
            SessionDescription = sessionDescription;
            Speaking = speaking;
            Video = video;

            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.AddContext<JsonModelsContext>();

            _deserializeOptions = new JsonSerializerOptions { Converters = { new VoiceSocketFrameConverter() } };
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
    }
}
