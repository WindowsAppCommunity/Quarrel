// Quarrel © 2022

namespace Discord.API.Sockets
{
    internal interface ISocketFrame<TOperation, TEvent>
    {
        TOperation Operation { get; set; }

        int? SequenceNumber { get; set; }

        TEvent? Event { get; set; }
    }
}
