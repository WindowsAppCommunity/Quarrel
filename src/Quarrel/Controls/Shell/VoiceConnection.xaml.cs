// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Voice;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    /// <summary>
    /// Control overlay for when the user's connected to a voice channel.
    /// </summary>
    public sealed partial class VoiceConnection : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceConnection"/> class.
        /// </summary>
        public VoiceConnection()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        private void DeafenToggle(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IVoiceService>().ToggleDeafen();
        }

        private void MuteToggle(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IVoiceService>().ToggleMute();
        }
    }
}
