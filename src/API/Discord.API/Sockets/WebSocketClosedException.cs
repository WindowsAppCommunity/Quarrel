// Adam Dernis © 2022

using System;

namespace Discord.API.Sockets
{
    public class WebSocketClosedException : Exception
    {
        public WebSocketClosedException(int closeCode, string reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : string.Empty)}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }

        public int CloseCode { get; }

        public string Reason { get; }
    }
}
