// Adam Dernis © 2022

using Discord.API.Models.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Guilds.Interfaces;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Guilds
{
    /// <summary>
    /// A wrapper of a <see cref="GuildFolder"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableGuildFolder : ObservableObject, IBindableGuildListItem
    {
        [ObservableProperty]
        private GuildFolder _folder;

        internal BindableGuildFolder(GuildFolder folder)
        {
            _folder = folder;

            var guilds = _folder.GetGuilds();
            Children = new ObservableCollection<BindableGuild>();
            foreach (var guild in guilds)
            {
                Children.Add(new BindableGuild(guild));
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
