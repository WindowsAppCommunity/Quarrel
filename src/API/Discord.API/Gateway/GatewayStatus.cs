// Adam Dernis © 2022

namespace Discord.API.Gateway
{
    public enum GatewayStatus
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
