// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public object Message { get; set; }
    }

    public class UDPSocket
    {
        public event EventHandler<PacketReceivedEventArgs> MessageReceived;

        UdpSocketClient _socket;

        public UDPSocket()
        {
            _socket = GetDatagramSocket();
        }

        private UdpSocketClient GetDatagramSocket()
        {
            var socket = new UdpSocketClient();
            socket.ReceivedDatagram += HandleMessage;
            return socket;
        }

        public async Task ConnectAsync(string connectionUrl, string port)
        {
            _socket.SetDestination(connectionUrl, int.Parse(port));
            await _socket.StartAsync();
        }

        public async Task SendBytesAsync(byte[] bytes)
        {
            await _socket.SendAsync(bytes, 0, bytes.Length);
        }

        public async Task SendDiscovery(uint ssrc)
        {
            var packet = new byte[70];
            packet[0] = (byte)(ssrc >> 24);
            packet[1] = (byte)(ssrc >> 16);
            packet[2] = (byte)(ssrc >> 8);
            packet[3] = (byte)(ssrc >> 0);
            await SendBytesAsync(packet);
        }

        private void HandleMessage(byte[] bytes, int index, int count)
        {
            try
            {
                OnMessageReceived(bytes);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private void OnMessageReceived(object message)
        {
            var messageReceivedEvent = new PacketReceivedEventArgs
            {
                Message = message
            };

            MessageReceived?.Invoke(this, messageReceivedEvent);
        }
    }
}
