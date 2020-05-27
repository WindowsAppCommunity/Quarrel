// Copyright (c) Quarrel. All rights reserved.
/*
 * Adapted From Discord.Net
 *
   The MIT License (MIT)

   Copyright (c) 2015-2017 Discord.Net Contributors

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
 */

using System;

namespace DiscordAPI.Sockets
{
    /// <summary>
    ///     The exception that is thrown when the WebSocket session is closed by Discord.
    /// </summary>
    public class WebSocketClosedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketClosedException"/> class using a Discord close code
        /// and an optional reason.
        /// </summary>
        /// <param name="closeCode">The close code.</param>
        /// <param name="reason">The closure reason.</param>
        public WebSocketClosedException(int closeCode, string reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : string.Empty)}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }

        /// <summary>
        ///     Gets the close code sent by Discord.
        /// </summary>
        /// <returns>
        ///     A <see href="https://discordapp.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-close-event-codes">close code</see>
        ///     from Discord.
        /// </returns>
        public int CloseCode { get; }

        /// <summary>
        ///     Gets the reason of the interruption.
        /// </summary>
        public string Reason { get; }
    }
}