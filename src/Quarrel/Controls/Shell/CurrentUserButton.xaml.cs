// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Gateway;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    /// <summary>
    /// Button at bottom of ChannelList displaying current user info.
    /// </summary>
    public sealed partial class CurrentUserButton : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserButton"/> class.
        /// </summary>
        public CurrentUserButton()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        /// <summary>
        /// Update status according to selected radio button.
        /// </summary>
        private async void StatusSelected(object sender, RoutedEventArgs e)
        {
            string status = (sender as RadioButton).Tag.ToString();
            SimpleIoc.Default.GetInstance<IGatewayService>().Gateway.UpdateStatus(status, 0, null);
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings("{\"status\":\"" + status + "\"}");
        }
    }
}
