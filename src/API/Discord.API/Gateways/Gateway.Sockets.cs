// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.JsonConverters;
using Discord.API.Sockets;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private readonly JsonSerializerOptions _serialiseOptions;
        private readonly JsonSerializerOptions _deserialiseOptions;
        private WebSocketClient _socket;
        private DeflateStream? _decompressor;
        private MemoryStream? _decompressionBuffer;

        private WebSocketClient CreateSocket()
        {
            _socket?.Dispose();
            _socket = new WebSocketClient();
            _socket.TextMessage += HandleTextMessage;
            _socket.BinaryMessage += HandleBinaryMessage;
            _socket.Closed += HandleClosed;
            return _socket;
        }

        private void SetupCompression()
        {
            _decompressionBuffer = new MemoryStream();
            _decompressor = new DeflateStream(_decompressionBuffer, CompressionMode.Decompress);
        }
        
        private async Task SendMessageAsync<T>(SocketFrame<T> frame)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, frame, _serialiseOptions);
            await SendMessageAsync(stream);
        }

        private async Task SendMessageAsync(MemoryStream stream)
        {
            try
            {
                await _socket.SendAsync(stream.GetBuffer(), 0, (int)stream.Length, true);
            }
            catch (WebSocketClosedException exception)
            {
                GatewayClosed?.Invoke(this, exception);
            }
        }

        private void HandleTextMessage(string message)
        {
            using var reader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(message)));
            HandleMessage(reader);
        }

        private void HandleBinaryMessage(byte[] bytes, int _, int count)
        {
            Guard.IsNotNull(_decompressor, nameof(_decompressor));
            Guard.IsNotNull(_decompressionBuffer, nameof(_decompressionBuffer));

            using var ms = new MemoryStream(bytes);
            ms.Position = 0;
            byte[] data = new byte[count];
            ms.Read(data, 0, count);
            int index = 0;
            using var decompressed = new MemoryStream();
            if (data[0] == 0x78)
            {
                _decompressionBuffer.Write(data, index + 2, count - 2);
                _decompressionBuffer.SetLength(count - 2);
            }
            else
            {
                _decompressionBuffer.Write(data, index, count);
                _decompressionBuffer.SetLength(count);
            }

            _decompressionBuffer.Position = 0;
            _decompressor.CopyTo(decompressed);
            _decompressionBuffer.Position = 0;
            decompressed.Position = 0;

            using var reader = new StreamReader(decompressed);
            HandleMessage(reader);
        }

        private async void HandleMessage(TextReader reader)
        {
            Stream stream = ((StreamReader)reader).BaseStream;
            SocketFrame? frame = await ParseFrame(stream);
            if (frame is null) return;

            if (frame.SequenceNumber.HasValue)
            {
                _lastEventSequenceNumber = frame.SequenceNumber.Value;
            }

            ProcessEvents(frame);
        }

        private void HandleClosed(Exception exception)
        {
            GatewayClosed?.Invoke(this, exception);
        }

        private async Task<SocketFrame?> ParseFrame(Stream stream)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<SocketFrame>(stream, _deserialiseOptions);
            }
            catch (SocketFrameException ex)
            {
                UnhandledMessageEncountered?.Invoke(this, ex);
            }
            catch
            {
                UnhandledMessageEncountered?.Invoke(this, null);
            }

            return null;
        }
    }
}
