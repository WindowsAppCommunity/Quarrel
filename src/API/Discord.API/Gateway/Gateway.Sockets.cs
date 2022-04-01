// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Sockets;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal partial class Gateway
    {
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

        private async Task SendMessageAsync(SocketFrame frame, bool includeNulls = false)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            if (includeNulls)
            {
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            }

            try
            {
                string? json = JsonSerializer.Serialize(frame, options);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(bytes, 0, bytes.Length, true);
            }
            catch (WebSocketClosedException exception)
            {
                GatewayClosed?.Invoke(this, exception);
            }
        }

        private void HandleTextMessage(string message)
        {
            HandleMessage(new StringReader(message));
        }

        private void HandleBinaryMessage(byte[] bytes, int _, int count)
        {
            Guard.IsNotNull(_decompressor, nameof(_decompressor));
            Guard.IsNotNull(_decompressionBuffer, nameof(_decompressionBuffer));

            using (var ms = new MemoryStream(bytes))
            {
                ms.Position = 0;
                byte[] data = new byte[count];
                ms.Read(data, 0, count);
                int index = 0;
                using (var decompressed = new MemoryStream())
                {
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

                    HandleMessage(new StreamReader(decompressed));
                }
            }
        }

        private void HandleMessage(TextReader reader)
        {
            Stream stream = ((StreamReader)reader).BaseStream;
            SocketFrame? frame = JsonSerializer.Deserialize<SocketFrame>(stream);

            Guard.IsNotNull(frame, nameof(frame));

            if (frame.SequenceNumber.HasValue)
            {
                _lastEventSequenceNumber = frame.SequenceNumber.Value;
            }

            if (frame.Operation.HasValue && _operationHandlers.ContainsKey(frame.Operation.Value))
            {
                _operationHandlers[frame.Operation.Value](frame);
            }
            else if (frame.Type is not null && _eventHandlers.ContainsKey(frame.Type))
            {
                _eventHandlers[frame.Type](frame);
            }
            else
            {
                UnhandledMessageEncountered?.Invoke(this, frame);
            }
        }

        private void HandleClosed(Exception exception)
        {
            GatewayClosed?.Invoke(this, exception);
        }
    }
}
