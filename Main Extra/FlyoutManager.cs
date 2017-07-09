using Discord_UWP.API;
using Discord_UWP.API.User;
using Discord_UWP.API.User.Models;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.CacheModels;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.Gateway;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace Discord_UWP
{
    public sealed partial class Main : Page
    {
        private void ShowMenu(object sender, App.MenuArgs e)
        {
            MenuFlyout flyout = new MenuFlyout();
            switch (e.Type)
            {
                case App.Type.Guild:
                    break;
                case App.Type.DMChn:
                    MakeDMChannelMenu(Storage.Cache.DMs[e.Id]);
                    break;
            }
            flyout.ShowAt(Frame, new Point(e.X, e.Y));
        }

        private MenuFlyout MakeDMChannelMenu(DmCache dm)
        {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem profile = new MenuFlyoutItem() { Text = "Profile", Tag = dm.Raw.Users.FirstOrDefault().Id };
            profile.Click += OpenProfile;
            return menu;
        }

        private void OpenProfile(object sender, RoutedEventArgs e)
        {
            ShowUserDetails((sender as MenuFlyoutItem).Tag.ToString());
        }
    }
}
