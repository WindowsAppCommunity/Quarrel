// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for direct message channels.
    /// </summary>
    public interface IDirectChannel : IPrivateChannel
    {
        /// <summary>
        /// Gets the id of the other direct message channel member.
        /// </summary>
        ulong RecipientId { get; }
    }
}
