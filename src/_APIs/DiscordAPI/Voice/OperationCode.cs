namespace DiscordAPI.Voice
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
        Signal = 10,
        IDK1 = 11,
        MediaSources = 12,
        ClientDisconnect = 13,
        IDK2 = 14
    }

    public static class OperationCodeExtensions
    {
        public static int ToInt(this OperationCode opCode)
        {
            return (int)opCode;
        }
    }
}
