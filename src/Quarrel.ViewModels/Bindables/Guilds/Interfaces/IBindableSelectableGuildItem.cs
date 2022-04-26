// Quarrel © 2022

using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Interfaces;
using System.Collections.Generic;

namespace Quarrel.Bindables.Guilds.Interfaces
{
    /// <summary>
    /// An interface for items that can be treated as a selected guild.
    /// </summary>
    public interface IBindableSelectableGuildItem : IBindableGuildListItem, ISelectableItem
    {
        /// <summary>
        /// The id of the selected channel in the guild.
        /// </summary>
        /// <remarks>
        /// This is used to reopen a channel when navigating to a guild.
        /// </remarks>
        ulong? SelectedChannelId { get; set; }

        IEnumerable<BindableChannelGroup>? GetGroupedChannels(out IBindableSelectableChannel? selected);
    }
}
