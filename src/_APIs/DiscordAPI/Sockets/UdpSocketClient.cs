// Copyright (c) Quarrel. All rights reserved.
/*
 * Adapted From Discord.Net
 *
   The MIT License (MIT)

   Copyright (c) 2015-2017 Discord.Net Contributors

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAPI.Sockets
{
    /// <summary>
    /// A UDP Socket.
    /// </summary>
    internal class UdpSocketClient : IDisposable
    {
        private readonly SemaphoreSlim _lock;
        private UdpClient _udp;
        private IPEndPoint _destination;
        private CancellationTokenSource _stopCancelTokenSource;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private CancellationToken _parentToken;
        private Task _task;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpSocketClient"/> class.
        /// </summary>
        public UdpSocketClient()
        {
            _lock = new SemaphoreSlim(1, 1);
            _stopCancelTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Raised when data is recieved.
        /// </summary>
        public event Action<byte[], int, int> ReceivedDatagram;

        /// <summary>
        /// Gets the port of the UDP connection.
        /// </summary>
        public ushort Port => (ushort)((_udp?.Client.LocalEndPoint as IPEndPoint)?.Port ?? 0);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Start socket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task StartAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StartInternalAsync(_cancelToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Stop the socket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task StopAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StopInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Set destination.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        public void SetDestination(string ip, int port)
        {
            _destination = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        /// <summary>
        /// Set cancel token.
        /// </summary>
        /// <param name="cancelToken">New cancellation token.</param>
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _cancelTokenSource?.Dispose();

            _parentToken = cancelToken;
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _stopCancelTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;
        }

        /// <summary>
        /// Sends data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">Starting index.</param>
        /// <param name="count">Count of data to send.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SendAsync(byte[] data, int index, int count)
        {
            if (index != 0)
            {
                // Should never happen?
                var newData = new byte[count];
                Buffer.BlockCopy(data, index, newData, 0, count);
                data = newData;
            }

            await _udp.SendAsync(data, count, _destination).ConfigureAwait(false);
        }

        private async Task StartInternalAsync(CancellationToken cancelToken)
        {
            await StopInternalAsync().ConfigureAwait(false);

            _stopCancelTokenSource?.Dispose();
            _cancelTokenSource?.Dispose();

            _stopCancelTokenSource = new CancellationTokenSource();
            _cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _stopCancelTokenSource.Token);
            _cancelToken = _cancelTokenSource.Token;

            _udp?.Dispose();
            _udp = new UdpClient(0);

            _task = RunAsync(_cancelToken);
        }

        private async Task StopInternalAsync(bool isDisposing = false)
        {
            try
            {
                _stopCancelTokenSource.Cancel(false);
            }
            catch
            {
            }

            if (!isDisposing)
            {
                await (_task ?? Task.Delay(0)).ConfigureAwait(false);
            }

            if (_udp != null)
            {
                try
                {
                    _udp.Dispose();
                }
                catch
                {
                }

                _udp = null;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    StopInternalAsync(true).GetAwaiter().GetResult();
                    _stopCancelTokenSource?.Dispose();
                    _cancelTokenSource?.Dispose();
                    _lock?.Dispose();
                }

                _isDisposed = true;
            }
        }

        private async Task RunAsync(CancellationToken cancelToken)
        {
            var closeTask = Task.Delay(-1, cancelToken);
            while (!cancelToken.IsCancellationRequested)
            {
                var receiveTask = _udp.ReceiveAsync();

                _ = receiveTask.ContinueWith(
                    (receiveResult) =>
                {
                    // observe the exception as to not receive as unhandled exception
                    _ = receiveResult.Exception;
                }, TaskContinuationOptions.OnlyOnFaulted);

                var task = await Task.WhenAny(closeTask, receiveTask).ConfigureAwait(false);
                if (task == closeTask)
                {
                    break;
                }

                var result = receiveTask.Result;
                ReceivedDatagram(result.Buffer, 0, result.Buffer.Length);
            }
        }
    }
}