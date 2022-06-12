// Quarrel © 2022

namespace Discord.API.Sockets
{
    internal enum GatewayStatus
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
