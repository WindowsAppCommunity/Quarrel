using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public interface IUdpSocket
    {
        event Func<byte[], int, int, Task> ReceivedDatagram;

        ushort Port { get; }

        void SetCancelToken(CancellationToken cancelToken);
        void SetDestination(string ip, int port);

        Task StartAsync();
        Task StopAsync();

        Task SendAsync(byte[] data, int index, int count);
    }
}
