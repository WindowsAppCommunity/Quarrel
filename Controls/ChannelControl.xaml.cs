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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class ChannelControl : UserControl
    {
        public GuildChannel? GuildChannel
        {
            get { return (GuildChannel?)GetValue(GuildChannelProperty); }
            set { SetValue(GuildChannelProperty, value); }
        }
        public static readonly DependencyProperty GuildChannelProperty = DependencyProperty.Register(
            nameof(GuildChannel),
            typeof(GuildChannel?),
            typeof(ChannelControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));
        
        public DirectMessageChannel? DmChannel
        {
            get { return (DirectMessageChannel?)GetValue(DmChannelProperty); }
            set { SetValue(DmChannelProperty, value); }
        }
        public static readonly DependencyProperty DmChannelProperty = DependencyProperty.Register(
            nameof(DmChannel),
            typeof(DirectMessageChannel?),
            typeof(ChannelControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

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

        public string Playing
        {
            get { return (string)GetValue(PlayingProperty); }
            set { SetValue(PlayingProperty, value); }
        }
        public static readonly DependencyProperty PlayingProperty = DependencyProperty.Register(
            nameof(Playing),
            typeof(string),
            typeof(ChannelControl),
            new PropertyMetadata("", OnPropertyChangedStatic));

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
            if (prop == PlayingProperty)
            {
                if (Playing != "")
                {
                    PlayingBlock.Visibility = Visibility.Visible;
                    PlayingBlock.Text = Playing;
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
            if (prop == GuildChannelProperty && GuildChannel.HasValue)
            {
                ChannelName.Text = GuildChannel.Value.Name;
                ChannelImage.Visibility = Visibility.Collapsed;
                ChannelImageBackdrop.Visibility = Visibility.Collapsed;
                if (GuildChannel.Value.Type.ToLower() == "text")
                {
                    HashtagIcon.Visibility = Visibility.Visible;
                    VoiceIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    HashtagIcon.Visibility = Visibility.Collapsed;
                    VoiceIcon.Visibility = Visibility.Visible;
                }
                if(GuildChannel.Value.Topic != null)
                    ToolTipService.SetToolTip(this, GuildChannel.Value.Topic);
                else
                    ToolTipService.SetToolTip(this, null);
            }
            if (prop == DmChannelProperty && DmChannel.HasValue)
            {
                ChannelImageBackdrop.Visibility = Visibility.Visible;
                ChannelImage.Visibility = Visibility.Visible;
                ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                ChannelImageBrush.ImageSource =
                    new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + DmChannel.Value.Users.FirstOrDefault().Id + "/" +
                                            DmChannel.Value.Users.FirstOrDefault().Avatar + ".png?size=64"));
                ChannelName.Text = DmChannel.Value.Users.FirstOrDefault().Username;
                
                //if (DmChannel.Value.Users.Any() && DmChannel.Value.Users.Count() > 1)
                //{
                //    if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                //        ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("Assets/Friends_white.svg"));
                //    else
                //        ChannelImageBrush.ImageSource = new SvgImageSource(new Uri("Assets/Friends_black.svg"));

                //    ChannelImage.Margin = new Thickness(6, 12, 12, 12);
                //    List<string> channelMembers = new List<string>();
                //    foreach (var user in DmChannel.Value.Users)
                //        channelMembers.Add(user.Username);
                //    ChannelName.Text = string.Join(", ", channelMembers);
                //    PlayingBlock.Visibility = Visibility.Visible;
                //    PlayingBlock.Text = DmChannel.Value.Users.Count().ToString() + " members";
                //}
                //else
                //{
                //    ChannelImage.Margin = new Thickness(0, 6, 6, 6);
                //    ChannelImageBrush.ImageSource =
                //        new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + DmChannel.Value.User.Id + "/" +
                //                                DmChannel.Value.User.Avatar + ".png?size=64"));
                //    ChannelName.Text = DmChannel.Value.User.Username;
                //}
            }
        }

        public ChannelControl()
        {
            this.InitializeComponent();
        }
    }
}
