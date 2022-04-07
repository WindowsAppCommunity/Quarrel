// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Quarrel.Bindables.Guilds
{
    public partial class BindableGuild : ObservableObject
    {
        [AlsoNotifyChangeFor(nameof(IconUrl))]
        [AlsoNotifyChangeFor(nameof(IconUri))]
        [ObservableProperty]
        private Guild _guild;

        public BindableGuild(Guild guild)
        {
            Guild = guild;
        }

        public string IconUrl => $"https://cdn.discordapp.com/icons/{Guild.Id}/{Guild.IconId}.png?size=128";

        public Uri IconUri => new Uri(IconUrl);
    }
}
