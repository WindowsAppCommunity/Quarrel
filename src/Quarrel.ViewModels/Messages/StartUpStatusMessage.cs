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
        Failed,
        Offline
    }

    public class StartUpStatusMessage
    {
        public StartUpStatusMessage(Status status)
        {
            Status = status;
        }

        public Status Status;
    }
}
