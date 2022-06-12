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
    public class DiscordSocketClient<TFrame>
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private Task? _task;
        private DeflateStream? _decompressor;
        private MemoryStream? _decompressionBuffer;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private Action<TFrame> HandleMessage { get; }
        private Action<WebSocketCloseStatus?> HandleClose { get; }
        private Action<SocketFrameException> UnhandledMessageEncountered { get; }

        public CancellationToken Token => _tokenSource.Token;

        public DiscordSocketClient(JsonSerializerOptions serializeOptions, JsonSerializerOptions deserializeOptions, Action<TFrame> handleMessage, Action<WebSocketCloseStatus?> handleClose, Action<SocketFrameException> unhandledMessageEncountered)
        {
            _serializeOptions = serializeOptions;
            _deserializeOptions = deserializeOptions;

            HandleMessage = handleMessage;
            HandleClose = handleClose;
            UnhandledMessageEncountered = unhandledMessageEncountered;
        }
        
        public async Task CloseSocket(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
        {
            if (_socket is { State: WebSocketState.Open })
                await _socket.CloseAsync(status, string.Empty, CancellationToken.None);
            _tokenSource.Cancel();
            if (_task != null)
                await _task;
            _task = null;
        }

        public async Task SendMessageAsync<TPayload>(TPayload frame)
        {
            var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, frame, _serializeOptions, _tokenSource.Token);
            await SendMessageAsync(stream);
        }

        private async Task SendMessageAsync(MemoryStream stream)
        {
            await _socket.SendAsync(new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length), WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        public async Task ConnectAsync(string connectionUrl)
        {
            SetupCompression();
            
            await _socket.ConnectAsync(new Uri(connectionUrl), CancellationToken.None);

            _task = Task.Run(async () =>
            {
                await ListenOnSocket(_tokenSource.Token);
            });
        }

        private async Task ListenOnSocket(CancellationToken token)
        {
            var buffer = new ArraySegment<byte>(new byte[16 * 1024]);
            while (!token.IsCancellationRequested && _socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult socketResult = await _socket.ReceiveAsync(buffer, token).ConfigureAwait(false);
                if (socketResult.MessageType == WebSocketMessageType.Close)
                {
                    // call error handler socketResult.CloseStatus
                    HandleClose(socketResult.CloseStatus);
                    return;
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
                    _ = HandleTextMessage(bytes);
                }
                else
                {
                    HandleBinaryMessage(bytes, length);
                }
            }
        }

        private void SetupCompression()
        {
            _decompressionBuffer = new MemoryStream();
            _decompressor = new DeflateStream(_decompressionBuffer, CompressionMode.Decompress);
        }

        private async Task HandleTextMessage(byte[] buffer)
        {
            await HandleStream(new MemoryStream(buffer));
        }

        private async void HandleBinaryMessage(byte[] buffer, int count)
        {
            Guard.IsNotNull(_decompressor, nameof(_decompressor));
            Guard.IsNotNull(_decompressionBuffer, nameof(_decompressionBuffer));

            using var decompressed = new MemoryStream();

            if (buffer[0] == 0x78)
            {
                await _decompressionBuffer.WriteAsync(buffer, 2, count - 2, _tokenSource.Token);
                _decompressionBuffer.SetLength(count - 2);
            }
            else
            {
                await _decompressionBuffer.WriteAsync(buffer, 0, count, _tokenSource.Token);
                _decompressionBuffer.SetLength(count);
            }

            _decompressionBuffer.Position = 0;
            await _decompressor.CopyToAsync(decompressed);
            _decompressionBuffer.Position = 0;
            decompressed.Position = 0;

            await HandleStream(decompressed);
        }

        private async Task HandleStream(Stream stream)
        {
            try
            {
                TFrame? frame = await JsonSerializer.DeserializeAsync<TFrame>(stream, _deserializeOptions, _tokenSource.Token);
                if (frame is null) return;
                HandleMessage(frame);
            }
            catch (SocketFrameException ex)
            {
                UnhandledMessageEncountered(ex);
            }
            catch
            {
                UnhandledMessageEncountered(new SocketFrameException("Unknown Exception"));
            }
        }

    }
}
