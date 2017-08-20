using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Discord_UWP.DiscordAPI.Sockets
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class UDPSocket
    {
        public event EventHandler<PacketReceivedEventArgs> MessageReceived;

        DatagramSocket _socket;
        DataWriter _dataWriter;

        public UDPSocket()
        {
            _socket = GetDatagramSocket();
            _dataWriter = GetDataWriter();
        }

        private DatagramSocket GetDatagramSocket()
        {
            var socket = new DatagramSocket();
            socket.MessageReceived += HandleMessage;
            return socket;
        }

        private DataWriter GetDataWriter()
        {
            return new DataWriter(_socket.OutputStream);
        }

        public async Task ConnectAsync(string connectionUrl, string port)
        {
            await _socket.ConnectAsync(new Windows.Networking.HostName(connectionUrl), port);
        }

        public async Task SendMessageAsync(string message)
        {
            _dataWriter.WriteString(message);
            await _dataWriter.StoreAsync();
        }

        private void HandleMessage(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs e)
        {
            using (var dataReader = e.GetDataReader())
            {
                string messageString = dataReader.ReadString(dataReader.UnconsumedBufferLength); //TODO: Don't recieve sound as a string!!!
                OnMessageReceived(messageString);
            }
        }

        private void OnMessageReceived(string message)
        {
            var messageReceivedEvent = new PacketReceivedEventArgs
            {
                Message = message
            };

            MessageReceived?.Invoke(this, messageReceivedEvent);
        }
    }
}
