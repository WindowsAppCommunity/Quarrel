using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace DiscordAPI.Sockets
{
    public sealed class WebMessageSocket : IWebMessageSocket
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
        private readonly WebSocketClient _socket;

        public WebMessageSocket()
        {
            _socket = GetMessageWebSocket();
        }

        private WebSocketClient GetMessageWebSocket()
        {
            var socket = new WebSocketClient();

            socket.TextMessage += HandleMessage;
            socket.Closed += HandleClosed;

            return socket;
        }

        public async Task ConnectAsync(string connectionUrl)
        {
            try
            {
                await _socket.ConnectAsync(connectionUrl);
            }
            catch { }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(bytes, 0, bytes.Length, true);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        private void HandleMessage(string message)
        {
            OnMessageReceived(message);
        }
        public void ConvertToBase64(Stream stream)
        {
            Byte[] inArray = new Byte[(int)stream.Length];
            Char[] outArray = new Char[(int)(stream.Length * 1.34)];
            stream.Read(inArray, 0, (int)stream.Length);
            Debug.WriteLine("before= " + Convert.ToBase64String(inArray));
        }
        private void OnMessageReceived(string message)
        {
            var messageReceivedEvent = new MessageReceivedEventArgs
            {
                Message = message
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

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}
