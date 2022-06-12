// Quarrel © 2022

using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Channels;
using Discord.API.Sockets;
using System;
using System.Text.Json;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection : DiscordSocketClient<VoiceOperation, VoiceEvent>
    {
        public VoiceConnection(
            VoiceServerUpdate config,
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<string> unknownEventEncountered,
            Action<int> unknownOperationEncountered,
            Action<string> knownEventEncountered,
            Action<VoiceOperation> unhandledOperationEncountered,
            Action<VoiceEvent> unhandledEventEncountered,
            Action<ConnectionStatus> connectionStatusChanged) :
            base(connectionStatusChanged,
                unhandledMessageEncountered,
                unknownEventEncountered,
                unknownOperationEncountered,
                knownEventEncountered,
                unhandledOperationEncountered,
                unhandledEventEncountered)
        {
        }

        protected override JsonSerializerOptions DeserializeOptions { get; }
    }
}
