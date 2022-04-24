// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client.Models.Settings;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="GuildFolder"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuildFolder : BindableItem, IBindableGuildListItem
    {
        [ObservableProperty]
        private GuildFolder _folder;

        internal BindableGuildFolder(IDiscordService discordService, IDispatcherService dispatcherService, GuildFolder folder) :
            base(discordService, dispatcherService)
        {
            _folder = folder;

            var guilds = _folder.GetGuilds();
            Children = new ObservableCollection<BindableGuild>();
            foreach (var guild in guilds)
            {
                Children.Add(new BindableGuild(discordService, dispatcherService, guild));
            }
        }

        /// <summary>
        /// A collection of the guilds contained in the guild folder.
        /// </summary>
        public ObservableCollection<BindableGuild> Children { get; }
        
        /// <inheritdoc/>
        public string? Name => Folder.Name;
    }
}
