// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Gateway.DownstreamEvents;
using DiscordAPI.SharedModels;

namespace Quarrel.Controls
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

                // Check basic presence
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

                // If the user has any roles
                if (DisplayedMember.JoinedAt.Ticks != 0 && DisplayedMember.Roles.Any())
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
                else
                {
                    // Hide Role Presenter
                    RoleWrapper.Visibility = Visibility.Collapsed;
                }

                // Load notes
                if (LocalState.Notes.ContainsKey(DisplayedMember.User.Id))
                    Note.Text = LocalState.Notes[DisplayedMember.User.Id];
                else
                    Note.Text = "";

                // Check Rich Presense status 
                if (LocalState.PresenceDict.ContainsKey(DisplayedMember.User.Id) && LocalState.PresenceDict[DisplayedMember.User.Id].Game != null)
                {
                    // Initialize richPresense display control
                    richPresence.GameContent = LocalState.PresenceDict[DisplayedMember.User.Id].Game;
                    richPresence.Visibility = Visibility.Visible;

                    // Determine control accent color
                    SolidColorBrush color = (SolidColorBrush)Application.Current.Resources["Blurple"];
                    switch (LocalState.PresenceDict[DisplayedMember.User.Id].Game.Type)
                    {
                        case 1:
                            {
                                // Streaming
                                color = new SolidColorBrush(Color.FromArgb(255, 100, 65, 164));
                                break;
                            }
                        case 2:
                            {
                                // Spotify
                                color = new SolidColorBrush(Color.FromArgb(255, 30, 215, 96));
                                break;
                            }
                    }
                    if (LocalState.PresenceDict[DisplayedMember.User.Id].Game.ApplicationId == "438122941302046720")
                    {
                        // Xbox
                        color = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                    }
                    ChangeAccentColor(color);
                }
                else
                {
                    // User has no presence or is not playing a game
                    richPresence.Visibility = Visibility.Collapsed;
                    if (Storage.Settings.DerivedColor)
                    {
                        // Get color by avatar
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
                    // Remove DM block
                    SendDM.Visibility = Visibility.Collapsed;
                    
                    // Adjust allignment
                    borderColor.Width = 228; // 228 is the right-panel width
                    borderColor.CornerRadius = new CornerRadius(0); // No corners
                    borderColor.BorderThickness = new Thickness(0); // No borders

                    // Adjust content layout
                    Nick.Margin = new Thickness(12, 12, 0, 0);
                    UserStacker.Margin = new Thickness(12, 6, 0, 12);
                    Username.FontSize = 14;
                    Discriminator.FontSize = 12;
                    profileGrid.Margin = new Thickness(12, 24, 0, 0);

                    // Adjust color
                    Row1Grid.Background = new SolidColorBrush(Colors.Transparent);
                } else
                {
                   // Not actually necessary, because there is absolutely no risk of the control getting recycled in a different situation
                }
            }
        }

        /// <summary>
        /// Changes the accent color of the color to <paramref name="color"/>
        /// </summary>
        /// <param name="color">The new accent color for the control</param>
        private void ChangeAccentColor(SolidColorBrush color)
        {
            PresenceColor.Fill = color;
            borderColor.BorderBrush = color;
        }

        SpriteVisual _imageVisual;

        /// <summary>
        /// Setup compositional graphics for background
        /// </summary>
        /// <param name="imageURL">Image URL for the background</param>
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

        /// <summary>
        /// Fade in composition on loading
        /// </summary>
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

        /// <summary>
        /// Dipose control on unloading
        /// </summary>
        private void UserDetailsControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// User presence updated event
        /// </summary>
        private async void Gateway_PresenceUpdated(object sender, GatewayEventArgs<Presence> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    // If the updated user is the current user
                    if (e.EventData.User.Id == DisplayedMember.User.Id)
                    {
                        // Set new basic presence
                        if (e.EventData.Status != null && e.EventData.Status != "invisible")
                            rectangle.Fill = (SolidColorBrush)App.Current.Resources[e.EventData.Status];
                        else if (e.EventData.Status == "invisible")
                            rectangle.Fill = (SolidColorBrush)App.Current.Resources["offline"];

                        // Update game
                        if (e.EventData.Game != null)
                        {
                            // Update rich presence control
                            richPresence.GameContent = e.EventData.Game;
                            richPresence.Visibility = Visibility.Visible;

                            // Determine new accent color
                            SolidColorBrush color = (SolidColorBrush)Application.Current.Resources["Blurple"];
                            switch (e.EventData.Game.Type)
                            {
                                case 1:
                                    {
                                        // Streaming
                                        color = new SolidColorBrush(Color.FromArgb(255, 100, 65, 164));
                                        break;
                                    }
                                case 2:
                                    {
                                        // Spotify
                                        color = new SolidColorBrush(Color.FromArgb(255, 30, 215, 96));
                                        break;
                                    }
                            }
                            if (e.EventData.Game.ApplicationId == "438122941302046720")
                            {
                                // Xbox
                                color = new SolidColorBrush(Color.FromArgb(255, 16, 124, 16));
                            }
                            ChangeAccentColor(color);
                        }
                        else
                        {
                            // There's no game, hide game control
                            richPresence.Visibility = Visibility.Collapsed;
                            if (Storage.Settings.DerivedColor)
                            {
                                // Get accent color by avatar
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

        /// <summary>
        /// User-note updated
        /// </summary>
        private async void Gateway_UserNoteUpdated(object sender, GatewayEventArgs<UserNote> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        // If current user, update note
                        if(e.EventData.UserId == DisplayedMember.User.Id)
                            Note.Text = e.EventData.Note;
                    });
        }

        /// <summary>
        /// Send DM from DM block
        /// </summary>
        private async void SendDirectMessage(object sender, RoutedEventArgs e)
        {
            string channelid = null;
            
            // If user already has DM, use it
            foreach (var dm in LocalState.DMs)
                if (dm.Value.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == DisplayedMember.User.Id)
                    channelid = dm.Value.Id;

            // If the user doesn't have a DM, create one
            if (channelid == null)
                channelid = (await RESTCalls.CreateDM(new DiscordAPI.API.User.Models.CreateDM() { Recipients = new List<string>() { DisplayedMember.User.Id }.AsEnumerable() })).Id;

            // return if failed
            if (string.IsNullOrEmpty(channelid)) return;

            //TODO: The above may be handled by a prefined function, check that
            // Send DM
            App.SelectGuildChannel("@me", channelid, SendDM.Text, true); 
        }

        /// <summary>
        /// Open full profile for user
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Close popup
            if ((Parent is FlyoutPresenter))
            {
                ((Parent as FlyoutPresenter).Parent as Popup).IsOpen = false;
            }

            // Connected animation
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("avatar", FullAvatar);
            App.navImageCache = Avatar.ImageSource;

            // Navigate to subpage
            App.NavigateToProfile(DisplayedMember.User);
        }

        /// <summary>
        /// Update notes when the note block losses focus
        /// </summary>
        private async void Note_LostFocus(object sender, RoutedEventArgs e)
        {
            var userid = DisplayedMember.User.Id;
            var note = Note.Text.Trim();

            // If there's no change, return
            if (LocalState.Notes.ContainsKey(userid) && note == LocalState.Notes[userid].Trim())
                return;

            // The notes are null and there's no notes to overrides, return
            if (!LocalState.Notes.ContainsKey(userid) && string.IsNullOrEmpty(note))
                return;

            // Update note
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

        /// <summary>
        /// Dipose of control
        /// </summary>
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

        /// <summary>
        /// Fade in avatar on load
        /// </summary>
        private void Avatar_OnImageOpened(object sender, RoutedEventArgs e)
        {
            AvatarRectangle.Fade(1,300, 0, EasingType.Circle).Start();
        }

        /// <summary>
        /// Handle right-tap on avatar
        /// </summary>
        private void AvatarRectangle_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // Prompt, "save photo"
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(DisplayedMember.User.Avatar, DisplayedMember.User.Id), e.GetPosition(this));
            }
        }

        /// <summary>
        /// Handle holding on avatar
        /// </summary>
        private void AvatarRectangle_Holding(object sender, HoldingRoutedEventArgs e)
        {
            // Prompt "save photo"
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, Common.AvatarString(DisplayedMember.User.Avatar, DisplayedMember.User.Id), e.GetPosition(this));
            }
        }
    }
}
