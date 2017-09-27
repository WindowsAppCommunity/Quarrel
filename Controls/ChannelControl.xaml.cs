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

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            nameof(Name),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public string UserStatus
        {
            get { return (string)GetValue(UserStatusProperty); }
            set { SetValue(UserStatusProperty, value); }
        }
        public static readonly DependencyProperty UserStatusProperty = DependencyProperty.Register(
            nameof(UserStatus),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

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

            if (prop == UserStatusProperty)
            {
                if (UserStatus != "" && UserStatus != null)
                {
                    Status.Fill = (SolidColorBrush)App.Current.Resources[UserStatus];
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
            if (prop == PlayingProperty)
            {
                if (Playing.HasValue)
                {
                    PlayingBlock.Visibility = Visibility.Visible;
                    switch (Playing.Value.Type)
                    {
                        case 0:
                            PlayingType.Text = App.GetString("/Controls/Playing");
                            break;
                        case 1:
                            PlayingType.Text = App.GetString("/Controls/Streaming");
                            break;
                    }
                    PlayingText.Text = Playing.Value.Name;
                } else
                {
                    PlayingBlock.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == IsUnreadProperty)
            {
                if (IsUnread && !IsMuted)
                {
                    ChannelName.Fade(1,200).Start();
                    UnreadIndicator.Visibility = Visibility.Visible;
                }
                else
                {
                    ChannelName.Fade(0.75f, 200).Start();
                    UnreadIndicator.Visibility = Visibility.Collapsed;
                }
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
            }
            if (prop == NameProperty)
            {
                ChannelName.Text = Name;
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
                if (Type == 0)
                {
                    //TEXT
                    HashtagIcon.Visibility = Visibility.Visible;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                    //Tapped -= JoinVoiceChannel;
                }
                else if(Type == 2)
                {
                    //VOICE
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Visible;
                    Tapped += JoinVoiceChannel;
                }
                else if (Type == 1)
                {
                    //DM
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Visible;
                    ChannelImage.Visibility = Visibility.Visible;
                    Status.Visibility = Visibility.Visible;
                    ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                    //Tapped -= JoinVoiceChannel;
                }
                else if (Type == 3)
                {
                    //GROUP DM
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Visible;
                    ChannelImage.Visibility = Visibility.Visible;
                    Status.Visibility = Visibility.Collapsed;
                    //ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("ms-appx:///Assets/DiscordAssets/groupchat.svg"));

                    if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                        ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_dark.png"));
                    else
                        ChannelImageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/DiscordAssets/Friends_light.png"));

                    ChannelImage.Margin = new Thickness(0,6,6,6);
                    //Tapped -= JoinVoiceChannel;
                }
                else if (Type == 4)
                {
                    //CHANNEL CATEGORY
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                    ChannelImage.Visibility = Visibility.Collapsed;
                    Status.Visibility = Visibility.Collapsed;
                }
            }
            //TODO: Vocie channels
            //if (prop == MembersProperty)
            //{
            //    if (Members != null)
            //    {
            //        foreach (string member in Members)
            //        {
            //            if (LocalState.Guilds[App.CurrentGuildId].members.ContainsKey(member))
            //            {
            //                MemberList.Items.Add(new VoiceMemberControl.SimpleMember() { Member = Storage.Cache.Guilds[App.CurrentGuildId].Members[member] });
            //            }
            //        }
            //        //Debug MemberList.Items.Add(new VoiceMemberControl.SimpleMember() { Member = Storage.Cache.Guilds[App.CurrentGuildId].Members[Storage.Cache.CurrentUser.Raw.Id] });
            //    }

            //    if (MemberList.Items.Count > 0)
            //    {
            //        MemberListEnd.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        MemberListEnd.Visibility = Visibility.Collapsed;
            //    }
            //}
        }

        private void JoinVoiceChannel(object sender, TappedRoutedEventArgs e)
        {
            App.ConnectToVoice(Id, App.CurrentGuildId);
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

        private void HideBadge_Completed(object sender, object e)
        {
            //NotificationBorder.Visibility = Visibility.Collapsed;
        }
    }
}
