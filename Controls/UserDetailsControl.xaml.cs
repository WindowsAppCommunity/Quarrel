using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.UI.Animations;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Xaml.Hosting;
using System.Numerics;

namespace Discord_UWP.Controls
{
    public sealed partial class UserDetailsControl : UserControl
    {
        /// <summary>
        /// Data for displayed member
        /// </summary>
        public GuildMember DisplayedMember
        {
            get => (GuildMember)GetValue(DisplayedMemberProperty);
            set => SetValue(DisplayedMemberProperty, value);
        }
        public static readonly DependencyProperty DisplayedMemberProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(GuildMember),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// True if the Control is in the Right Panel for a DM
        /// </summary>
        public bool DMPane
        {
            get => (bool)GetValue(DMPaneProperty);
            set => SetValue(DMPaneProperty, value);
        }
        public static readonly DependencyProperty DMPaneProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(bool),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// Not documented
        /// </summary>
        public bool Webhook
        {
            get => (bool)GetValue(WebhookProperty);
            set => SetValue(WebhookProperty, value);
        }
        public static readonly DependencyProperty WebhookProperty = DependencyProperty.Register(
            nameof(DisplayedMember),
            typeof(bool),
            typeof(UserDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as UserDetailsControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == WebhookProperty)
            {
                // Hide avatar and user details for webhook 
                if (Webhook)
                {
                    Row1Grid.Visibility = Visibility.Collapsed;
                    avatarButton.IsEnabled = false;
                }
                else
                {
                    Row1Grid.Visibility = Visibility.Visible;
                    avatarButton.IsEnabled = true;
                }
            }
            if (prop == DisplayedMemberProperty)
            {
                // Update user
                var user = DisplayedMember.User;

                // Hide SendDM if current user
                if (user.Id == LocalState.CurrentUser.Id)
                {
                    SendDM.Visibility = Visibility.Collapsed;
                }

                // If user has a nickname
                if (DisplayedMember.Nick != null)
                {
                    // Show nickname
                    UserStacker.Opacity = 0.5;
                    UserStacker.Margin = new Thickness(0, 0, 0, 20);
                    Nick.Text = DisplayedMember.Nick;
                } 
                else
                {
                    // Hide nickname and show Username#discriminator in place
                    UserStacker.Opacity = 1;
                    UserStacker.Margin = new Thickness(0, 0, 0, 20);
                    Username.FontSize = 16;
                    Username.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                    Discriminator.FontSize = 14;
                    Nick.Visibility = Visibility.Collapsed;
                }

                // Assign Username text
                Username.Text = user.Username;
                Discriminator.Text = "#" + user.Discriminator;

                // Set Avatar Icon
                var imageURL = Common.AvatarUri(user.Avatar, user.Id);
                Avatar.ImageSource = new BitmapImage(imageURL);
                var image = new BitmapImage(Common.AvatarUri(user.Avatar, user.Id));
                Avatar.ImageSource = image;

                // Set blurred background 
                SetupComposition(imageURL);

                // Handle null avatars 
                AvatarBG.Fill = user.Avatar == null ? Common.DiscriminatorColor(user.Discriminator) : Common.GetSolidColorBrush("#00000000");

                // Check presence
                if (LocalState.PresenceDict.ContainsKey(user.Id))
                {
                    if (LocalState.PresenceDict[user.Id].Status != null && LocalState.PresenceDict[user.Id].Status != "invisible")
                        rectangle.Fill = (SolidColorBrush)App.Current.Resources[LocalState.PresenceDict[user.Id].Status];
                    else if (LocalState.PresenceDict[user.Id].Status == "invisible")
                        rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];
                } else
                {
                    rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];
                }

