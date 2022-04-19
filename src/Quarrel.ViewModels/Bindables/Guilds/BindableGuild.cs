// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client.Models.Guilds;
using System;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Guilds.Guild"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuild : SelectableItem, IBindableGuildListItem
    {
        [AlsoNotifyChangeFor(nameof(IconUrl))]
        [AlsoNotifyChangeFor(nameof(IconUri))]
        [ObservableProperty]
        private Guild _guild;

        internal BindableGuild(Guild guild)
        {
            _guild = guild;
        }

        /// <summary>
        /// The id of the selected channel in the guild.
        /// </summary>
        /// <remarks>
        /// This is used to reopen a channel when navigating to a guild.
        /// </remarks>
        public ulong? SelectedChannelId { get; set; }

        /// <summary>
        /// Gets the url of the guild's icon.
        /// </summary>
        public string IconUrl => $"https://cdn.discordapp.com/icons/{Guild.Id}/{Guild.IconId}.png?size=128";

        /// <summary>
        /// Gets the url of the guild's icon as a <see cref="Uri"/>.
        /// </summary>
        public Uri IconUri => new(IconUrl);

        /// <inheritdoc/>
        public string? Name => Guild.Name;
    }
}
