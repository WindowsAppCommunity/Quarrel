// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings Display page.
    /// </summary>
    public sealed partial class DisplaySettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplaySettingsPage"/> class.
        /// </summary>
        public DisplaySettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new DisplaySettingsViewModel();
        }

        /// <summary>
        /// Gets the app's Display settings.
        /// </summary>
        public DisplaySettingsViewModel ViewModel => this.DataContext as DisplaySettingsViewModel;
    }
}
