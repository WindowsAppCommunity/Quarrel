// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Sockets;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;
        private ClientWebSocket? _socket;
        private Task? _task;
        private CancellationTokenSource _tokenSource = new();
        private DeflateStream? _decompressor;
        private MemoryStream? _decompressionBuffer;
        

        private void SetupCompression()
        {
            _decompressionBuffer = new MemoryStream();
            _decompressor = new DeflateStream(_decompressionBuffer, CompressionMode.Decompress);
        }

        /// <summary>
        /// Sets up a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task Connect(string token)
        {
            _token = token;
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            GatewayStatus = GatewayStatus == GatewayStatus.Initialized ? GatewayStatus.Connecting : GatewayStatus.Reconnecting;
            await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "9", "&compress=zlib-stream"));
            _task = Task.Run(async () =>
            {
                await ListenOnSocket(_tokenSource.Token);
            });
        }

        /// <summary>
        /// Resumes a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task ResumeAsync()
        {
            GatewayStatus = GatewayStatus.Resuming;
            await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "9", "&compress=zlib-stream"));
            _task = Task.Run(async () =>
            {
                await ListenOnSocket(_tokenSource.Token);
            });
        }

        private async Task ListenOnSocket(CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[16 * 1024]);
            while (!token.IsCancellationRequested && _socket!.State == WebSocketState.Open)
            {
                WebSocketReceiveResult socketResult = await _socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
                if (socketResult.MessageType == WebSocketMessageType.Close)
                {
                    switch (socketResult.CloseStatus)
                    {
                        case (WebSocketCloseStatus)4000:
                        case (WebSocketCloseStatus)4001:
                        case (WebSocketCloseStatus)4002:
                        case (WebSocketCloseStatus)4003:
                        case (WebSocketCloseStatus)4005:
                        case (WebSocketCloseStatus)4007:
                        case (WebSocketCloseStatus)4008:
                        case (WebSocketCloseStatus)4009:
                            GatewayStatus = GatewayStatus.Reconnecting;
                            _socket = null;
                            _ = ConnectAsync();
                            return;

                        case (WebSocketCloseStatus)4004:
                        default:
                            GatewayStatus = GatewayStatus.Disconnected;
                            _socket = null;
                            return;

                    }
                }

                byte[] bytes = buffer.Array;
                int length = socketResult.Count;

                if (!socketResult.EndOfMessage)
                {
                    // This is a large message (likely just READY), lets create a temporary expandable stream
                    var stream = new MemoryStream();
                    await stream.WriteAsync(buffer.Array, 0, socketResult.Count, token).ConfigureAwait(false);
                    do
                    {
                        if (token.IsCancellationRequested)
                        {
                            _socket = null;
                            return;
                        }
                        socketResult = await _socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
                        await stream.WriteAsync(buffer.Array, 0, socketResult.Count, token).ConfigureAwait(false);
                    }
                    while (!socketResult.EndOfMessage);

                    bytes = stream.GetBuffer();
                    length = (int)stream.Length;
                }

                if (socketResult.MessageType == WebSocketMessageType.Text)
                {
                    HandleTextMessage(bytes);
                }
                else
                {
                    HandleBinaryMessage(bytes, length);
                }
            }
        }

        private async Task ReconnectAsync()
        {
            await CloseSocket();
            await ConnectAsync(_connectionUrl!);
        }
        private async Task ConnectAsync(string connectionUrl)
        {
            _connectionUrl = connectionUrl;
            SetupCompression();
            _tokenSource = new CancellationTokenSource();
            _socket = new ClientWebSocket();


            if (_socket.State is WebSocketState.Connecting or WebSocketState.Open)
            {
                throw new Exception("Tried to connect to socket while already connected");
            }
            await _socket.ConnectAsync(new Uri(connectionUrl), CancellationToken.None);
        }

        private async Task SendMessageAsync<T>(SocketFrame<T> frame)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, frame, _serializeOptions);
            await SendMessageAsync(stream);
        }

        private async Task SendMessageAsync(MemoryStream stream)
        {
            await _socket!.SendAsync(new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length), WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        private void HandleTextMessage(byte[] buffer)
        {
            HandleMessage(new MemoryStream(buffer));
        }

        private async void HandleBinaryMessage(byte[] buffer, int count)
        {
            Guard.IsNotNull(_decompressor, nameof(_decompressor));
            Guard.IsNotNull(_decompressionBuffer, nameof(_decompressionBuffer));
            
            using var decompressed = new MemoryStream();
            
            if (buffer[0] == 0x78)
            {
                await _decompressionBuffer.WriteAsync(buffer, 2, count - 2);
                _decompressionBuffer.SetLength(count - 2);
            }
            else
            {
                await _decompressionBuffer.WriteAsync(buffer, 0, count);
                _decompressionBuffer.SetLength(count);
            }

            _decompressionBuffer.Position = 0;
            await _decompressor.CopyToAsync(decompressed);
            _decompressionBuffer.Position = 0;
            decompressed.Position = 0;
            
            HandleMessage(decompressed);
        }

        private async void HandleMessage(Stream stream)
        {
            SocketFrame? frame = await ParseFrame(stream);
            if (frame is null) return;

            if (frame.SequenceNumber.HasValue)
            {
                _lastEventSequenceNumber = frame.SequenceNumber.Value;
            }

            ProcessEvents(frame);
        }

        private async Task CloseSocket()
        {
            if(_socket is { State: WebSocketState.Open })
                await _socket.CloseAsync((WebSocketCloseStatus)4000, string.Empty, CancellationToken.None);
            _tokenSource.Cancel();
            if(_task != null)
                await _task;
            _task = null;
        }

        private async Task<SocketFrame?> ParseFrame(Stream stream)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<SocketFrame>(stream, _deserializeOptions);
            }
            catch (SocketFrameException ex)
            {
                UnhandledMessageEncountered(ex);
            }
            catch
            {
                UnhandledMessageEncountered(new SocketFrameException("Unknown Exception"));
            }

            return null;
        }
    }
}
