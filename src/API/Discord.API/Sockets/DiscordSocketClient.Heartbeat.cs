// Quarrel © 2022

using Discord.API.Gateways;
using System.Threading.Tasks;

namespace Discord.API.Sockets
{
    internal partial class DiscordSocketClient<TOpCode, TEvent>
    {
        private bool _receivedAck;

        public int LastEventSequenceNumber { get; private set; }

        protected bool OnHeartbeatAck()
        {
            _receivedAck = true;
            return true;
        }

        protected async Task BeginHeartbeatAsync(int interval)
        {
            while (ConnectionStatus == ConnectionStatus.Connected)
            {
                await SendHeartbeatAsync();
                _receivedAck = false;
                await Task.Delay(interval);
                if (!_receivedAck)
                {
                    ConnectionStatus = ConnectionStatus.Disconnected;
                    await CloseSocket();
                    await ResumeAsync();
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var frame = new SocketFrame<int>()
                {
                    Operation = GatewayOperation.Heartbeat,
                    Payload = LastEventSequenceNumber
                };

                await SendMessageAsync(frame);
            }
            catch
            {
            }
        }
    }
}
