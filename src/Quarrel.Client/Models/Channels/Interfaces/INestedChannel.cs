// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channels that can belong to an <see cref="ICategoryChannel"/>.
    /// </summary>
    public interface INestedChannel : IGuildChannel
    {
        /// <summary>
        /// The id of the parenting <see cref="ICategoryChannel"/>.
        /// </summary>
        ulong? CategoryId { get; }
    }
}
