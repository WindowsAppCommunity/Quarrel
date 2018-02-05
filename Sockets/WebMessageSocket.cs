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


namespace Discord_UWP.Sockets
{
    public class WebMessageSocket : IWebMessageSocket
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
        public bool UseCompression = false;
        private readonly MessageWebSocket _socket;
        private readonly DataWriter _dataWriter;

        public WebMessageSocket(bool compression)
        {
            UseCompression = compression;

            _socket = GetMessageWebSocket();
            _dataWriter = GetDataWriter();
        }

        private MessageWebSocket GetMessageWebSocket()
        {
            var socket = new MessageWebSocket();
            socket.Control.MessageType = SocketMessageType.Utf8;
            if (UseCompression)
            {
                _compressed = new MemoryStream();
                _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
            }

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
        private MemoryStream _compressed;
        private DeflateStream _decompressor;

        private async void HandleMessage(object sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            if (UseCompression)
            {
                using (var datastr = e.GetDataStream().AsStreamForRead())
                using (var ms = new MemoryStream())
                {
                    datastr.CopyTo(ms);
                    ms.Position = 0;
                    byte[] data = new byte[ms.Length];
                    ms.Read(data, 0, (int)ms.Length);
                    int index = 0;
                    int count = data.Length;
                    using (var decompressed = new MemoryStream())
                    {
                        if (data[0] == 0x78)
                        {
                            _compressed.Write(data, index + 2, count - 2);
                            _compressed.SetLength(count - 2);
                        }
                        else
                        {
                            _compressed.Write(data, index, count);
                            _compressed.SetLength(count);
                        }

                        _compressed.Position = 0;
                        _decompressor.CopyTo(decompressed);
                        _compressed.Position = 0;
                        decompressed.Position = 0;
                        using (var reader = new StreamReader(decompressed))
                            OnMessageReceived(reader.ReadToEnd());
                    }
                }
            }
            else
            {
                var dr = e.GetDataReader();
                OnMessageReceived(dr.ReadString(dr.UnconsumedBufferLength));
            }
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
