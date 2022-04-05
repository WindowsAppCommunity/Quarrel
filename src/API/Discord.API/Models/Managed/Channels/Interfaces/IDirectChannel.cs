// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for direct message channels.
    /// </summary>
    internal interface IDirectChannel : IPrivateChannel, IMessageChannel, IAudioChannel
    {
        ulong RecipientId { get; }
    }
}
