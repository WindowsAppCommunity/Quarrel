using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Messages
{
    public enum Status
    {
        Starting,
        Connecting,
        Connected,
        Disconnected,
        Failed,
        Offline
    }

    public class ConnectionStatusMessage
    {
        public ConnectionStatusMessage(Status status)
        {
            Status = status;
        }

        public Status Status;
    }
}
