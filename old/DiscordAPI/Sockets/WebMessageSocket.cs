using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace Quarrel.Sockets
{
    public class WebMessageSocket : IWebMessageSocket
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
        private readonly MessageWebSocket _socket;
        private readonly DataWriter _dataWriter;

        public WebMessageSocket()
        {
            _socket = GetMessageWebSocket();
            _dataWriter = GetDataWriter();
        }

        private MessageWebSocket GetMessageWebSocket()
        {
            var socket = new MessageWebSocket();
            socket.Control.MessageType = SocketMessageType.Utf8;

            socket.MessageReceived += HandleMessage;
            socket.Closed += HandleClosed;
           
            return socket;
        }

        private DataWriter GetDataWriter()
        {
            return new DataWriter(_socket.OutputStream);
        }

        public async Task ConnectAsync(string connectionUrl)
        {
            try
            {
                await _socket.ConnectAsync(new Uri(connectionUrl));
            }
            catch { }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                _dataWriter.WriteString(message);
                await _dataWriter.StoreAsync();
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        private void HandleMessage(object sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            var dr = e.GetDataReader();
            OnMessageReceived(dr.ReadString(dr.UnconsumedBufferLength));
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

        private void HandleClosed(object sender, WebSocketClosedEventArgs args)
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
            _dataWriter.Dispose();
        }
    }
}
