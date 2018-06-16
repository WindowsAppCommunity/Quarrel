using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

namespace Discord_UWP.Controls
{
    public sealed partial class FriendControl : UserControl
    {
        public FriendPanel.SimpleFriend DisplayedFriend
        {
            get { try { return (FriendPanel.SimpleFriend)GetValue(DisplayedFriendProperty); } catch { return null;  } }
            set { SetValue(DisplayedFriendProperty, value); }
        }
        public static readonly DependencyProperty DisplayedFriendProperty = DependencyProperty.Register(
            nameof(DisplayedFriend),
            typeof(FriendPanel.SimpleFriend),
            typeof(FriendControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public event EventHandler AcceptFriend;
        public event EventHandler RemovedFriend;
        //public event EventHandler StartVoiceCall;
        //public event EventHandler SentMessage;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FriendControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedFriendProperty)
            {
                if (DisplayedFriend == null) return;
                username.Text = DisplayedFriend.User.Username;
                discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedFriend.User.Avatar, DisplayedFriend.User.Id));
                if (DisplayedFriend.User.Avatar != null)
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");
                else
                    AvatarBG.Fill = Common.DiscriminatorColor(DisplayedFriend.User.Discriminator);

                SharedGuildContainer.Children.Clear();
                if(DisplayedFriend.SharedGuilds != null)
                foreach (var guild in DisplayedFriend.SharedGuilds)
                {
                    Button b = new Button()
                    {
                        Padding = new Thickness(0),
                        Background = new SolidColorBrush(Windows.UI.Colors.Transparent),
                        Margin = new Thickness(2,0,2,0),
                        Content = new Rectangle()
                        {
                            Height=36,
                            Width=36,
                            RadiusX=36,
                            RadiusY=36,
                            Fill=new ImageBrush()
                            {
                                ImageSource = new BitmapImage(new Uri(guild.ImageUrl))
                            }
                        },
                        Tag = guild.Id
                    };
                        ToolTipService.SetToolTip(b, guild.Name);
                    b.Click += ClickedGuild;
                    SharedGuildContainer.Children.Add(b);
                }
                if(DisplayedFriend.UserStatus != null)
                status.Fill = (SolidColorBrush)App.Current.Resources[DisplayedFriend.UserStatus];
                switch (DisplayedFriend.RelationshipStatus)
                {
                    case 1: //Friend
                        RemoveButton.Visibility = Visibility.Collapsed;
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Collapsed;
                        moreButton.Visibility = Visibility.Visible;
                        break;
                    case 2: //Blocked
                        RemoveButton.Visibility = Visibility.Visible;
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Collapsed;
                        moreButton.Visibility = Visibility.Collapsed;
                        break;
                    case 3: //Incoming request
                        AcceptButton.Visibility = Visibility.Visible;
                        RemoveButton.Visibility = Visibility.Visible;
                        RelationshipStatus.Visibility = Visibility.Visible;
                        moreButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Text = App.GetString("/Controls/AcceptFriendRequestQ");
                        break;
                    case 4: //Outgoing request
                        AcceptButton.Visibility = Visibility.Collapsed;
                        RemoveButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Visibility = Visibility.Visible;
                        moreButton.Visibility = Visibility.Collapsed;
                        RelationshipStatus.Text = App.GetString("/Controls/FriendRequestSent");
                        break;
                }
            }
        }

        private void ClickedGuild(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuild((sender as Button).Tag as string);
        }

        public FriendControl()
        {
            this.InitializeComponent();
            GatewayManager.Gateway.PresenceUpdated += Gateway_PresenceUpdated;
        }

        private async void Gateway_PresenceUpdated(object sender, Gateway.GatewayEventArgs<SharedModels.Presence> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (DisplayedFriend == null) return;
                    DisplayedFriend.UserStatus = e.EventData.Status;
                    if (e.EventData.User.Id == DisplayedFriend.User.Id)
                        status.Fill = (SolidColorBrush)App.Current.Resources[e.EventData.Status];
                });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RESTCalls.SendFriendRequest(DisplayedFriend.User.Id);
            AcceptFriend?.Invoke(null,null);
        }

        private async void RemoveRelationship(object sender, RoutedEventArgs e)
        {
            await RESTCalls.RemoveFriend(DisplayedFriend.User.Id);
            RemovedFriend?.Invoke(null,null);
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            moreButton.Flyout.ShowAt(moreButton);
        }


        private void Username_OnClick(object sender, RoutedEventArgs e)
        {
            App.ShowMemberFlyout(username, DisplayedFriend.User);
        }

        private void username_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedFriend.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedFriend.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        public void Dispose()
        {
            SharedGuildContainer.Children.Clear();
            GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
        }
    }
}
