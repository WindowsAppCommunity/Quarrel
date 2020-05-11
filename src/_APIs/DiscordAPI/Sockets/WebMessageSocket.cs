// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
{
    /// <summary>
    /// A WebSocket wrapper.
    /// </summary>
    public sealed class WebMessageSocket : IWebMessageSocket
    {
        private readonly WebSocketClient _socket;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebMessageSocket"/> class.
        /// </summary>
        public WebMessageSocket()
        {
            _socket = GetMessageWebSocket();
        }

        /// <summary>
        /// Raised when message recieved.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Raised when connection closed.
        /// </summary>
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;

        /// <summary>
        /// Connect to socket.
        /// </summary>
        /// <param name="connectionUrl">Connection url.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ConnectAsync(string connectionUrl)
        {
            try
            {
                await _socket.ConnectAsync(connectionUrl);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends a message over socket.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SendMessageAsync(string message)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(bytes, 0, bytes.Length, true);
            }
            catch
            {
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _socket.Dispose();
        }

        private WebSocketClient GetMessageWebSocket()
        {
            var socket = new WebSocketClient();

            socket.TextMessage += HandleMessage;
            socket.Closed += HandleClosed;

            return socket;
        }

        private void ConvertToBase64(Stream stream)
        {
            byte[] inArray = new byte[(int)stream.Length];
            char[] outArray = new char[(int)(stream.Length * 1.34)];
            stream.Read(inArray, 0, (int)stream.Length);
            Debug.WriteLine("before= " + Convert.ToBase64String(inArray));
        }

        private void HandleMessage(string message)
        {
            OnMessageReceived(message);
        }

        private void OnMessageReceived(string message)
        {
            var messageReceivedEvent = new MessageReceivedEventArgs
            {
                Message = message,
            };

            MessageReceived?.Invoke(this, messageReceivedEvent);
        }

        private void HandleClosed(Exception exception)
        {
            OnClosed();
        }

        private void OnClosed()
        {
            var connectionClosedEvent = new ConnectionClosedEventArgs();

            ConnectionClosed?.Invoke(this, connectionClosedEvent);
        }
    }
}
