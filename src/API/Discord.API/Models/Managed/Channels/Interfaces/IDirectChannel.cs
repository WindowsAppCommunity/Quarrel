// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IDirectChannel : IMessageChannel, IPrivateChannel
    {
        ulong RecipientId { get; }
    }
}
