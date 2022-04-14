// Quarrel © 2022

using System;

namespace Discord.API.Sockets
{
    internal class WebSocketClosedException : Exception
    {
        public WebSocketClosedException(int closeCode, string? reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : string.Empty)}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }

        public int CloseCode { get; }

        /// <summary>
        /// The reason the websocket closed.
        /// </summary>
        public string? Reason { get; }
    }
}
