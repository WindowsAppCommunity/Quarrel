// Adam Dernis © 2022

using System;

namespace Discord.API.Gateway
{
    internal class GatewayEventArgs<T> : EventArgs
    {
        public T EventData { get; }

        public GatewayEventArgs(T eventData)
        {
            EventData = eventData;
        }
    }
}
