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
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

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

        public bool DMPane
        {
            get { return (bool)GetValue(DMPaneProperty); }
            set { SetValue(DMPaneProperty, value); }
        }
        public static readonly DependencyProperty DMPaneProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(bool),
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
                            Visibility ellipseView = Visibility.Visible;
                            if (role.Color == 0)
                                ellipseView = Visibility.Collapsed;
                            else
                            {
                                Border b = new Border()
                                {
                                    CornerRadius = new CornerRadius(10, 10, 10, 10),
                                    BorderThickness = new Thickness(1),
                                    BorderBrush = c,
                                    Margin = new Thickness(2, 2, 2, 2),
                                    Child = new StackPanel()
                                    {
                                        Children =
                                    {
                                        new Ellipse
                                        {
                                            Margin=new Thickness(4,0,0,0),
                                            Fill=c,
                                            Width=11,
                                            Height=11,
                                            Visibility = ellipseView
                                        },
                                        new TextBlock
                                        {
                                            FontSize = 11,
                                            Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"],
                                            Padding = new Thickness(3, 2, 6, 3),
                                            Opacity=0.8,
                                            Text = role.Name
                                        },

                                    },
                                        Orientation = Orientation.Horizontal
                                    },
                                    Tag = roleStr
                                };

                                RoleWrapper.Children.Add(b);
                            }

                        }
                    }
                }
                else
                {
                    RoleHeader.Visibility = Visibility.Collapsed;
                    RoleWrapper.Visibility = Visibility.Collapsed;
                }
                if(LocalState.Notes.ContainsKey(DisplayedMember.User.Id))
                    Note.Text = LocalState.Notes[DisplayedMember.User.Id];

                //TODO: DM Functionality 
                if (LocalState.PresenceDict.ContainsKey(DisplayedMember.User.Id))
                {
                    if(LocalState.PresenceDict[DisplayedMember.User.Id].Game.HasValue)
                    {
                        PlayingHeader.Visibility = Visibility.Visible;
                        richPresence.GameContent = LocalState.PresenceDict[DisplayedMember.User.Id].Game.Value;
                        richPresence.Visibility = Visibility.Visible;
                    }
                }
            }
            if (prop == DMPaneProperty)
            {
                if (DMPane)
                {
                    SendDM.Visibility = Visibility.Collapsed;
                    mainGrid.Width = 200;
                } else
                {
                    SendDM.Visibility = Visibility.Visible;
                    mainGrid.Width = 248;
                }
            }
        }

        public UserDetailsControl()
        {
            this.InitializeComponent();
            SendDM.Send += SendDirectMessage;
            if (App.GatewayCreated)
            {
                GatewayManager.Gateway.UserNoteUpdated += Gateway_UserNoteUpdated;
                GatewayManager.Gateway.PresenceUpdated += Gateway_PresenceUpdated;
            }
            Unloaded += UserDetailsControl_Unloaded;
        }

        private void UserDetailsControl_Unloaded(object sender, RoutedEventArgs e)
        {
            GatewayManager.Gateway.UserNoteUpdated -= Gateway_UserNoteUpdated;
            GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
            Unloaded -= UserDetailsControl_Unloaded;
        }

        private async void Gateway_PresenceUpdated(object sender, Gateway.GatewayEventArgs<Presence> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        try
                {
                    if (e.EventData.User.Id == DisplayedMember.User.Id)
                    {
                        if (e.EventData.Game.HasValue)
                        {
                            PlayingHeader.Visibility = Visibility.Visible;
                            var game = e.EventData.Game.Value;
                            richPresence.GameContent = game;
                            richPresence.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            PlayingHeader.Visibility = Visibility.Collapsed;
                            richPresence.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                catch{ }
            });
          
        }

        private async void Gateway_UserNoteUpdated(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if(e.EventData.UserId == DisplayedMember.User.Id)
                        Note.Text = e.EventData.Note;
                    });
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
            if ((Parent is FlyoutPresenter))
            {
                ((Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
            }
            App.NavigateToProfile(DisplayedMember.User);
        }

        private async void Note_LostFocus(object sender, RoutedEventArgs e)
        {
            var userid = DisplayedMember.User.Id;
            var note = Note.Text;
            await Task.Run(async () =>
            {
                await RESTCalls.AddNote(userid, note);
            });
        }
    }
}
