// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for channels that can belong to an <see cref="ICategoryChannel"/>.
    /// </summary>
    internal interface INestedChannel : IGuildChannel
    {
        /// <summary>
        /// The id of the parenting <see cref="ICategoryChannel"/>.
        /// </summary>
        ulong? CategoryId { get; }
    }
}
