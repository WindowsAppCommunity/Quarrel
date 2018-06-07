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
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Windows.UI.Text;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class ChannelControl : UserControl
    {
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

        public Game? Playing
        {
            get { try { return (Game?)GetValue(PlayingProperty); } catch { return null; } }
            set { SetValue(PlayingProperty, value); }
        }
        public static readonly DependencyProperty PlayingProperty = DependencyProperty.Register(
            nameof(Playing),
            typeof(Game?),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

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

        public Dictionary<string, VoiceMemberContainer> VoiceMembers;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ChannelControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            //if (!App.CurrentGuildIsDM && Id != "")
            //{
            //    if (!Storage.Cache.Guilds[App.CurrentGuildId].Channels[Id].chnPerms.EffectivePerms.Administrator && !Storage.Cache.Guilds[App.CurrentGuildId].Channels[Id].chnPerms.EffectivePerms.ReadMessages)
            //    {
            //        this.Visibility = Visibility.Collapsed;
            //    }
            //}

            if (prop == IsSelectedProperty)
            {
                if (Type == 2)
                {
                    if (IsSelected)
                    {
                        ChannelName.Fade(1, 200).Start();
                        VoiceIcon.Fade(1, 200).Start();
                        SelectIndicator.Fade(0.6f, 200).Start();
                    }
                    else
                    {
                        ChannelName.Fade(0.6f, 200).Start();
                        VoiceIcon.Fade(0.6f, 200).Start();
                        SelectIndicator.Fade(0.6f, 200).Start();
                    }
                }
                else
                {
                    if (IsSelected)
                    {
                        SelectIndicator.Fade(1, 200).Start();
                    }
                    else
                    {
                        SelectIndicator.Fade(0, 200).Start();
                    }
                }
                UpdateHidden();
            }
            else if (prop == UserStatusProperty)
            {
                if (UserStatus!= null && UserStatus.Status != null && UserStatus.Status != "invisible")
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources[UserStatus.Status];
                else
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];
                if (UserStatus != null && UserStatus.Game != null)
                {
                    playing.Visibility = Visibility.Visible;
                    game.Visibility = Visibility.Visible;
                    game.Text = UserStatus.Game.Value.Name;
                    if (UserStatus.Game.Value.State != null || UserStatus.Game.Value.Details != null || UserStatus.Game.Value.SessionId != null)
                    {
                        game.Opacity = 1;
                        rich.Visibility = Visibility.Visible;
                        switch (UserStatus.Game.Value.Type)
                        {
                            case -1:
                                playing.Visibility = Visibility.Collapsed; break;
                            case 0:
                                playing.Visibility = Visibility.Visible;
                                playing.Text = "Playing"; break;
                            case 1:
                                playing.Visibility = Visibility.Visible;
                                playing.Text = "Streaming"; break;
                            case 2:
                                playing.Visibility = Visibility.Visible;
                                playing.Text = "Listening to"; break;
                        }
                    }
                    else
                    {
                        game.Opacity = 0.6;
                        rich.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    playing.Visibility = Visibility.Collapsed;
                    rich.Visibility = Visibility.Collapsed;
                    game.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == SubtitleProperty)
            {
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
                    ChannelName.Fade(1,200).Start();
                    UnreadIndicator.Visibility = Visibility.Visible;
                    if (IsHidden)
                    {
                        //await this.Fade(0, 200, 0).StartAsync();
                        this.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    ChannelName.Fade(0.6f, 200).Start();
                    UnreadIndicator.Visibility = Visibility.Collapsed;
                    if (IsHidden)
                    {
                        this.Visibility = Visibility.Visible;
                        this.Fade(1, 200, 0).Start();
                    }
                }
                UpdateHidden();
            }
            if (prop == IsMutedProperty)
            {
                if (IsMuted)
                {
                    ChannelName.Opacity = 0.5;
                    MuteIcon.Visibility = Visibility.Visible;
                    ChannelName.Fade(0.75f, 200).Start();
                    UnreadIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MuteIcon.Visibility = Visibility.Collapsed;
                }
                UpdateHidden();
            }
            if (prop == IsTypingProperty)
            {
                if (IsTyping)
                {
                    TypingIndic.Visibility = Visibility.Visible;
                    TypingIndic.Fade(1, 200).Start();
                }
                else
                {
                    await TypingIndic.Fade(0,200).StartAsync();
                    TypingIndic.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == NotificationCountProperty)
            {
                if (NotificationCount > 0)
                {
                    NotificationCounter.Text = NotificationCount.ToString();
                    ShowBadge.Begin();
                }
                else
                {
                    HideBadge.Begin();
                }
                UpdateHidden();
            }
            if (prop == ChnNameProperty)
            {
                    ChannelName.Text = CName;
            }
            if (prop == ImageUrlProperty)
            {
                if (ImageUrl != "")
                {
                    ChannelImageBrush.ImageSource = new BitmapImage(new Uri(ImageUrl));
                }
            }
            if (prop == TypeProperty)
            {
                ChannelName.FontWeight = FontWeights.Normal;
                ChannelName.Opacity = 0.75;
                ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["Foreground"];
                Chevron.Visibility = Visibility.Collapsed;
                HoverCache.Visibility = Visibility.Collapsed;

                this.Margin = new Thickness(0);

                if (Type == 0)
                {
                    //TEXT
                    HashtagIcon.Visibility = Visibility.Visible;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    MemberList.Visibility = Visibility.Collapsed;
                    Tapped -= JoinVoiceChannel;
                }
                else if (Type == 2)
                {
                    //VOICE
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Visible;

                    VoiceMembers = new Dictionary<string, VoiceMemberContainer>();

                    GatewayManager.Gateway.VoiceStateUpdated += Gateway_VoiceStateUpdated;

                    foreach (var user in LocalState.VoiceDict.Values)
                    {
                        if (user.ChannelId == Id)
                        {
                            VoiceMembers.Add(user.UserId, new VoiceMemberContainer() { VoiceState = LocalState.VoiceDict[user.UserId]});
                        }
                    }

                    if (VoiceMembers != null)
                    {
                        foreach (VoiceMemberContainer member in VoiceMembers.Values)
                        {
                            if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(member.VoiceState.UserId))
                            {
                                MemberList.Items.Add(member);
                            }
                        }
                        //Debug MemberList.Items.Add(new VoiceMemberControl.SimpleMember() { Member = Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id] });
                    }

                    Tapped += JoinVoiceChannel;
                }
                else if (Type == 1)
                {
                    //DM
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    if (LocalState.DMs[Id].Users.FirstOrDefault().Avatar == null)
                    {
                        ChannelImageBackdrop.Visibility = Visibility.Visible;
                    }
                    ChannelImage.Visibility = Visibility.Visible;
                    rectangle.Visibility = Visibility.Visible;
                    grid.Height = 48;
                    ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                    ChannelImageBackdrop.Fill = Common.DiscriminatorColor(LocalState.DMs[Id].Users.FirstOrDefault().Discriminator);
                    ChannelImageBrush.ImageSource = new BitmapImage(Common.AvatarUri(LocalState.DMs[Id].Users.FirstOrDefault().Avatar, LocalState.DMs[Id].Users.FirstOrDefault().Id, "?size=64"));
                    MemberList.Visibility = Visibility.Collapsed;
                    Tapped -= JoinVoiceChannel;
                }
                else if (Type == 3)
                {
                    //GROUP DM
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Visible;
                    ChannelImage.Visibility = Visibility.Visible;
                    rectangle.Visibility = Visibility.Collapsed;
                    grid.Height = 48;
                    //ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("ms-appx:///Assets/DiscordAssets/groupchat.svg"));

                    if (string.IsNullOrEmpty(Icon)){
                        if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                            ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_dark.png"));
                        else
                            ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_light.png"));

                    }

                    ChannelImage.Margin = new Thickness(0,6,6,6);
                    MemberList.Visibility = Visibility.Collapsed;
                    Tapped -= JoinVoiceChannel;
                }
                else if (Type == 4)
                {
                    //CHANNEL CATEGORY
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Collapsed;
                    rectangle.Visibility = Visibility.Collapsed;
                    ChannelName.FontWeight = FontWeights.Light;
                    ChannelName.Opacity = 1;
                    ChannelName.Foreground = (SolidColorBrush)App.Current.Resources["Blurple"];
                    Chevron.Visibility = Visibility.Visible;
                    HoverCache.Visibility = Visibility.Visible;
                    this.Margin = new Thickness(0, 18, 0, 0);
                    MemberList.Visibility = Visibility.Collapsed;
                    Tapped -= JoinVoiceChannel;
                }

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

        private async void Gateway_VoiceStateUpdated(object sender, Gateway.GatewayEventArgs<VoiceState> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     if (e.EventData.ChannelId == Id)
                     {
                         if (!VoiceMembers.ContainsKey(e.EventData.UserId))
                         {
                             VoiceMembers.Add(e.EventData.UserId, new VoiceMemberContainer() { VoiceState = e.EventData });
                             MemberList.Items.Add(VoiceMembers[e.EventData.UserId]);
                         }
                     }
                     else if (VoiceMembers.ContainsKey(e.EventData.UserId))
                     {
                         MemberList.Items.Remove(VoiceMembers[e.EventData.UserId]);
                         VoiceMembers.Remove(e.EventData.UserId);
                     }
                 });
        }

        private void JoinVoiceChannel(object sender, TappedRoutedEventArgs e)
        {
            //await GatewayManager.Gateway.VoiceStatusUpdate(Id, App.CurrentGuildId, true, false);
        }

        public void UpdateHidden()
        {
            if ((IsMuted || !((Storage.Settings.collapseOverride == CollapseOverride.Unread && IsUnread) || (Storage.Settings.collapseOverride == CollapseOverride.Mention && NotificationCount > 0))) && !IsSelected)
            {
                if (IsHidden)
                {
                    if (Type == 4)
                        Chevron.Rotate(-90, 7, 7, 400, 0, EasingType.Circle).Start();
                    else
                    {
                        //await this.Fade(0, 200, 0).StartAsync();
                        this.Visibility = Visibility.Collapsed;
                    }

                }
                else
                {
                    if (Type == 4)
                        Chevron.Rotate(0, 7, 7, 400, 0, EasingType.Circle).Start();
                    else
                    {
                        this.Visibility = Visibility.Visible;
                        this.Fade(1, 200, 0).Start();
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

        private void OpenMenuFlyout(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if(e.PointerDeviceType != PointerDeviceType.Touch)
                switch (Type)
                {
                    case 0: /*Text*/
                        App.ShowMenuFlyout(this, FlyoutManager.Type.TextChn, Id, App.CurrentGuildId, e.GetPosition((this)));
                        break;
                    case 1: /*DM*/
                        App.ShowMenuFlyout(this, FlyoutManager.Type.DMChn, Id, null, e.GetPosition(this));
                        break;
                    case 2: /*Voice*/

                        break;
                    case 3: /*Group*/
                        App.ShowMenuFlyout(this, FlyoutManager.Type.GroupChn, Id, App.CurrentGuildId, e.GetPosition(this));
                        break;
                }
        }

        private void OpenMenuFlyout(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            try
            {
                if (e.HoldingState == HoldingState.Started)
                    switch (Type)
                    {
                        case 0: /*Text*/
                            App.ShowMenuFlyout(this, FlyoutManager.Type.TextChn, Id, App.CurrentGuildId, e.GetPosition(this));
                            break;
                        case 1: /*DM*/
                            App.ShowMenuFlyout(this, FlyoutManager.Type.DMChn, Id, null, e.GetPosition(this));
                            break;
                        case 2: /*Voice*/
                            break;
                        case 3: /*Group*/
                            App.ShowMenuFlyout(this, FlyoutManager.Type.GroupChn, Id, App.CurrentGuildId, e.GetPosition(this));
                            break;
                    }
            }
            catch { }
        }

        private void HideBadge_Completed(object sender, object e)
        {
            //NotificationBorder.Visibility = Visibility.Collapsed;
        }

        private void UserControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            App.UniversalPointerDown(e);
        }
    }
}
