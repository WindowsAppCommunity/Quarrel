// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings Privacy page.
    /// </summary>
    public sealed partial class PrivacySettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivacySettingsPage"/> class.
        /// </summary>
        public PrivacySettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the guild's Privacy settings.
        /// </summary>
        public PrivacySettingsPageViewModel ViewModel => DataContext as PrivacySettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new PrivacySettingsPageViewModel(e.Parameter as BindableGuild);
        }
    }
}
