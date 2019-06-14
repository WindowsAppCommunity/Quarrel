using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Wire;

namespace NamedPipeWrapper
{
    public class NamedPipeClient : NamedPipeBase
    {
        private const int MaxReconnectAttempts = 20;

        private readonly string _pipeName;
        private readonly int _timeout;

        private NamedPipeClientStream _client;
        private CancellationTokenSource _aggreagateToken;

        public NamedPipeClient(string pipeName, TimeSpan timeout)
        {
            _timeout = (int)timeout.TotalMilliseconds;
            _pipeName = pipeName;
        }

        public bool IsConnected { get; set; }

        public Task<bool> ConnectAsync(CancellationToken ct = default(CancellationToken))
        {
            _aggreagateToken = CombineWithInternalToken(ct);
            return ConnectAsyncInternal();
        }

        private async Task<bool> ConnectAsyncInternal()
        {
            CheckDisposed();

            IsConnected = false;

            Logger.Info($"Trying to connect to {_pipeName}");

            _client = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            try
            {
                await _client.ConnectAsync(_timeout, _aggreagateToken.Token);
                _client.ReadMode = PipeTransmissionMode.Message;
            }
            catch (OperationCanceledException e)
            {
                OnPipeDied(_client, true);
                return false;
            }
            catch (Exception e)
            {
                Logger.Warn($"Failed to connect to {_pipeName} ({e.Message})", e);
                _client = null;
                return false;
            }

            Logger.Info($"Connected to {_pipeName}");

            IsConnected = true;

            ReadMessagesAsync(_client, _aggreagateToken.Token).HandleException(ex => { }); // All exceptions are handled internally.

            return true;
        }

        protected override void OnPipeDied(PipeStream stream, bool isCancelled)
        {
            IsConnected = false;

            base.OnPipeDied(stream, isCancelled);

            if (!isCancelled)
            {
                ReconnectAsync().HandleException(ex => {}); // All exceptions are handled internally.
            }
        }

        private async Task ReconnectAsync()
        {
            var interval = TimeSpan.FromSeconds(2);
            int attemptCount = 0;

            while (attemptCount < MaxReconnectAttempts)
            {
                Logger.Info($"Will attempt to reconnect in {interval.TotalSeconds} s.");
                await Task.Delay(interval, _aggreagateToken.Token);
                if (await ConnectAsyncInternal())
                {
                    return;
                }

                interval = TimeSpan.FromSeconds(interval.TotalSeconds * 2);
                attemptCount++;
            }

            Logger.Error($"Failed to reconnect to {_pipeName} in {MaxReconnectAttempts} attempts. Disposing pipe.");
            Dispose();
        }

        public Task<bool> SendAsync<T>(T message, CancellationToken ct = default(CancellationToken))
        {
            CheckDisposed();

            if (!IsConnected)
            {
                return Task.FromResult(false);
            }

            Logger.Debug("Sending message.");

            return SendAsync(_client, message.Serialize(), ct);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _client?.Dispose();
            }
        }
    }
}