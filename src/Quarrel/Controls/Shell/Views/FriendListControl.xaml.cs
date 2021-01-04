// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User;
using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Discord.Rest;
using Refit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Quarrel.Controls.Shell.Views
{
    /// <summary>
    /// Control for FriendsList page on DM Guild.
    /// </summary>
    public sealed partial class FriendListControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendListControl"/> class.
        /// </summary>
        public FriendListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the MainViewModel for the app.
        /// </summary>
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        public ResourceLoader ResControls = ResourceLoader.GetForCurrentView("Controls");

        private string GetString(string str)
        {
            str = str.Remove(0, 1);
            int index = str.IndexOf('/');
            string map = str;
            if (index != -1)
            {
                map = str.Remove(index);
                str = str.Remove(0, index + 1);
            }

            switch (map)
            {
                case "Controls": return ResControls.GetString(str);
            }

            return "String";
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<SendFriendRequestResponse> SendFriendRequest(string username, int discriminator)
        {
            try
            {
                IUserService userservice = SimpleIoc.Default.GetInstance<IDiscordService>().UserService;
                return await userservice.SendFriendRequest(new SendFriendRequest() { Username = username, Discriminator = discriminator });
            }
            catch (ApiException exception)
            {
                return JsonConvert.DeserializeObject<SendFriendRequestResponse>(exception.Content);
            }
        }

        private void SendFriendTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SendFriendTB.Text.Length > 0)
            {
                SendFriendRequestBTN.IsEnabled = true;
            }
            else
            {
                SendFriendRequestBTN.IsEnabled = false;
            }

            string[] strings = SendFriendTB.Text.Split('#');

            if (strings.Count() > 1)
            {
                if (strings[1].Count() > 4)
                {
                    SendFriendTB.Text = SendFriendTB.Text.Remove(SendFriendTB.Text.Length - 1);
                    SendFriendTB.SelectionStart = SendFriendTB.Text.Length;
                }
            }
        }

        private async void SendFriendRequestBTN_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (IsDigitsOnly(SendFriendTB.Text))
            {
                // All Integer
                FriendRequestStatus.Text = GetString("/Controls/AllInteger");
                FriendRequestStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                // Split at Descriminator
                string[] strings = SendFriendTB.Text.Split('#');

                if (strings.Count() == 2 && strings[1].Count() == 4)
                {
                    // Send Friend Request
                    SendFriendRequestResponse result = await SendFriendRequest(strings[0], Convert.ToInt32(strings[1]));
                    if (result != null && result.Message != null)
                    {
                        // Could not add bot, yourself or someone does not exist
                        FriendRequestStatus.Text = GetString("/Controls/InvalidInput");
                        FriendRequestStatus.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        // Success
                        FriendRequestStatus.Text = string.Format(GetString("/Controls/Success"), SendFriendTB.Text);
                        FriendRequestStatus.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
                else if (strings.Count() <= 1)
                {
                    // Need Descriminator
                    FriendRequestStatus.Text = string.Format(GetString("/Controls/NeedDesc"), strings[0]);
                    FriendRequestStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    // Invalid input
                    FriendRequestStatus.Text = GetString("/Controls/InvalidInput");
                    FriendRequestStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }
    }
}