                // Roles
                if (DisplayedMember.JoinedAt.Ticks != 0)
                {
                    if (!DisplayedMember.Roles.Any())
                    {
                     //   RoleHeader.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        var roles = LocalState.Guilds[App.CurrentGuildId].roles;
                        foreach (var roleStr in DisplayedMember.Roles)
                        {
                            // Load role
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
                   // RoleHeader.Visibility = Visibility.Collapsed;
                    RoleWrapper.Visibility = Visibility.Collapsed;
                }

                if (LocalState.Notes.ContainsKey(DisplayedMember.User.Id))
                    Note.Text = LocalState.Notes[DisplayedMember.User.Id];
                else
                    Note.Text = "";

                // Check Rich Presense status
                if (LocalState.PresenceDict.ContainsKey(DisplayedMember.User.Id))
                {
                    if(LocalState.PresenceDict[DisplayedMember.User.Id].Game != null)
                    {
                       // PlayingHeader.Visibility = Visibility.Visible;
                        richPresence.GameContent = LocalState.PresenceDict[DisplayedMember.User.Id].Game;
                        richPresence.Visibility = Visibility.Visible;
                        SolidColorBrush color = (SolidColorBrush)Application.Current.Resources["Blurple"];
                        switch (LocalState.PresenceDict[DisplayedMember.User.Id].Game.Type)
                        {
                            case 1:
                                {
                                    //streaming
                                    color = new SolidColorBrush(Color.FromArgb(255, 100, 65, 164));
                                    break;
                                }
                            case 2:
                                {
                                    //spotify
                                    color = new SolidColorBrush(Color.FromArgb(255, 30, 215, 96));
                                    break;
                                }
                        }
                        if (LocalState.PresenceDict[DisplayedMember.User.Id].Game.ApplicationId == "438122941302046720")
                        {
                            //xbox
                            color = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                        }
                        ChangeAccentColor(color);
                    }
                    else 
                    {
                        richPresence.Visibility = Visibility.Collapsed;
                        if (Storage.Settings.DerivedColor)
                        {
                            Color? color = await App.getUserColor(user);
                            if (color.HasValue)
                            {
                                ChangeAccentColor(new SolidColorBrush(color.Value));
                            }
                        }
                    }
                }
                else
                {
                    richPresence.Visibility = Visibility.Collapsed;
                    if (Storage.Settings.DerivedColor)
                    {
                        Color? color = await App.getUserColor(user);
                        if (color.HasValue)
                        {
                            ChangeAccentColor(new SolidColorBrush(color.Value));
                        }
                    }
                }
                
            }
            if (prop == DMPaneProperty)
            {
                if (DMPane)
                {
                    SendDM.Visibility = Visibility.Collapsed;
                    borderColor.Width = 228;
                    borderColor.CornerRadius = new CornerRadius(0);
                    borderColor.BorderThickness = new Thickness(0);
                    Nick.Margin = new Thickness(12, 12, 0, 0);
                    UserStacker.Margin = new Thickness(12, 6, 0, 12);
                    Username.FontSize = 14;
                    Discriminator.FontSize = 12;
                    profileGrid.Margin = new Thickness(12, 24, 0, 0);
                    Row1Grid.Background = new SolidColorBrush(Colors.Transparent);
                } else
                {
                   //Not actually necessary, because there is absolutely no risk of the control getting recycled in a different situation
                }
            }
        }
        private void ChangeAccentColor(SolidColorBrush color)
        {
            PresenceColor.Fill = color;
            borderColor.BorderBrush = color;
        }
        SpriteVisual _imageVisual;
        private void SetupComposition(Uri imageURL)
        {
            try
            {
                CompositionSurfaceBrush _imageBrush;

                Compositor _compositor = Window.Current.Compositor;

                _imageBrush = _compositor.CreateSurfaceBrush();
                _imageBrush.Stretch = CompositionStretch.UniformToFill;


                LoadedImageSurface _loadedSurface = LoadedImageSurface.StartLoadFromUri(imageURL);
                _loadedSurface.LoadCompleted += _loadedSurface_LoadCompleted;
                _imageBrush.Surface = _loadedSurface;

                // Apply black and white filter for background
                var saturationEffect = new SaturationEffect
                {
                    Saturation = 0.0f,
                    Source = new CompositionEffectSourceParameter("image")
                };
                var effectFactory = _compositor.CreateEffectFactory(saturationEffect);
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("image", _imageBrush);

                // Apply blur for background
                var blurEffect = new GaussianBlurEffect
                {
                    BlurAmount = 8,
                    Source = new CompositionEffectSourceParameter("image")
                };
                var effectFactory2 = _compositor.CreateEffectFactory(blurEffect);
                var effectBrush2 = effectFactory2.CreateBrush();
                effectBrush2.SetSourceParameter("image", effectBrush);

                _imageVisual = _compositor.CreateSpriteVisual();
                _imageVisual.Brush = effectBrush2;
                _imageVisual.Size = new Vector2(Convert.ToSingle(AvatarContainer.ActualWidth), Convert.ToSingle(AvatarContainer.ActualHeight));

                ElementCompositionPreview.SetElementChildVisual(AvatarContainer, _imageVisual);
            }
            catch
            {
                // I guess it'll just look a little worse
            }
        }

        private void _loadedSurface_LoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
        {
            AvatarContainer.Fade(0.35f,300,0,EasingType.Circle).Start();
        }

