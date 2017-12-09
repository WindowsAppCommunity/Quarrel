using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;

namespace Discord_UWP.Controls
{
    public sealed partial class UserDetailsControl : UserControl
    {
        public GuildMember DisplayedMember
        {
            get { return (GuildMember)GetValue(DisplayedMemberProperty); }
            set { SetValue(DisplayedMemberProperty, value); }
        }
        public static readonly DependencyProperty DisplayedMemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(GuildMember),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as UserDetailsControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == DisplayedMemberProperty)
            {
                var user = DisplayedMember.User;
                if (user.Id == LocalState.CurrentUser.Id)
                {
                    SendDM.Visibility = Visibility.Collapsed;
                }
                if (DisplayedMember.Nick != null)
                {
                    UserStacker.Opacity = 0.5;
                    Nick.Text = DisplayedMember.Nick;
                } 
                else
                {
                    UserStacker.Opacity = 1;
                    Nick.Visibility = Visibility.Collapsed;
                }
                Username.Text = user.Username;
                Discriminator.Text = "#" + user.Discriminator;
                var imageURL = Common.AvatarUri(user.Avatar, user.Id);
                Avatar.ImageSource = new BitmapImage(imageURL);
                AvatarBlurred.Source = new BitmapImage(imageURL);
                BackgroundGrid.Blur(8, 0).Start();
                if (user.Avatar != null)
                {
                    var AvatarExtension = ".png";
                    if (user.Avatar.StartsWith("a_")) AvatarExtension = ".gif";
                    var image = new BitmapImage(new Uri("https://cdn.discordapp.com/avatars/" + user.Id + "/" + user.Avatar + AvatarExtension));
                    Avatar.ImageSource = image;
                    AvatarBlurred.Source = image;
                }
                else
                {
                    var image = new BitmapImage(new Uri("ms-appx:///Assets/DiscordIcon.png"));
                    Avatar.ImageSource = image;
                    AvatarBlurred.Source = image;
                }
                if (!App.CurrentGuildIsDM)
                {
                    if (DisplayedMember.Roles.Count() == 0)
                    {
                        RoleHeader.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        var roles = LocalState.Guilds[App.CurrentGuildId].roles;
                        foreach (var roleStr in DisplayedMember.Roles)
                        {
                            var role = roles[roleStr];
                            var c = Common.IntToColor(role.Color);
                            Border b = new Border()
                            {
                                CornerRadius = new CornerRadius(3, 3, 3, 3),
                                Background = new SolidColorBrush(Color.FromArgb(50, c.Color.R, c.Color.G, c.Color.B)),
                                BorderThickness = new Thickness(1),
                                BorderBrush = c,
                                Margin = new Thickness(2, 2, 2, 2),
                                Child = new TextBlock()
                                {
                                    FontSize = 12,
                                    Foreground = c,
                                    Padding = new Thickness(4, 2, 4, 4),
                                    Text = role.Name
                                }
                            };
                            RoleWrapper.Children.Add(b);
                        }
                    }
                }
                else
                {
                    RoleHeader.Visibility = Visibility.Collapsed;
                    RoleWrapper.Visibility = Visibility.Collapsed;
                }
                //TODO: Note functionality and hook it up to the NoteChanged events
                //TODO: DM Functionality
                //TODO: Live status+playing indicator
                //TODO: 
            }
        }

        public UserDetailsControl()
        {
            this.InitializeComponent();
            SendDM.Send += SendDirectMessage;
        }

        private void SendDirectMessage(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel(null, DisplayedMember.User.Id, SendDM.Text, true);
        }

        private void FadeIn_ImageOpened(object sender, RoutedEventArgs e)
        {
            (sender as Image).Fade(0.2f, 200).Start();
        }


        private void AvatarShowMidAnimation()
        {
            //AvatarRectangle.Blur(2, 200, 0).Start();
            //CacheRectangle.Fade(0.6f, 200).Start();
            //ShowProfile.Fade(0.8f, 200).Start();
        }
        private void AvatarShowFullAnimation()
        {
            //AvatarRectangle.Blur(4, 200, 0).Start();
            //CacheRectangle.Fade(1, 200).Start();
            //ShowProfile.Fade(1, 200).Start();
        }
        private void AvatarHideAnimation()
        {
            //AvatarRectangle.Blur(0, 200, 0).Start();
            //CacheRectangle.Fade(0, 200).Start();
            //ShowProfile.Fade(0, 200).Start();
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AvatarShowMidAnimation();
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AvatarHideAnimation();
        }

        private void Button_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            AvatarHideAnimation();
        }

        private void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            AvatarShowFullAnimation();
        }

        private void Button_LostFocus(object sender, RoutedEventArgs e)
        {
            AvatarHideAnimation();
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            AvatarShowMidAnimation();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
            App.NavigateToProfile(DisplayedMember.User);
        }
    }
}
