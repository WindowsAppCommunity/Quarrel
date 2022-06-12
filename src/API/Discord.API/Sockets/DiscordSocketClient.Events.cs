// Quarrel © 2022

using Discord.API.Exceptions;
using System;

namespace Discord.API.Sockets
{
    internal partial class DiscordSocketClient<TOperation, TEvent> 
        where TOperation : struct, Enum
        where TEvent : struct, Enum
    {
        protected Action<ConnectionStatus> ConnectionStatusChanged { get; }

        protected Action<SocketFrameException> UnhandledMessageEncountered { get; }

        protected Action<string> UnknownEventEncountered { get; }

        protected Action<int> UnknownOperationEncountered { get; }

        protected Action<string> KnownEventEncountered { get; }

        protected Action<TOperation> UnhandledOperationEncountered { get; }

        protected Action<TEvent> UnhandledEventEncountered { get; }
    }
}
