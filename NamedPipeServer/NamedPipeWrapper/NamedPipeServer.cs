using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace NamedPipeWrapper
{
    public class NamedPipeServer : NamedPipeBase
    {
        private readonly ConcurrentDictionary<NamedPipeServerStream, object> _streams = new ConcurrentDictionary<NamedPipeServerStream, object>();
        
        public NamedPipeServer(string pipeName)
        {
            PipeName = pipeName;
        }

        public string PipeName { get; }
        
        public event EventHandler<ClientConnectedArgs> ClientConnected;

        public event EventHandler<ClientConnectedArgs> ClientDisconnected;

        public void StartListen()
        {
            if (_streams.Count > 0)
            {
                Logger.Info("Already listening.");
                return;
            }

            ListenForConnectionsAsync().HandleException(ex => {});
        }

        public Task SendMessageToAllAsync<T>(T message, CancellationToken ct = default(CancellationToken))
        {
            CheckDisposed();

            Logger.Debug("Sending message to all clients.");

            return SendToAllAsync(message.Serialize(), ct);
        }

        public async Task SendToAllAsync(byte[] message, CancellationToken ct = default(CancellationToken))
        {
            CheckDisposed();

            foreach (var server in _streams.Keys.ToList())
            {
                if (!server.CanWrite)
                    continue;

                await SendAsync(server, message, ct);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var stream in _streams.Keys.ToList())
                {
                    stream.Dispose();
                }

                _streams.Clear();
            }
        }

        protected override void OnPipeDied(PipeStream stream, bool isCancelled)
        {
            base.OnPipeDied(stream, isCancelled);
            RemoveServer((NamedPipeServerStream)stream);
        }

        private void OnClientConnected(NamedPipeServer server)
        {
            ClientConnected?.Invoke(this, new ClientConnectedArgs(server));
        }

        private void OnClientDisconnected(NamedPipeServer server)
        {
            ClientDisconnected?.Invoke(this, new ClientConnectedArgs(server));
        }
        
        private void AddServer(NamedPipeServerStream server)
        {
            _streams.AddOrUpdate(server, new object(), (stream, o) => new object());
        }

        private void RemoveServer(NamedPipeServerStream server)
        {
            if (_streams.Remove(server))
            {
                OnClientDisconnected(this);
            }
        }

        private async Task ListenForConnectionsAsync()
        {
            CheckDisposed();

            Logger.Info($"Starting to listen to connections on pipe {PipeName}");

            while (!CancellationTokenSource.Token.IsCancellationRequested)
            {
                var server = GetServer();

                try
                {
                    await server.WaitForConnectionAsync(CancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    OnPipeDied(server, true);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Unhandled stream exception: {ex.Message}. Disposing the pipe.", ex);
                    Dispose();
                    return;
                }

                AddServer(server);

                if (server.CanRead)
                    ReadMessagesAsync(server, CancellationTokenSource.Token).HandleException(ex => { });

                OnClientConnected(this);
            }
        }

        private NamedPipeServerStream GetServer()
        {
            return new NamedPipeServerStream(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
    }
}