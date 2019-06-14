using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway
{
    public enum OperationCode : int
    {
        Dispatch = 0,
        Heartbeat = 1,
        Identify = 2,
        StatusUpdate = 3,
        VoiceStateUpdate = 4,
        VoiceServerPing = 5,
        Resume = 6,
        Reconnect = 7,
        RequestGuildMember = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatAck = 11,
        SubscribeToGuild = 12
    }

    public static class OperationCodeExtensions
    {
        public static int ToInt(this OperationCode opCode)
        {
            return (int)opCode;
        }
    }
}
