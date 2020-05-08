// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings Notifications page.
    /// </summary>
    public sealed partial class NotificationsSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsSettingsPage"/> class.
        /// </summary>
        public NotificationsSettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the guild's notification settings.
        /// </summary>
        public NotificationsSettingsPageViewModel ViewModel => DataContext as NotificationsSettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new NotificationsSettingsPageViewModel(e.Parameter as BindableGuild);
        }
    }
}
