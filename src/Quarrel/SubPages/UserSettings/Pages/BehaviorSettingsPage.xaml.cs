// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings Behavior page.
    /// </summary>
    public sealed partial class BehaviorSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorSettingsPage"/> class.
        /// </summary>
        public BehaviorSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new BehaviorSettingsViewModel();
        }

        /// <summary>
        /// Gets the app's Behavior settings.
        /// </summary>
        public BehaviorSettingsViewModel ViewModel => this.DataContext as BehaviorSettingsViewModel;
    }
}
