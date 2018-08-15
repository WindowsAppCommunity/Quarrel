using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using System.Threading.Tasks;

using Discord_UWP.Managers;
using Windows.UI.Text;
using Discord_UWP.SimpleClasses;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MemberControl : UserControl
    {
        public GuildMember RawMember
        {
            get { return (GuildMember)GetValue(RawMemberProperty); }
            set { SetValue(RawMemberProperty, value); }
        }
        public static readonly DependencyProperty RawMemberProperty = DependencyProperty.Register(
            nameof(RawMember),
            typeof(GuildMember),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        public Presence Status
        {
            get {
                var pr = GetValue(PresenceProperty);
                if (pr != null) return (Presence)pr;
                else return new Presence();
            }
            set { SetValue(PresenceProperty, value); }
        }
        public static readonly DependencyProperty PresenceProperty = DependencyProperty.Register(
            nameof(Status),
            typeof(Presence),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        public HoistRole Role
        {
            get { return (HoistRole)GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }
        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            nameof(Role),
            typeof(HoistRole),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        public bool IsTyping
        {
            get { return (bool)GetValue(IsTypingProperty); }
            set { SetValue(IsTypingProperty, value); }
        }
        public static readonly DependencyProperty IsTypingProperty = DependencyProperty.Register(
            nameof(IsTyping),
            typeof(bool),
            typeof(MemberControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
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
           
            if(prop == TextProperty)
            {
                if (Text == null) return;
                username.Text = Text;
                return;
            }
            else if(prop == RawMemberProperty)
            {
                if (RawMember == null) return;


                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(RawMember.User.Avatar, RawMember.User.Id, "?size=64"));
                if (RawMember.User.Avatar != null)
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");
                else
                    AvatarBG.Fill = Common.DiscriminatorColor(RawMember.User.Discriminator);
                if (RawMember.User.Bot)
                    BotIndicator.Visibility = Visibility.Visible;
                else
                    BotIndicator.Visibility = Visibility.Collapsed;
            }
            else if(prop == IsTypingProperty)
            {
                if (IsTyping)
                    ShowTyping.Begin();
                else
                    HideTyping.Begin();
            }
            else if(prop == PresenceProperty)
            {
                if (Status.Status != null && Status.Status != "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources[Status.Status];
                else if (Status.Status == "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];
                if (Status.Game != null)
                {
                    playing.Visibility = Visibility.Visible;
                    game.Visibility = Visibility.Visible;
                    game.Text = Status.Game.Name;
                    UpdateColor();
                    if (Status.Game.State != null || Status.Game.Details != null || Status.Game.Assets != null)
                    {
                        game.Opacity = 1;
                        rich.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        game.Opacity = 0.6;
                        rich.Visibility = Visibility.Collapsed;
                    }

                    switch (Status.Game.Type)
                    {
                        case 0:
                            playing.Text = App.GetString("/Controls/Playing");
                            break;
                        case 1:
                            playing.Text = App.GetString("/Controls/Streaming");
                            break;
                        case 2:
                            playing.Text = App.GetString("/Controls/ListeningTo");
                            break;
                        case 3:
                            playing.Text = App.GetString("/Controls/Watching");
                            break;
                    }
                }
                else
                {
                    playing.Visibility = Visibility.Collapsed;
                    rich.Visibility = Visibility.Collapsed;
                    game.Visibility = Visibility.Collapsed;
                }
            }
            else if (prop == RoleProperty)
            {
                if (RawMember == null) return;
                UpdateColor();
            }
        }
        private void UpdateColor()
        {
            if (RawMember?.Roles != null)
            {
                bool changed = false;
                foreach (var role in RawMember.Roles)
                    if (LocalState.Guilds[App.CurrentGuildId].roles[role].Color != 0)
                    {
                        username.Foreground = Common.IntToColor(LocalState.Guilds[App.CurrentGuildId].roles[role].Color);
                        changed = true;
                        break;
                    }
                if (changed == false)
                    username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
            }
            else
                username.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
        }
        public MemberControl()
        {
            this.InitializeComponent();

           // App.DisposeMemberListHandler += Dispose;
          //  App.TypingHandler += App_TypingHandler;
           // RegisterPropertyChangedCallback(MemberProperty, OnPropertyChanged);
            RightTapped += OpenMenuFlyout;
            Holding += OpenMenuFlyout;
        }

        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, RawMember.User.Id, App.CurrentGuildId, e.GetPosition(this));

        }

        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, RawMember.User.Id, App.CurrentGuildId, e.GetPosition(this));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            RightTapped -= OpenMenuFlyout;
            Holding -= OpenMenuFlyout;
        }
    }
}
