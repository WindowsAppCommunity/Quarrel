// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings Privacy page.
    /// </summary>
    public sealed partial class PrivacySettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivacySettingsPage"/> class.
        /// </summary>
        public PrivacySettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new PrivacySettingsViewModel();
        }

        /// <summary>
        /// Gets the user's Privacy settings.
        /// </summary>
        public PrivacySettingsViewModel ViewModel => this.DataContext as PrivacySettingsViewModel;
    }
}
