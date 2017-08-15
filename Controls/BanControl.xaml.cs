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
using Discord_UWP.SharedModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.Controls
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BanControl : Page
    {
        public Ban DisplayedBan
        {
            get { return (Ban)GetValue(DisplayedBanProperty); }
            set { SetValue(DisplayedBanProperty, value); }
        }
        public static readonly DependencyProperty DisplayedBanProperty = DependencyProperty.Register(
            nameof(DisplayedBan),
            typeof(Ban),
            typeof(BanControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as BanControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedBanProperty)
            {
                username.Text = DisplayedBan.User.Username;
                discriminator.Text = DisplayedBan.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedBan.User.Avatar));
            }
        }

        public BanControl()
        {
            this.InitializeComponent();
        }

        private void RevokeBan(object sender, RoutedEventArgs e)
        {
            //TODO: RevokeBan
            Session.RemoveBan(App.CurrentGuildId,DisplayedBan.User.Id);
        }
    }
}
