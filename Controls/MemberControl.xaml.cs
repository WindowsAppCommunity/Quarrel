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
            if(DisplayedMember.status.Status != null && DisplayedMember.status.Status != "invisible")
                rectangle.Fill = (SolidColorBrush)App.Current.Resources[DisplayedMember.status.Status];
            else if (DisplayedMember.status.Status == "invisible")
                rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];
            if (DisplayedMember.status.Game != null)
            {
                playing.Visibility = Visibility.Visible;
                game.Visibility = Visibility.Visible;
                game.Text = DisplayedMember.status.Game.Value.Name;
                if (DisplayedMember.status.Game.Value.State != null)
                {
                    game.Opacity = 1;
                    rich.Visibility = Visibility.Visible;
                }
                else
                {
                    game.Opacity = 0.8;
                    rich.Visibility = Visibility.Collapsed;
                }
                    
            }
            else
            {
                playing.Visibility = Visibility.Collapsed;
                rich.Visibility = Visibility.Collapsed;
                game.Visibility=Visibility.Collapsed;
            }
            if (DisplayedMember.Raw.User.Bot)
                BotIndicator.Visibility = Visibility.Visible;
            else
                BotIndicator.Visibility = Visibility.Collapsed;
        }

        public MemberControl()
        {
            this.InitializeComponent();

            App.PresenceUpdatedHandler += App_PresenceUpdatedHandler; ;
            GatewayManager.Gateway.GuildMemberUpdated += Gateway_GuildMemberUpdated;
            App.TypingHandler += App_TypingHandler;
            RegisterPropertyChangedCallback(MemberProperty, OnPropertyChanged);
            Tapped += OpenMemberFlyout;
            RightTapped += OpenMenuFlyout;
            Holding += OpenMenuFlyout;
        }

        private void App_TypingHandler(object sender, App.TypingArgs e)
        {
            if (DisplayedMember != null)
            {
                if (e.UserId == DisplayedMember.Raw.User.Id)
                {
                    DisplayedMember.IsTyping = e.Typing && e.ChnId == App.CurrentChannelId;
                    if (DisplayedMember.IsTyping)
                    {
                        ShowTyping.Begin();
                    }
                    else
                    {
                        HideTyping.Begin();
                    }
                }
            } else
            {
                App.TypingHandler -= App_TypingHandler;
            }
        }

        private async void Gateway_GuildMemberUpdated(object sender, Gateway.GatewayEventArgs<GuildMemberUpdate> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        if (DisplayedMember != null)
                        {
                            if (e.EventData.User.Id == DisplayedMember.Raw.User.Id)
                            {
                                DisplayedMember.Raw.Roles = e.EventData.Roles;
                                DisplayedMember.Raw.Nick = e.EventData.Nick;
                                rectangle.Fill = (SolidColorBrush)App.Current.Resources[DisplayedMember.status.Status];
                            }
                        }
                        else
                        {
                            App.PresenceUpdatedHandler -= App_PresenceUpdatedHandler;
                            GatewayManager.Gateway.GuildMemberUpdated -= Gateway_GuildMemberUpdated;
                        }
                    }
                    catch
                    {

                    }
                });
        }

        private async void App_PresenceUpdatedHandler(object sender, App.PresenceUpdatedArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        if (DisplayedMember != null)
                        {
                            if (e.UserId == DisplayedMember.Raw.User.Id)
                            {
                                DisplayedMember.status = e.Presence;
                                rectangle.Fill = (SolidColorBrush)App.Current.Resources[DisplayedMember.status.Status];
                            }
                        }
                        else
                        {
                            App.PresenceUpdatedHandler -= App_PresenceUpdatedHandler;
                            GatewayManager.Gateway.GuildMemberUpdated -= Gateway_GuildMemberUpdated;
                        }
                    }
                    catch
                    {

                    }
                });
        }

        private void OpenMemberFlyout(object sender, TappedRoutedEventArgs e)
        {
            App.ShowMemberFlyout(this, DisplayedMember.Raw.User);
        }

        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == HoldingState.Started)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedMember.Raw.User.Id, App.CurrentGuildId, e.GetPosition(this));

        }

        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedMember.Raw.User.Id, App.CurrentGuildId, e.GetPosition(this));
        }
    }
}