        public UserDetailsControl()
        {
            InitializeComponent();
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
            Dispose();
        }

        private async void Gateway_PresenceUpdated(object sender, Gateway.GatewayEventArgs<Presence> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    if (e.EventData.User.Id == DisplayedMember.User.Id)
                    {
                        if (e.EventData.Status != null && e.EventData.Status != "invisible")
                            rectangle.Fill = (SolidColorBrush)App.Current.Resources[e.EventData.Status];
                        else if (e.EventData.Status == "invisible")
                            rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];

                        if (e.EventData.Game != null)
                        {
                            // PlayingHeader.Visibility = Visibility.Visible;
                            richPresence.GameContent = e.EventData.Game;
                            richPresence.Visibility = Visibility.Visible;
                            SolidColorBrush color = (SolidColorBrush)Application.Current.Resources["Blurple"];
                            switch (e.EventData.Game.Type)
                            {
                                case 1:
                                    {
                                        //streaming
                                        color = new SolidColorBrush(Color.FromArgb(255, 100, 65, 164));
                                        break;
                                    }
                                case 2:
                                    {
                                        //spotify
                                        color = new SolidColorBrush(Color.FromArgb(255, 30, 215, 96));
                                        break;
                                    }
                            }
                            if (e.EventData.Game.ApplicationId == "438122941302046720")
                            {
                                //xbox
                                color = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                            }
                            ChangeAccentColor(color);
                        }
                        else
                        {
                            richPresence.Visibility = Visibility.Collapsed;
                            if (Storage.Settings.DerivedColor)
                            {
                                Color? color = await App.getUserColor(DisplayedMember.User);
                                if (color.HasValue)
                                {
                                    ChangeAccentColor(new SolidColorBrush(color.Value));
                                }
                            }
                        }
                    }
                }
                catch{ }
            });
          
        }

        private async void Gateway_UserNoteUpdated(object sender, Gateway.GatewayEventArgs<Gateway.DownstreamEvents.UserNote> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        if(e.EventData.UserId == DisplayedMember.User.Id)
                        Note.Text = e.EventData.Note;
                    });
        }

        private async void SendDirectMessage(object sender, RoutedEventArgs e)
        {
            string channelid = null;
            foreach (var dm in LocalState.DMs)
                if (dm.Value.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == DisplayedMember.User.Id)
                    channelid = dm.Value.Id;
            if (channelid == null)
                channelid = (await RESTCalls.CreateDM(new API.User.Models.CreateDM() { Recipients = new List<string>() { DisplayedMember.User.Id }.AsEnumerable() })).Id;
            if (string.IsNullOrEmpty(channelid)) return;
            App.SelectGuildChannel("@me", channelid, SendDM.Text, true);
        }

        private void FadeIn_ImageOpened(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((Parent is FlyoutPresenter))
            {
                ((Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
            }
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("avatar", FullAvatar);
            App.navImageCache = Avatar.ImageSource;
            App.NavigateToProfile(DisplayedMember.User);
        }

        private async void Note_LostFocus(object sender, RoutedEventArgs e)
        {
            var userid = DisplayedMember.User.Id;
            var note = Note.Text.Trim();
            if (LocalState.Notes.ContainsKey(userid) && note == LocalState.Notes[userid].Trim())
                return;
            if (!LocalState.Notes.ContainsKey(userid) && string.IsNullOrEmpty(note))
                return;
            await Task.Run(async () =>
            {
                await RESTCalls.AddNote(userid, note);
            });
        }

        private void AvatarContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(_imageVisual != null)
            {
                _imageVisual.Size = new Vector2(Convert.ToSingle(AvatarContainer.ActualWidth), Convert.ToSingle(AvatarContainer.ActualHeight));
            }
        }

        public void Dispose()
        {
            SendDM.Send -= SendDirectMessage;
            if (App.GatewayCreated)
            {
                GatewayManager.Gateway.UserNoteUpdated -= Gateway_UserNoteUpdated;
                GatewayManager.Gateway.PresenceUpdated -= Gateway_PresenceUpdated;
                Unloaded -= UserDetailsControl_Unloaded;
            }
           
        }

        private void Avatar_OnImageOpened(object sender, RoutedEventArgs e)
        {
            AvatarRectangle.Fade(1,300, 0, EasingType.Circle).Start();
        }

        private void AvatarRectangle_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(DisplayedMember.User.Avatar, DisplayedMember.User.Id), e.GetPosition(this));
            }
        }

        private void AvatarRectangle_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(DisplayedMember.User.Avatar, DisplayedMember.User.Id), e.GetPosition(this));
            }
        }
    }
}
