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
using DiscordAPI.API.Gateway;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Quarrel.LocalModels;
using System.Text.RegularExpressions;
using Quarrel.Managers;

namespace Quarrel.Controls
{
    public sealed partial class FriendControl : UserControl
    {
        /// <summary>
        /// Friend data to display
        /// </summary>
        public FriendPanel.SimpleFriend DisplayedFriend
        {
            get { return GetValue(DisplayedFriendProperty) is FriendPanel.SimpleFriend ? (FriendPanel.SimpleFriend)GetValue(DisplayedFriendProperty) : null; }
            set { SetValue(DisplayedFriendProperty, value); }
        }
        public static readonly DependencyProperty DisplayedFriendProperty = DependencyProperty.Register(
            nameof(DisplayedFriend),
            typeof(FriendPanel.SimpleFriend),
            typeof(FriendControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// Invoked when accept friend request button is pressed
        /// </summary>
        public event EventHandler AcceptFriend;
        
        /// <summary>
        /// Invoked when remove friend or decline friend request button is pressed 
        /// </summary>
        public event EventHandler RemovedFriend;

        // TODO: Add ability to start call from friends list
        //public event EventHandler StartVoiceCall;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as FriendControl;
            instance?.OnPropertyChanged(d, e.Property);
        }
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedFriendProperty)
            {
                // Null check
                if (DisplayedFriend == null) return;

                // Adjust User dewtails
                username.Text = DisplayedFriend.User.Username;
                discriminator.Text = "#" + DisplayedFriend.User.Discriminator;
                Avatar.ImageSource = new BitmapImage(Common.AvatarUri(DisplayedFriend.User.Avatar, DisplayedFriend.User.Id));

                // If the Avater is null, show default image
                if (DisplayedFriend.User.Avatar != null)
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");
                else
                    AvatarBG.Fill = Common.DiscriminatorColor(DisplayedFriend.User.Discriminator);

                // Show what guilds both members are a part of
                SharedGuildContainer.Children.Clear();
                if (DisplayedFriend.SharedGuilds != null)
                {
                    foreach (var guild in DisplayedFriend.SharedGuilds)
                    {
                        // Create Border content
                        Border content = new Border();
                        content.Width = 32;
                        content.Height = 32;
                        content.CornerRadius = new CornerRadius(16);

                        // Add image
                        if (string.IsNullOrEmpty(guild.ImageUrl))
                        {
                            var blurplecolor = ((Color)App.Current.Resources["BlurpleColor"]);
                            content.Background = new SolidColorBrush(Color.FromArgb(153, blurplecolor.R, blurplecolor.G, blurplecolor.B));
                            content.Child = new TextBlock()
                            {
                                Text = String.Join("", Regex.Matches(guild.Name, @"(?<=^|[ \-_|+=~])\w")
                                                            .Cast<Match>()
                                                            .Select(m => m.Value)
                                                            .ToArray()),
                                FontSize = 12,
                                Opacity = 1,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        else
                        {
                            content.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri("https://discordapp.com/api/guilds/" + guild.Id + "/icons/" + guild.ImageUrl + ".jpg")) };
                        }

                        // Create container button
                        Button b = new Button()
                        {
                            Padding = new Thickness(0),
                            Background = new SolidColorBrush(Windows.UI.Colors.Transparent),
                            Margin = new Thickness(2, 0, 2, 0),
                            Content = content,
                            Tag = guild.Id
                        };
                        ToolTipService.SetToolTip(b, guild.Name);
                        b.Click += ClickedGuild;

                        // Add to StackPanel
                        SharedGuildContainer.Children.Add(b);
                    }
                }

                // Adjust presence status
                if (DisplayedFriend.UserStatus != null)
                    status.Fill = (SolidColorBrush)App.Current.Resources[DisplayedFriend.UserStatus];

                // Toggle visible buttons by relationship status
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

        /// <summary>
        /// Navigate to Guild by Tag (for Shared Guild buttons)
        /// </summary>
        private void ClickedGuild(object sender, RoutedEventArgs e)
        {
            App.NavigateToGuild((sender as Button).Tag as string);
        }

        public FriendControl()
        {
            this.InitializeComponent();
            GatewayManager.Gateway.PresenceUpdated += Gateway_PresenceUpdated;
        }

        /// <summary>
        /// Event handled when a User Presence is updated
        /// </summary>
        private async void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    // Null check object
                    if (DisplayedFriend == null) return;

                    // If it's the same user
                    if (e.EventData.User.Id == DisplayedFriend.User.Id)
                    {
                        // Update user status
                        DisplayedFriend.UserStatus = e.EventData.Status;
                        status.Fill = (SolidColorBrush)App.Current.Resources[e.EventData.Status];
                    }
                });
        }

        /// <summary>
        /// Accept Friend request
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RESTCalls.SendFriendRequest(DisplayedFriend.User.Id);
            AcceptFriend?.Invoke(null,null);
        }

        /// <summary>
        /// Remove friend or decline friend request
        /// </summary>
        private async void RemoveRelationship(object sender, RoutedEventArgs e)
        {
            await RESTCalls.RemoveFriend(DisplayedFriend.User.Id);
            RemovedFriend?.Invoke(null,null);
        }

        /// <summary>
        /// Open context menu
        /// </summary>
        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            moreButton.Flyout.ShowAt(moreButton);
        }

        /// <summary>
        /// Member flyout
        /// </summary>
        private void Username_OnClick(object sender, RoutedEventArgs e)
        {
            App.ShowMemberFlyout(username, DisplayedFriend.User, false);
        }

        /// <summary>
        /// User flyout (right-tapped)
        /// </summary>
        private void username_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedFriend.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        /// <summary>
        /// User flyout (holding)
        /// </summary>
        private void username_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                if (!App.CurrentGuildIsDM)
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GuildMember, DisplayedFriend.User.Id, App.CurrentGuildId, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            SharedGuildContainer.Children.Clear();
            GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
        }
    }
}
