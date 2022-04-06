// Adam Dernis © 2022

namespace Discord.API.Gateways
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
