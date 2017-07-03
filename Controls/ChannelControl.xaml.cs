using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using GuildChannel = Discord_UWP.CacheModels.GuildChannel;

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

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as ChannelControl;

            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == UserStatusProperty)
            {
                //TODO add userstatus features
            }
            if (prop == SubtitleProperty)
            {
                if (Subtitle != "")
                {
                    PlayingBlock.Visibility = Visibility.Visible;
                    PlayingBlock.Text = Subtitle;
                }
                else
                {
                    PlayingBlock.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == IsUnreadProperty)
            {
                if (IsUnread) UnreadIndicator.Visibility = Visibility.Collapsed;
                else UnreadIndicator.Visibility = Visibility.Visible;
            }
            if (prop == IsMutedProperty)
            {
                if (IsMuted) MuteIcon.Visibility = Visibility.Collapsed;
                else MuteIcon.Visibility = Visibility.Visible;
            }
            if (prop == IsTypingProperty)
            {
                if(IsTyping) TypingIndic.Fade(1,200).Start();
                else TypingIndic.Fade(0,200).Start();
            }
            if (prop == NotificationCountProperty)
            {
                if (NotificationCount > 0)
                {
                    NotificationBorder.Visibility = Visibility.Visible;
                    NotificationCounter.Text = NotificationCount.ToString();
                }
                else
                {
                    NotificationBorder.Visibility = Visibility.Collapsed;
                }
            }
            if (prop == NameProperty)
            {
                ChannelName.Text = Name;
            }
            if (prop == ImageUrlProperty)
            {
                ChannelImageBrush.ImageSource = new BitmapImage(new Uri(ImageUrl));
            }
            if (prop == TypeProperty)
            {

                if (Type == 0)
                {
                    //TEXT
                    HashtagIcon.Visibility = Visibility.Visible;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                }
                else if(Type == 2)
                {
                    //VOICE
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Visible;
                }
                else if (Type == 1)
                {
                    //DM
                    ChannelImageBackdrop.Visibility = Visibility.Visible;
                    ChannelImage.Visibility = Visibility.Visible;
                    ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                }
                else if (Type == 3)
                {
                    //GROUP DM
                    ChannelImageBackdrop.Visibility = Visibility.Visible;
                    ChannelImage.Visibility = Visibility.Visible;

                    if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                        ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("Assets/Friends_white.svg"));
                    else
                        ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("Assets/Friends_black.svg"));

                    ChannelImage.Margin = new Thickness(6, 12, 12, 12);
                }
            }
        }

        public ChannelControl()
        {
            this.InitializeComponent();
        }
    }
}
