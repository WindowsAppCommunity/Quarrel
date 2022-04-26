// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for direct message channels.
    /// </summary>
    public interface IDirectChannel : IPrivateChannel
    {
        ulong RecipientId { get; }
    }
}
