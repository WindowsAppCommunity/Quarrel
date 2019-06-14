using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using DiscordAPI.SharedModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.Controls
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BanControl : Page
    {
        /// <summary>
        /// API ban data to display
        /// </summary>
        public Ban DisplayedBan
        {
            get { return (Ban)GetValue(DisplayedBanProperty); }
            set { SetValue(DisplayedBanProperty, value); }
        }
        public static readonly DependencyProperty DisplayedBanProperty = DependencyProperty.Register(
            nameof(DisplayedBan),
            typeof(Ban),
            typeof(BanControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as BanControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedBanProperty)
            {
                // Set User details
                username.Text = DisplayedBan.User.Username;
                discriminator.Text = "#"+DisplayedBan.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedBan.User.Avatar, DisplayedBan.User.Id));

                // Set Ban details
                if (!string.IsNullOrWhiteSpace(DisplayedBan.Reason))
                {
                    reason.Visibility = Visibility.Visible;
                    reason.Text = DisplayedBan.Reason;
                }
                else
                {
                    reason.Visibility = Visibility.Collapsed;
                }
            }
        }

        public BanControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Make API call to revoke ban
        /// </summary>
        private async void RevokeBan(object sender, RoutedEventArgs e)
        {
            await RESTCalls.RemoveBan(DisplayedBan.GuildId, DisplayedBan.User.Id);
        }

        public void Dispose()
        {
            //Nothing to dispose
        }
    }
}
