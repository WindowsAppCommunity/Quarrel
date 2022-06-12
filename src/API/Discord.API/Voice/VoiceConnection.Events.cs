// Quarrel © 2022

using Discord.API.Gateways;
using System;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private static bool FireEvent<T>(GatewaySocketFrame frame, Action<T> eventHandler)
        {
            var eventArgs = ((GatewaySocketFrame<T>)frame).Payload;
            eventHandler(eventArgs);
            return true;
        }

        public static bool FireEvent<T>(T data, Action<T> eventHandler)
        {
            eventHandler(data);
            return true;
        }

        protected override void ProcessEvents(GatewaySocketFrame frame)
        {
            bool succeeded = frame switch
            {
                UnknownOperationGatewaySocketFrame osf => FireEvent(osf.Operation, UnknownOperationEncountered),
                UnknownEventGatewaySocketFrame osf => FireEvent(osf.Event, UnknownEventEncountered),
            };
        }
    }
}
