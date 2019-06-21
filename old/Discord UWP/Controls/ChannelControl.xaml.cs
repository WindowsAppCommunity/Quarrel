using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.Core;
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
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Text;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using DiscordAPI.API.Gateway;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class ChannelControl : UserControl
    {
        /// <summary>
        /// ID of channel to display
        /// </summary>
        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
            nameof(Id),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// IconUrl of channel to display
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// Name of channel to display
        /// </summary>
        public string CName
        {
            get { return (string)GetValue(ChnNameProperty); }
            set { SetValue(ChnNameProperty, value); }
        }
        public static readonly DependencyProperty ChnNameProperty = DependencyProperty.Register(
            nameof(CName),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// The presense of the user if channel is DM
        /// </summary>
        public Presence UserStatus
        {
            get { return (Presence)GetValue(UserStatusProperty); }
            set { SetValue(UserStatusProperty, value); }
        }
        public static readonly DependencyProperty UserStatusProperty = DependencyProperty.Register(
            nameof(UserStatus),
            typeof(Presence),
            typeof(ChannelControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// Subtitle for channel (for example: Member Count in Group DM)
        /// </summary>
        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
            nameof(Subtitle),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// The Game the user is playing if the channel is a DM
        /// </summary>
        public Game Playing
        {
            get { try { return (Game)GetValue(PlayingProperty); } catch { return null; } }
            set { SetValue(PlayingProperty, value); }
        }
        public static readonly DependencyProperty PlayingProperty = DependencyProperty.Register(
            nameof(Playing),
            typeof(Game),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// URL of user avatar if the channel is a DM
        /// </summary>
        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }
        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register(
            nameof(ImageUrl),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        /// <summary>
        /// 0: Text channel
        /// 1: Direct Message
        /// 2: Voice channel
        /// 3: Group DM
        /// 4: Guild category
        /// </summary>
        public int Type
        {
            get { return (int)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            nameof(Type),
            typeof(int),
            typeof(ChannelControl),
            new PropertyMetadata(0, OnPropertyChangedStatic));

        /// <summary>
        /// Number of notifcations for channel
        /// </summary>
        public int NotificationCount
        {
            get { return (int)GetValue(NotificationCountProperty); }
            set { SetValue(NotificationCountProperty, value); }
        }
        public static readonly DependencyProperty NotificationCountProperty = DependencyProperty.Register(
            nameof(NotificationCount),
            typeof(int),
            typeof(ChannelControl),
            new PropertyMetadata(0, OnPropertyChangedStatic));

        /// <summary>
        /// True if latest message in the channel is unread
        /// </summary>
        public bool IsUnread
        {
            get { return (bool)GetValue(IsUnreadProperty); }
            set { SetValue(IsUnreadProperty, value); }
        }
        public static readonly DependencyProperty IsUnreadProperty = DependencyProperty.Register(
            nameof(IsUnread),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the channel group is collapsed
        /// </summary>
        public bool IsHidden
        {
            get { return (bool)GetValue(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }
        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register(
            nameof(IsHidden),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the user has Read Permsissions on the channel
        /// </summary>
        public bool HavePermission
        {
            get { return (bool)GetValue(HavePermissionProperty); }
            set { SetValue(HavePermissionProperty, value); }
        }
        public static readonly DependencyProperty HavePermissionProperty = DependencyProperty.Register(
            nameof(HavePermission),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if someone is typing in the channel
        /// </summary>
        public bool IsTyping
        {
            get { return (bool)GetValue(IsTypingProperty); }
            set { SetValue(IsTypingProperty, value); }
        }
        public static readonly DependencyProperty IsTypingProperty = DependencyProperty.Register(
            nameof(IsTyping),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the channel is muted
        /// </summary>
        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }
        public static readonly DependencyProperty IsMutedProperty = DependencyProperty.Register(
            nameof(IsMuted),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the channel is currently selected by the user
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// True if the channel is marked NSFW
        /// </summary>
        public bool NSFW
        {
            get { return (bool)GetValue(NSFWProperty); }
            set { SetValue(NSFWProperty, value); }
        }
        public static readonly DependencyProperty NSFWProperty = DependencyProperty.Register(
            nameof(NSFW),
            typeof(bool),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// List members if the channel is a DM
        /// </summary>
        public List<string> Members
        {
            get { try { return (List<string>)GetValue(MembersProperty); } catch { return null; } }
            set { SetValue(MembersProperty, value); }
        }
        public static readonly DependencyProperty MembersProperty = DependencyProperty.Register(
            nameof(Members),
            typeof(List<string>),
            typeof(ChannelControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        /// <summary>
        /// Dictionary of Voice Status if the channel is a Voice Channel
        /// </summary>
        public Dictionary<string, VoiceMemberContainer> VoiceMembers;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ChannelControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        /// <summary>
        /// Update the opacities of elements in the ChannelControl
        /// </summary>
        private void UpdateOpacity()
        {
            if ((IsUnread && !IsMuted && HavePermission) || Type == 4)
            {
                // Show the text at 100% because there's an unread message
                ChannelName.Fade(1, 200).Start();
            }
            else
            {
                if (!HavePermission)
                {
                    // Show at 15% because the user can't enter the channel
                    ChannelName.Fade(0.15f, 200).Start();
                    HashtagIcon.Fade(0.15f, 200).Start();
                    VoiceIcon.Fade(0.15f, 200).Start();
                }
                else if (IsMuted)
                {
                    // Show at 35% because the channel is muted
                    ChannelName.Fade(0.35f, 200).Start();
                    HashtagIcon.Fade(0.35f, 200).Start();
                    VoiceIcon.Fade(0.35f, 200).Start();
                }
                else
                {
                    // Show at 55%, standard opacity
                    ChannelName.Fade(0.55f, 200).Start();
                    HashtagIcon.Fade(0.55f, 200).Start();
                    VoiceIcon.Fade(0.55f, 200).Start();
                }
            }
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == IsSelectedProperty)
            {
                if (Type == 2)
                {
                    // Because it's a Voice Channel the Voice Icon needs to be brighter too
                    if (IsSelected)
                    {
                        VoiceIcon.Fade(1, 200).Start();
                        SelectIndicator.Fade(0.4f, 200).Start();
                    }
                    else
                    {
                        VoiceIcon.Fade(0.4f, 200).Start();
                        SelectIndicator.Fade(0.4f, 200).Start();
                    }
                }
                else
                {
                    // Show Selected indicator if selected
                    if (IsSelected)
                    {
                        SelectIndicator.Fade(1, 200).Start();
                    }
                    else
                    {
                        SelectIndicator.Fade(0, 200).Start();
                    }
                }

                // Tie up loose ends
                UpdateOpacity();
                UpdateHidden();
            }
            if (prop == UserStatusProperty)
            {
                // Set the Presense icon
                if (UserStatus!= null && UserStatus.Status != null && UserStatus.Status != "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources[UserStatus.Status];
                else
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];

                // Set the game status
                if (UserStatus != null && UserStatus.Game != null)
                {
                    // There's a game, show it
                    playing.Visibility = Visibility.Visible;
                    game.Visibility = Visibility.Visible;

                    // Set name of the game
                    game.Text = UserStatus.Game.Name;

                    // Check if Rich Presence
                    if (UserStatus.Game.State != null || UserStatus.Game.Details != null || UserStatus.Game.SessionId != null)
                    {
                        game.Opacity = 1;
                        rich.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        game.Opacity = 0.4;
                        rich.Visibility = Visibility.Collapsed;
                    }

                    // Check type of rich-presense 
                    switch (UserStatus.Game.Type)
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
                    // There's no game, hide it all
                    playing.Visibility = Visibility.Collapsed;
                    rich.Visibility = Visibility.Collapsed;
                    game.Visibility = Visibility.Collapsed;
                }

            }
            if (prop == SubtitleProperty)
            {
                // Update Subtitle text
                if (Subtitle != "")
                {
                    SubTitle.Visibility = Visibility.Visible;
                    SubTitle.Text = Subtitle;
                }
                else
                {
                    SubTitle.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == IsUnreadProperty)
            {
                if (IsUnread && !IsMuted)
                {
                    // Show indicator
                    UnreadIndicator.Visibility = Visibility.Visible;
                }
                else
                {
                    // Hide indicator
                    UnreadIndicator.Visibility = Visibility.Collapsed;
                }

                // Tie up loose ends
                UpdateOpacity();
                UpdateHidden();
            }
            if (prop == HavePermissionProperty)
            {
                if (HavePermission)
                {
                    // Enable the channel if it can be entered
                    IsEnabled = true;
                    UpdateOpacity();
                } else
                {
                    // Disable the channel if it can't be entered
                    IsEnabled = false;
                    UpdateOpacity();
                }
            }
            if (prop == IsMutedProperty)
            {
                if (IsMuted)
                {
                    // Show muted icon
                    MuteIcon.Visibility = Visibility.Visible;

                    // Hide unread
                    UnreadIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Hide muted icon
                    MuteIcon.Visibility = Visibility.Collapsed;

                    // Show unread
                    if(IsUnread)
                        UnreadIndicator.Visibility = Visibility.Visible;
                }
                
                // Tie up loose ends
                UpdateOpacity();
                UpdateHidden();
            }
            if (prop == IsTypingProperty)
            {
                if (IsTyping && !IsMuted)
                {
                    // Show typing indicator
                    TypingIndic.Visibility = Visibility.Visible;
                    TypingIndic.Fade(1, 200).Start();
                }
                else
                {
                    // Hide typing indicator
                    await TypingIndic.Fade(0,200).StartAsync();
                    TypingIndic.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == NotificationCountProperty)
            {
                if (NotificationCount > 0)
                {
                    // Show Notification text
                    NotificationCounter.Text = NotificationCount.ToString();
                    ShowBadge.Begin();
                }
                else
                {
                    // Hide Notification text
                    HideBadge.Begin();
                }

                //Tie up loose ends
                UpdateHidden();
            }
            if (prop == ChnNameProperty)
            {
                // Update Channel Name
                ChannelName.Text = CName;
            }
            if (prop == ImageUrlProperty)
            {
                if (ImageUrl != "")
                {
                    // Update Image Source
                    ChannelImageBrush.ImageSource = new BitmapImage(new Uri(ImageUrl));
                }
            }
            if (prop == TypeProperty)
            {
                // Reset
                ChannelName.FontWeight = FontWeights.Normal;
                ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                Chevron.Visibility = Visibility.Collapsed;
                HoverCache.Visibility = Visibility.Collapsed;

                this.Margin = new Thickness(0);

                //TEXT
                if (Type == 0)
                {
                    HashtagIcon.Visibility = Visibility.Visible;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    MemberList.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    rectangle.Visibility = Visibility.Collapsed;
                    grid.Height = Double.NaN;
                }

                //VOICE
                else if (Type == 2)
                {
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Visible;
                    Chevron.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    rectangle.Visibility = Visibility.Collapsed;
                    VoiceMembers = new Dictionary<string, VoiceMemberContainer>();
                    grid.Height = Double.NaN;

                    // Subscribe to event of VoiceState changing
                    GatewayManager.Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

                    // Add users to list of voice members
                    foreach (var user in LocalState.VoiceDict.Values)
                    {
                        if (user.ChannelId == Id)
                        {
                            VoiceMembers.Add(user.UserId, new VoiceMemberContainer() { VoiceState = LocalState.VoiceDict[user.UserId]});
                        }
                    }

                    // Add Voice Members to display
                    if (VoiceMembers != null && VoiceMembers.Count > 0)
                    {
                        MemberList.Visibility = Visibility.Visible;
                        foreach (VoiceMemberContainer member in VoiceMembers.Values)
                        {
                            if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(member.VoiceState.UserId))
                            {
                                MemberList.Items.Add(member);
                            }
                        }
                    }
                    else
                    {
                        MemberList.Visibility = Visibility.Collapsed;
                    }
                }

                //DM
                else if (Type == 1)
                {
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility =
                             LocalState.DMs[Id].Users.FirstOrDefault().Avatar == null ?
                             Visibility.Visible : Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Visible;
                    rectangle.Visibility = Visibility.Visible;
                    Chevron.Visibility = Visibility.Collapsed;
                    rectangle.Visibility = Visibility.Visible;
                    grid.Height = 48;
                    ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                    ChannelImageBackdrop.Fill = Common.DiscriminatorColor(LocalState.DMs[Id].Users.FirstOrDefault().Discriminator);
                    ChannelImageBrush.ImageSource = new BitmapImage(Common.AvatarUri(LocalState.DMs[Id].Users.FirstOrDefault().Avatar, LocalState.DMs[Id].Users.FirstOrDefault().Id, "?size=64"));
                    MemberList.Visibility = Visibility.Collapsed;
                }

                //GROUP DM
                else if (Type == 3)
                {
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Visible;
                    rectangle.Visibility = Visibility.Collapsed;
                    Chevron.Visibility = Visibility.Collapsed;
                    grid.Height = 48;

                    // Set icon (if null)
                    if (string.IsNullOrEmpty(Icon)){
                        if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                            ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_dark.png"));
                        else
                            ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_light.png"));
                    }

                    ChannelImage.Margin = new Thickness(0,6,6,6);
                    MemberList.Visibility = Visibility.Collapsed;
                }

                //CHANNEL CATEGORY
                else if (Type == 4)
                {
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Collapsed;
                    rectangle.Visibility = Visibility.Collapsed;
                    ChannelName.FontWeight = FontWeights.Light;
                    ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["Blurple"];
                    Chevron.Visibility = Visibility.Visible;
                    HoverCache.Visibility = Visibility.Visible;
                    this.Margin = new Thickness(0, 18, 0, 0);
                    MemberList.Visibility = Visibility.Collapsed;
                    grid.Height = Double.NaN;
                }

                // Tie up loose ends
                UpdateOpacity();

                // Clear Voice Update event if applicable
                if (Type != 2)
                {
                    GatewayManager.Gateway.VoiceStateUpdated -= Gateway_VoiceStateUpdated;
                }
            }
            if (prop == IsHiddenProperty)
            {
                UpdateHidden();
            }
            if (prop == NSFWProperty)
            {
                if (NSFW)
                {
                    HashtagIcon.Foreground = ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["dnd"];
                } else
                {
                    HashtagIcon.Foreground = ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                }
            }
            if(prop == IconProperty)
            {
                if (!string.IsNullOrEmpty(Icon))
                {
                    ChannelImageBrush.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/channel-icons/" + Id + "/" + Icon + ".png"));
                }
            }
        }

        /// <summary>
        /// Voice State changed
        /// </summary>
        private async void Gateway_VoiceStateUpdated(object sender, GatewayEventArgs<VoiceState> e)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (e.EventData.ChannelId == Id)
                     {
                         // Add updated voice user to this list
                         if (!VoiceMembers.ContainsKey(e.EventData.UserId))
                         {
                             VoiceMembers.Add(e.EventData.UserId, new VoiceMemberContainer() { VoiceState = e.EventData });
                             MemberList.Items.Add(VoiceMembers[e.EventData.UserId]);
                             if (VoiceMembers.Count> 0)
                                 MemberList.Visibility = Visibility.Visible;
                         }
                     }
                     else if (VoiceMembers.ContainsKey(e.EventData.UserId))
                     {
                         // Remove updated voice user from this list
                         MemberList.Items.Remove(VoiceMembers[e.EventData.UserId]);
                         VoiceMembers.Remove(e.EventData.UserId);
                         if (VoiceMembers.Count == 0)
                             MemberList.Visibility = Visibility.Collapsed;
                     }
                 });
        }

        /// <summary>
        /// Update the Hidden Status of the control
        /// </summary>
        public void UpdateHidden()
        {
            if ((IsMuted || !((Storage.Settings.collapseOverride == CollapseOverride.Unread && IsUnread) || (Storage.Settings.collapseOverride == CollapseOverride.Mention && NotificationCount > 0))) && !IsSelected)
            {
                if (IsHidden)
                {
                    if (Type == 4)
                    {
                        // Rotate Chevron on side
                        Chevron.Rotate(-90, 7, 7, 400, 0, EasingType.Circle).Start();
                    }
                    else
                    {
                        // Hide item
                        this.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (Type == 4)
                    {
                        // Rotate Chevron upright
                        Chevron.Rotate(0, 7, 7, 400, 0, EasingType.Circle).Start();
                    }
                    else
                    {
                        // Show item
                        this.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public ChannelControl()
        {
            this.InitializeComponent();
            this.Holding += OpenMenuFlyout;
            this.RightTapped += OpenMenuFlyout;
        }

        /// <summary>
        /// Open flyout (right-tapped)
        /// </summary>
        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != PointerDeviceType.Touch)
                OpenMenuFlyout(e.GetPosition(this));
        }

        /// <summary>
        /// Open flyout (holding)
        /// </summary>
        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (e.HoldingState == HoldingState.Started)
                    OpenMenuFlyout(e.GetPosition(this));
                    
            }
            catch { }
        }

        /// <summary>
        /// Open Menu Flyout by type
        /// </summary>
        private void OpenMenuFlyout(Point e)
        {
            switch (Type)
            {
                case 0: /*Text*/
                    App.ShowMenuFlyout(this, FlyoutManager.Type.TextChn, Id, App.CurrentGuildId, e);
                    break;
                case 1: /*DM*/
                    App.ShowMenuFlyout(this, FlyoutManager.Type.DMChn, Id, null,  e);
                    break;
                case 2: /*Voice*/
                    break;
                case 3: /*Group*/
                    App.ShowMenuFlyout(this, FlyoutManager.Type.GroupChn, Id, App.CurrentGuildId, e);
                    break;
            }
        }

        /// <summary>
        /// Universal Pointer down (for SideDrawer)
        /// </summary>
        private void UserControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }


        /// <summary>
        /// Unloaded
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            GatewayManager.Gateway.VoiceStateUpdated -= Gateway_VoiceStateUpdated;
            this.Holding -= OpenMenuFlyout;
            this.RightTapped -= OpenMenuFlyout;
        }

        /// <summary>
        /// Show UserFlyout from Voice-MemberList
        /// </summary>
        private void MemberList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var memberItem = (ListViewItem)MemberList.ContainerFromItem(e.ClickedItem);
            App.ShowMemberFlyout(memberItem, LocalState.Guilds[App.CurrentGuildId].members[(e.ClickedItem as VoiceMemberContainer).VoiceState.UserId].User, false);
        }
    }
}
