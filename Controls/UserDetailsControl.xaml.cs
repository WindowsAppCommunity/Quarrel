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
using Windows.UI.Xaml.Media.Animation;
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
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Graphics.Effects;
using Windows.UI.Xaml.Hosting;

using System.Numerics;

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
                    UserStacker.Margin = new Thickness(0, 0, 0, 20);
                    Nick.Text = DisplayedMember.Nick;
                } 
                else
                {
                    UserStacker.Opacity = 1;
                    UserStacker.Margin = new Thickness(0, 20, 0, 20);
                    Username.FontSize = 16;
                    Username.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
                    Discriminator.FontSize = 14;
                    Nick.Visibility = Visibility.Collapsed;
                }
                Username.Text = user.Username;
                Discriminator.Text = "#" + user.Discriminator;
                var imageURL = Common.AvatarUri(user.Avatar, user.Id);
                Avatar.ImageSource = new BitmapImage(imageURL);
                var image = new BitmapImage(Common.AvatarUri(user.Avatar, user.Id));
                Avatar.ImageSource = image;

                SetupComposition(imageURL);


                if (user.Avatar == null)
                {
                    AvatarBG.Fill = Common.DiscriminatorColor(user.Discriminator);
                } else
                {
                    AvatarBG.Fill = Common.GetSolidColorBrush("#00000000");
                }

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

                if (DisplayedMember.JoinedAt.Ticks != 0)
                {
                    if (DisplayedMember.Roles.Count() == 0)
                    {
                     //   RoleHeader.Visibility = Visibility.Collapsed;
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
                   // RoleHeader.Visibility = Visibility.Collapsed;
                    RoleWrapper.Visibility = Visibility.Collapsed;
                }
                if(LocalState.Notes.ContainsKey(DisplayedMember.User.Id))
                    Note.Text = LocalState.Notes[DisplayedMember.User.Id];
                
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
                        PresenceColor.Fill = color;
                        ChangeFlyoutBorder(color);
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
                    UserStacker.HorizontalAlignment = HorizontalAlignment.Left;
                    Nick.HorizontalAlignment = HorizontalAlignment.Left;
                    profileGrid.HorizontalAlignment = HorizontalAlignment.Left;
                    Nick.Margin = new Thickness(12, 12, 0, 0);
                    UserStacker.Margin = new Thickness(12, 6, 0, 12);
                    Username.FontSize = 14;
                    Discriminator.FontSize = 12;
                    profileGrid.Margin = new Thickness(12, 24, 0, 0);
                    Row1Grid.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                } else
                {
                   //Not actually necessary, because there is absolutely no risk of the control getting recycled in a different situation
                }
            }
        }
        private void ChangeFlyoutBorder(SolidColorBrush color)
        {
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
                _imageBrush.Surface = _loadedSurface;


                var saturationEffect = new SaturationEffect
                {
                    Saturation = 0.0f,
                    Source = new CompositionEffectSourceParameter("image")
                };
                var effectFactory = _compositor.CreateEffectFactory(saturationEffect);
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("image", _imageBrush);

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
                //Fuck this shit
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        try
                {
                    if (e.EventData.User.Id == DisplayedMember.User.Id)
                    {
                        if (e.EventData.Game != null)
                        {
                           // PlayingHeader.Visibility = Visibility.Visible;
                            var game = e.EventData.Game;
                            richPresence.GameContent = game;
                            richPresence.Visibility = Visibility.Visible;
                        }
                        else
                        {
                          //  PlayingHeader.Visibility = Visibility.Collapsed;
                            richPresence.Visibility = Visibility.Collapsed;
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

        private void SendDirectMessage(object sender, RoutedEventArgs e)
        {
            App.NavigateToDMChannel(DisplayedMember.User.Id, SendDM.Text, true, false, true);
        }

        private void FadeIn_ImageOpened(object sender, RoutedEventArgs e)
        {

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
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("avatar", FullAvatar);
      //      ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("richpresence", richPresence);
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
    }
}
