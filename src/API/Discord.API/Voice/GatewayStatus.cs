// Quarrel © 2022

namespace Discord.API.Sockets
{
    internal enum VoiceConnectionStatus
    {
        Initialized,
        Connecting,
        Connected,
        Disconnected,
        Resuming,
        Reconnecting,
        InvalidSession,
        Error,
    }
}
