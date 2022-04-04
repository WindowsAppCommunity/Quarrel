﻿// Adam Dernis © 2022

namespace Discord.API.Gateways
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