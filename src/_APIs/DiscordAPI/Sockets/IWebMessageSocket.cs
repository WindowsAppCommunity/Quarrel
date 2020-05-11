// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ConnectionClosedEventArgs : EventArgs
    {

    }

    public interface IWebMessageSocket : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
        Task ConnectAsync(string connectionUrl);
        Task SendMessageAsync(string message);
    }
}
