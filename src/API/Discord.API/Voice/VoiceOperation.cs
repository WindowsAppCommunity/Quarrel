// Quarrel © 2022

namespace Discord.API.Voice
{
    internal enum VoiceOperation : int
    {
        Identify = 0,
        SelectProtocol = 1,
        Ready = 2,
        Heartbeat = 3,
        SessionDescription = 4,
        Speaking = 5,
        HeartbeatAck = 6,
        Resume = 7,
        Hello = 8,
        Resumed = 9,
        Video = 12,
        ClientDisconnect = 13,
        SessionUpdate = 14,
        MediaSinkWants = 15,
        VoiceBackendVersion = 16,
    }
}
