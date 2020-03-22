// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;

namespace Quarrel
{
    /// <summary>
    /// The Main View, root for the app's visual tree.
    /// </summary>
    public sealed partial class MainView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        /// <param name="splash">The Splash Screen data for the loading screen.</param>
        public MainView(SplashScreen splash)
        {
            this.InitializeComponent();
            ExtendedSplashScreen.InitializeAnimation(splash);
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            if (connections != null)
            {
                ViewModel.Login();
            }
            else
            {
                Messenger.Default.Send(new ConnectionStatusMessage(ConnectionStatus.Offline));
            }
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/> for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
