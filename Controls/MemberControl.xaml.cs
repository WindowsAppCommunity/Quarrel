using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.CacheModels;
using Discord_UWP.SharedModels;
using System.Threading.Tasks;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MemberControl : UserControl
    {
        public Member DisplayedMember
        {
            get { return (Member)GetValue(MemberProperty); }
            set { SetValue(MemberProperty, value); }
        }
        public static readonly DependencyProperty MemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(Member),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));


        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MemberControl;
            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (DisplayedMember == null) return;
            if (DisplayedMember.IsTyping) ShowTyping.Begin();
            else HideTyping.Begin();

            if (DisplayedMember.Raw.Nick != null)
                username.Text = DisplayedMember.Raw.Nick;
            else if (DisplayedMember.Raw.User.Username != null)
                username.Text = DisplayedMember.Raw.User.Username;
            else
                username.Text = "";

            if(DisplayedMember.Raw.User.Avatar != null)
                Avatar.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + DisplayedMember.Raw.User.Id + "/" + DisplayedMember.Raw.User.Avatar + ".png?size=64"));
            if(DisplayedMember.status.Status != null)
                rectangle.Fill = (SolidColorBrush)App.Current.Resources[DisplayedMember.status.Status];
            if (!Session.Online)
            {
                rectangle.Visibility = Visibility.Collapsed;
            }
            if (DisplayedMember.status.Game != null)
            {
                playing.Visibility = Visibility.Visible;
                game.Visibility = Visibility.Visible;
                game.Text = DisplayedMember.status.Game.Value.Name;
            }
            else
            {
                game.Visibility=Visibility.Collapsed;
            }
        }
        public MemberControl()
        {
            this.InitializeComponent();
            RegisterPropertyChangedCallback(MemberProperty, OnPropertyChanged);
            Tapped += OpenMemberFlyout;
            RightTapped += OpenMenuFlyout;
            Holding += OpenMenuFlyout;
        }

        private void OpenMemberFlyout(object sender, TappedRoutedEventArgs e)
        {
            App.ShowMemberFlyout(this, DisplayedMember.Raw.User);
        }

        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
                App.ShowMenuFlyout(this, App.Type.GuildMember, DisplayedMember.Raw.User.Id, App.CurrentGuildId, e.GetPosition(this));

        }

        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                App.ShowMenuFlyout(this, App.Type.GuildMember, DisplayedMember.Raw.User.Id, App.CurrentGuildId, e.GetPosition(this));
        }
    }
}
