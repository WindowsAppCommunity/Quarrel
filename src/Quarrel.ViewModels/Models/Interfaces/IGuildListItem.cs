// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Models.Interfaces
{
    public interface IGuildListItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the GuildListItem is collapsed.
        /// </summary>
        bool IsCollapsed { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the GuildListItem is muted.
        /// </summary>
        bool IsMuted { get; }

        /// <summary>
        /// Gets a value indicating whether or not the GuildListItem should be displayed as unread.
        /// </summary>
        bool ShowUnread { get; }
    }
}
