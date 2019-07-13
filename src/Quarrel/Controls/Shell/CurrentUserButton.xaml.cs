using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UICompositionAnimations.Helpers;
using DiscordAPI.Gateway;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class CurrentUserButton : UserControl
    {
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        public CurrentUserButton()
        {
            this.InitializeComponent();
        }

        public BindableUser ViewModel => currentUsersService.CurrentUser;

        private async void StatusSelected(object sender, RoutedEventArgs e)
        {
            string status = (sender as RadioButton).Tag.ToString();
            discordService.Gateway.Gateway.UpdateStatus(status, 0, null);
            await discordService.UserService.UpdateSettings("{\"status\":\"" + status + "\"}");
        }
    }
}
