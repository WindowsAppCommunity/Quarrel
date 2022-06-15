// Quarrel © 2022

namespace Discord.API.Voice
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
