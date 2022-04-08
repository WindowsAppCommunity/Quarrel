// Adam Dernis © 2022

using Discord.API.Models.Settings;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Quarrel.Bindables.Guilds
{
    public class BindableGuildFolder : ObservableObject
    {
        private GuildFolder _folder;

        public BindableGuildFolder(GuildFolder folder)
        {
            _folder = folder;

            var guilds = _folder.GetGuilds();
            Children = new ObservableCollection<BindableGuild>();
            foreach (var guild in guilds)
            {
                Children.Add(new BindableGuild(guild));
            }
        }

        public ObservableCollection<BindableGuild> Children { get; }
    }
}
