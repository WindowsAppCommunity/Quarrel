// Quarrel © 2022

namespace Quarrel.Bindables.Guilds.Interfaces
{
    /// <summary>
    /// An interface for items that display in the guild list.
    /// </summary>
    public interface IBindableGuildListItem
    {
        /// <summary>
        /// Gets the name of the item in the list.
        /// </summary>
        string? Name { get; }
    }
}
