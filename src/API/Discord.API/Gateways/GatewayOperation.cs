// Quarrel © 2022

namespace Discord.API.Gateways
{
    internal enum GatewayOperation : int
    {
        Dispatch = 0,
        Heartbeat = 1,
        Identify = 2,
        PresenceUpdate = 3,
        VoiceStateUpdate = 4,
        VoiceServerPing = 5,
        Resume = 6,
        Reconnect = 7,
        RequestGuildMembers = 8,
        InvalidSession = 9,
        Hello = 10,
        HeartbeatAck = 11,
        //[Obsolete("OP code 12 is deprecated in favor of OP code 14.")]
        //SubscribeToGuild = 12,
        CallConnect = 13,
        UpdateGuildSubscriptions = 14,
        LobbyConnect = 15,
        LobbyDisconnect = 16,
        LobbyVoiceStatesUpdate = 17,
        StreamCreate = 18,
        StreamDelete = 19,
        StreamWatch = 20,
        StreamPing = 21,
        StreamSetPaused = 22,
        //FlushLfgSubscriptions = 23,
        RequestGuildApplicationCommands = 24,
        EmbeddedActivityLaunch = 25,
        EmbeddedActivityClose = 26,
        EmbeddedActivityUpdate = 27,
        RequestForumUnreads = 28,
    }
}
