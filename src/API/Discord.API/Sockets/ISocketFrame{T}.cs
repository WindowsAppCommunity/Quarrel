// Quarrel © 2022

namespace Discord.API.Sockets
{
    internal interface ISocketFrame<T, TOperation, TEvent> : ISocketFrame<TOperation, TEvent>
    {
        T Payload { get; set; }
    }
}
