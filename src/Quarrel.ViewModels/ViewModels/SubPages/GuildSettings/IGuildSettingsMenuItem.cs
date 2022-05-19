// Quarrel © 2022

namespace Quarrel.ViewModels.SubPages.GuildSettings
{
    /// <summary>
    /// An interface for items in the guild settings navigation menu.
    /// </summary>
    public interface IGuildSettingsMenuItem
    {
        /// <summary>
        /// Gets the title of the menu item.
        /// </summary>
        string Title { get; }
    }
}
