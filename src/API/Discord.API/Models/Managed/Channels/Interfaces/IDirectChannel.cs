// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IDirectChannel : IMessageChannel, IPrivateChannel
    {
        ulong RecipientId { get; }
    }
}
