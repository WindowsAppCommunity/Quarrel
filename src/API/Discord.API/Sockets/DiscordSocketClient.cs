// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways;
using Discord.API.JsonConverters;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.Sockets
{
    internal abstract partial class DiscordSocketClient<TFrame, TOperation, TEvent>
        where TFrame : class, ISocketFrame<TOperation, TEvent>
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private ClientWebSocket? _socket;
        private Task? _task;
        private DeflateStream? _decompressor;
        private MemoryStream? _decompressionBuffer;
        private CancellationTokenSource _tokenSource = new();

        private string? _connectionUrl;

        private ConnectionStatus _connectionStatus;

        protected DiscordSocketClient(
            Action<ConnectionStatus> connectionStatusChanged,
            Action<SocketFrameException> unhandledMessageEncountered,
            Action<string> unknownEventEncountered,
            Action<int> unknownOperationEncountered,
            Action<string> knownEventEncountered,
            Action<TOperation> unhandledOperationEncountered,
            Action<TEvent> unhandledEventEncountered)
        {
            ConnectionStatusChanged = connectionStatusChanged;
            UnhandledEventEncountered = unhandledEventEncountered;
            UnhandledOperationEncountered = unhandledOperationEncountered;
            KnownEventEncountered = knownEventEncountered;
            UnknownOperationEncountered = unknownOperationEncountered;
            UnknownEventEncountered = unknownEventEncountered;
            UnhandledMessageEncountered = unhandledMessageEncountered;

            _connectionStatus = ConnectionStatus.Initialized;

            _serializeOptions = new JsonSerializerOptions();
            _serializeOptions.AddContext<JsonModelsContext>();
        }

        protected ConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                ConnectionStatusChanged(_connectionStatus);
            }
        }

        protected abstract JsonSerializerOptions DeserializeOptions { get; }

        protected async Task ConnectAsync(string connectionUrl)
        {
            var connectionStatus = ConnectionStatus == ConnectionStatus.Initialized
                ? ConnectionStatus.Connecting
                : ConnectionStatus.Reconnecting;

            await ConnectAsync(connectionUrl, connectionStatus);
        }

        protected async Task ResumeAsync()
        {
            await ConnectAsync(_connectionUrl!, ConnectionStatus.Resuming);
        }

        protected async Task ReconnectAsync()
        {
            await CloseSocket();
            await ConnectAsync(_connectionUrl!);
        }

        protected async Task CloseSocket()
        {
            if (_socket is { State: WebSocketState.Open })
                await _socket.CloseAsync((WebSocketCloseStatus)4000, string.Empty, CancellationToken.None);
            _tokenSource.Cancel();
            if (_task != null)
                await _task;
            _task = null;
        }

        protected async Task SendMessageAsync<TPayloadFrame>(TPayloadFrame frame)
            where TPayloadFrame : ISocketFrame<TOperation, TEvent>
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, frame, _serializeOptions);
            await SendMessageAsync(stream);
        }

        private async Task SendMessageAsync(MemoryStream stream)
        {
            await _socket!.SendAsync(new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length), WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        protected abstract void ProcessEvents(TFrame frame);

        private async Task ConnectAsync(string connectionUrl, ConnectionStatus connectionStatus)
        {
            ConnectionStatus = connectionStatus;
            _connectionUrl = connectionUrl;
            SetupCompression();
            _tokenSource = new CancellationTokenSource();
            _socket = new ClientWebSocket();

            if (_socket.State is WebSocketState.Connecting or WebSocketState.Open)
            {
                throw new Exception("Tried to connect to socket while already connected");
            }
            await _socket.ConnectAsync(new Uri(_connectionUrl), CancellationToken.None);

            _task = Task.Run(async () =>
            {
                await ListenOnSocket(_tokenSource.Token);
            });
        }

        private void SetupCompression()
        {
            _decompressionBuffer = new MemoryStream();
            _decompressor = new DeflateStream(_decompressionBuffer, CompressionMode.Decompress);
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
                            ConnectionStatus = ConnectionStatus.Reconnecting;
                            _socket = null;
                            _ = ConnectAsync(_connectionUrl!);
                            return;

                        case (WebSocketCloseStatus)4004:
                        default:
                            ConnectionStatus = ConnectionStatus.Disconnected;
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
            TFrame? frame = await ParseFrame(stream);
            if (frame is null) return;

            if (frame.SequenceNumber.HasValue)
            {
                LastEventSequenceNumber = frame.SequenceNumber.Value;
            }

            ProcessEvents(frame);
        }

        private async Task<TFrame?> ParseFrame(Stream stream)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<TFrame>(stream, DeserializeOptions);
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
