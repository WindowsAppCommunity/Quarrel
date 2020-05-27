// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
{
    /// <summary>
    /// A WebSocket client.
    /// </summary>
    internal class WebSocketClient : IDisposable
    {
        private const int _receiveChunkSize = 16 * 1024; // 16KB
        private const int _sendChunkSize = 4 * 1024; // 4KB
        private const int _heartRateTimeout = -2147012894;

        private readonly SemaphoreSlim _lock;
        private readonly IDictionary<string, string> _headers;
        private readonly IWebProxy _proxy;
        private ClientWebSocket _client;
        private Task _task;
        private CancellationTokenSource _disconnectTokenSource;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private CancellationToken _parentToken;
        private bool _isDisposed;
        private bool _isDisconnecting;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketClient"/> class.
        /// </summary>
        /// <param name="proxy">The websocket proxy.</param>
        public WebSocketClient(IWebProxy proxy = null)
        {
            _lock = new SemaphoreSlim(1, 1);
            _disconnectTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;
            _headers = new ConcurrentDictionary<string, string>();
            _proxy = proxy;
        }

        /// <summary>
        /// Raised when binary recieved message.
        /// </summary>
        public event Action<byte[], int, int> BinaryMessage;

        /// <summary>
        /// Raised when text message recieved.
        /// </summary>
        public event Action<string> TextMessage;

        /// <summary>
        /// Raised when WebSocket closed.
        /// </summary>
        public event Action<Exception> Closed;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task ConnectAsync(string host)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync(host).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Disconnect from host.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task DisconnectAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Set a header.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value.</param>
        public void SetHeader(string key, string value)
        {
            _headers[key] = value;
        }

        /// <summary>
        /// Set the cancellation token.
        /// </summary>
        /// <param name="cancelToken">The new cancellation token.</param>
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _cancelTokenSource?.Dispose();

            _parentToken = cancelToken;
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;
        }

        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <param name="index">Starting index.</param>
        /// <param name="count">Count.</param>
        /// <param name="isText">Wheather or not the data is text.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SendAsync(byte[] data, int index, int count, bool isText)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_client == null)
                {
                    return;
                }

                int frameCount = (int)Math.Ceiling((double)count / _sendChunkSize);

                for (int i = 0; i < frameCount; i++, index += _sendChunkSize)
                {
                    bool isLast = i == (frameCount - 1);

                    int frameSize;
                    if (isLast)
                    {
                        frameSize = count - (i * _sendChunkSize);
                    }
                    else
                    {
                        frameSize = _sendChunkSize;
                    }

                    var type = isText ? WebSocketMessageType.Text : WebSocketMessageType.Binary;
                    await _client.SendAsync(new ArraySegment<byte>(data, index, count), type, isLast, _cancelToken).ConfigureAwait(false);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    DisconnectInternalAsync(true).GetAwaiter().GetResult();
                    _disconnectTokenSource?.Dispose();
                    _cancelTokenSource?.Dispose();
                    _lock?.Dispose();
                }

                _isDisposed = true;
            }
        }

        private async Task ConnectInternalAsync(string host)
        {
            await DisconnectInternalAsync().ConfigureAwait(false);

            _disconnectTokenSource?.Dispose();
            _cancelTokenSource?.Dispose();

            _disconnectTokenSource = new CancellationTokenSource();
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;

            _client?.Dispose();
            _client = new ClientWebSocket();
            _client.Options.Proxy = _proxy;
            _client.Options.KeepAliveInterval = TimeSpan.Zero;
            foreach (var header in _headers)
            {
                if (header.Value != null)
                {
                    _client.Options.SetRequestHeader(header.Key, header.Value);
                }
            }

            await _client.ConnectAsync(new Uri(host), _cancelToken).ConfigureAwait(false);
            _task = RunAsync(_cancelToken);
        }

        private async Task DisconnectInternalAsync(bool isDisposing = false)
        {
            try
            {
                _disconnectTokenSource.Cancel(false);
            }
            catch
            {
            }

            _isDisconnecting = true;

            if (_client != null)
            {
                if (!isDisposing)
                {
                    try
                    {
                        await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    }
                    catch
                    {
                    }
                }

                try
                {
                    _client.Dispose();
                }
                catch
                {
                }

                _client = null;
            }

            try
            {
                await (_task ?? Task.Delay(0)).ConfigureAwait(false);
                _task = null;
            }
            finally
            {
                _isDisconnecting = false;
            }
        }

        private async Task OnClosed(Exception ex)
        {
            if (_isDisconnecting)
            {
                return; // Ignore, this disconnect was requested.
            }

            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(false);
            }
            finally
            {
                _lock.Release();
            }

            Closed(ex);
        }

        private async Task RunAsync(CancellationToken cancelToken)
        {
            var buffer = new ArraySegment<byte>(new byte[_receiveChunkSize]);

            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult socketResult = await _client.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                    byte[] result;
                    int resultCount;

                    if (socketResult.MessageType == WebSocketMessageType.Close)
                    {
                        throw new WebSocketClosedException((int)socketResult.CloseStatus, socketResult.CloseStatusDescription);
                    }

                    if (!socketResult.EndOfMessage)
                    {
                        // This is a large message (likely just READY), lets create a temporary expandable stream
                        using (var stream = new MemoryStream())
                        {
                            stream.Write(buffer.Array, 0, socketResult.Count);
                            do
                            {
                                if (cancelToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                socketResult = await _client.ReceiveAsync(buffer, cancelToken).ConfigureAwait(false);
                                stream.Write(buffer.Array, 0, socketResult.Count);
                            }
                            while (socketResult == null || !socketResult.EndOfMessage);

                            // Use the internal buffer if we can get it
                            resultCount = (int)stream.Length;

                            result = stream.TryGetBuffer(out var streamBuffer) ? streamBuffer.Array : stream.ToArray();
                        }
                    }
                    else
                    {
                        // Small message
                        resultCount = socketResult.Count;
                        result = buffer.Array;
                    }

                    if (socketResult.MessageType == WebSocketMessageType.Text)
                    {
                        string text = Encoding.UTF8.GetString(result, 0, resultCount);
                        TextMessage(text);
                    }
                    else
                    {
                        BinaryMessage(result, 0, resultCount);
                    }
                }
            }
            catch (Win32Exception ex) when (ex.HResult == _heartRateTimeout)
            {
                var x = OnClosed(new Exception("Connection timed out.", ex));
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                // This cannot be awaited otherwise we'll deadlock when DiscordApiClient waits for this task to complete.
                var x = OnClosed(ex);
            }
        }
    }
}