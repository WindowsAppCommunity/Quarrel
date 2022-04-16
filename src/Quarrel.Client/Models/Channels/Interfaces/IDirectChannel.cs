// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for direct message channels.
    /// </summary>
    internal interface IDirectChannel : IPrivateChannel, IMessageChannel, IAudioChannel
    {
        ulong RecipientId { get; }
    }
}
