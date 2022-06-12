// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Channels;
using Discord.API.Models.Json.Voice;
using Discord.API.Sockets;
using System;
using System.Text.Json;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection : DiscordSocketClient<VoiceSocketFrame, VoiceOperation, VoiceEvent?>
    {
        private readonly VoiceServerUpdate _voiceConfig;
        public readonly JsonVoiceState _state;

        private uint _ssrc;

        public VoiceConnection(
            VoiceServerUpdate voiceConfig,
            JsonVoiceState state,
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<string> unknownEventEncountered,
            Action<int> unknownOperationEncountered,
            Action<string> knownEventEncountered,
            Action<VoiceOperation> unhandledOperationEncountered,
            Action<VoiceEvent?> unhandledEventEncountered,
            Action<ConnectionStatus> connectionStatusChanged) :
            base(connectionStatusChanged,
                unhandledMessageEncountered,
                unknownEventEncountered,
                unknownOperationEncountered,
                knownEventEncountered,
                unhandledOperationEncountered,
                unhandledEventEncountered)
        {
            _voiceConfig = voiceConfig;
            _state = state;
        }

        protected override JsonSerializerOptions DeserializeOptions { get; }
    }
}
