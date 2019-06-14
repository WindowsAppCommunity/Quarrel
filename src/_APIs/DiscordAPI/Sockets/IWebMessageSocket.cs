using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace Quarrel.Sockets
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ConnectionClosedEventArgs : EventArgs
    {
       
    }

    public interface IWebMessageSocket : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;
        Task ConnectAsync(string connectionUrl);
        Task SendMessageAsync(string message);
    }
}
