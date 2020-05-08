// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Invite Settings page data.
    /// </summary>
    public class InviteSettingsPageViewModel
    {
        private BindableGuild _guild;

        /// <summary>
        /// Initializes a new instance of the <see cref="InviteSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to view the invites of.</param>
        public InviteSettingsPageViewModel(BindableGuild guild)
        {
            _guild = guild;
            LoadInvites();
        }

        /// <summary>
        /// Gets or sets a list of invites for the guilds.
        /// </summary>
        public ObservableCollection<BindableInvite> Invites { get; set; } = new ObservableCollection<BindableInvite>();

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private async void LoadInvites()
        {
            var invites = await DiscordService.GuildService.GetGuildInvites(_guild.Model.Id);
            foreach (var invite in invites)
            {
                Invites.Add(new BindableInvite(invite));
            }
        }
    }
}
