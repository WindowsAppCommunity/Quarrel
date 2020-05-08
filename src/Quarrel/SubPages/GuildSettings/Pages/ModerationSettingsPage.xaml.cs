// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Quarrel.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// The guild settings Moderation page.
    /// </summary>
    public sealed partial class ModerationSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModerationSettingsPage"/> class.
        /// </summary>
        public ModerationSettingsPage()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                this.Bindings.Update();
            };
        }

        /// <summary>
        /// Gets the Guild's moderation settings data.
        /// </summary>
        public ModerationSettingsPageViewModel ViewModel => DataContext as ModerationSettingsPageViewModel;

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DataContext = new ModerationSettingsPageViewModel(e.Parameter as BindableGuild);
        }
    }
}
