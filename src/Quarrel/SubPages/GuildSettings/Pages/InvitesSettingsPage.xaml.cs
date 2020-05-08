// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings Invite page.
    /// </summary>
    public sealed partial class InvitesSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvitesSettingsPage"/> class.
        /// </summary>
        public InvitesSettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the AuditLog data.
        /// </summary>
        public InviteSettingsPageViewModel ViewModel => DataContext as InviteSettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new InviteSettingsPageViewModel(e.Parameter as BindableGuild);
        }

        private void InviteControl_RemoveClicked(object sender, BindableInvite e)
        {
            ViewModel.Invites.Remove(e);
        }
    }
}
