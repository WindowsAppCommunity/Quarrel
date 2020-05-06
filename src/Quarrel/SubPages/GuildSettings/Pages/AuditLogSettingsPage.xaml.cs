// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings AuditLog page.
    /// </summary>
    public sealed partial class AuditLogSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogSettingsPage"/> class.
        /// </summary>
        public AuditLogSettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the AuditLog data.
        /// </summary>
        public AuditLogSettingsPageViewModel ViewModel => DataContext as AuditLogSettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new AuditLogSettingsPageViewModel(e.Parameter as BindableGuild);
        }
    }
}
