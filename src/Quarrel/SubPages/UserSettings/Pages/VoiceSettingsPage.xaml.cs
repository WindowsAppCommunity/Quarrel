// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// The user settings Voice page.
    /// </summary>
    public sealed partial class VoiceSettingsPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceSettingsPage"/> class.
        /// </summary>
        public VoiceSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new VoiceSettingsViewModel();
        }

        /// <summary>
        /// Gets the app's voice settings.
        /// </summary>
        public VoiceSettingsViewModel ViewModel => this.DataContext as VoiceSettingsViewModel;
    }
}
