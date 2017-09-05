using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public enum OperationCode : int
    {
        Identify = 0,
        SelectProtocol = 1,
        Ready = 2,
        Heartbeat = 3,
        SessionDescription = 4,
        Speaking = 5,
        HeartbeatACK = 6,
        Resume = 7,
        Hello = 8,
        Resumed = 9,
        ClientDisconnect = 10
    }

    public static class OperationCodeExtensions
    {
        public static int ToInt(this OperationCode opCode)
        {
            return (int)opCode;
        }
    }
}
