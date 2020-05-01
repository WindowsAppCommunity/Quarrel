namespace DiscordAPI.Gateway
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
        RequestGuildMembers = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatAck = 11,
        SubscribeToGuild = 12,
        CallConnect = 13,
        UpdateGuildSubscriptions = 14,
        LobbyConnect = 15,
        LobbyDisconnect = 16,
        LobbyVoiceStatesUpdate = 17,
        GuildStreamCreate = 18,
        StreamDelete = 19,
        StreamWatch = 20,
        StreamPing = 21,
        StreamSetPaused = 22,
        FlushLfgSubscriptions = 23

    }

    public static class OperationCodeExtensions
    {
        public static int ToInt(this OperationCode opCode)
        {
            return (int)opCode;
        }
    }
}
