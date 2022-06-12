// Quarrel © 2022

namespace Discord.API.Sockets
{
    internal enum ConnectionStatus
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
