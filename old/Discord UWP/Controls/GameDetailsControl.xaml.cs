using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.LocalModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class GameDetailsControl : UserControl
    {
        /// <summary>
        /// ActivityId of the Game
        /// </summary>
        public string GameId
        {
            get { return (string)GetValue(GameIdProperty); }
            set { SetValue(GameIdProperty, value); }
        }
        public static readonly DependencyProperty GameIdProperty = DependencyProperty.Register(
            nameof(GameId),
            typeof(string),
            typeof(GameDetailsControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as GameDetailsControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        /// <summary>
        /// Used for to determine padding on generated buttons
        /// </summary>
        bool firstbutton = true;

        /// <summary>
        /// Add button showing where game can be found
        /// </summary>
        /// <param name="title">Text to display</param>
        /// <param name="uri">Navigation uri to launch game at</param>
        private void AddDistributorButton(string title, string uri)
        {
            var button = new HyperlinkButton();
            button.Style = (Style)Application.Current.Resources["PlainTextHyperlinkStyle"];
            button.Foreground = (SolidColorBrush)Application.Current.Resources["Blurple"];
            button.NavigateUri = new Uri(uri);
            if(firstbutton) button.Margin = new Thickness(0, 6, 0, 0);
            button.Content = title;
            button.FontSize = 12;
            LaunchButtons.Children.Add(button);
            firstbutton = false;
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == GameIdProperty)
            {
                firstbutton = true; //Just to be safe if ever the control is recycled
                if (LocalState.SupportedGames.ContainsKey(GameId))
                {
                    // Get game
                    var game = LocalState.SupportedGames[GameId];

                    // Show Game basic details
                    GameName.Text = game.Name;
                    if (game.Developers != null && game.Developers.Count>0)
                    {
                        DevName.Text = "by " + string.Join(",", game.Developers);
                        DevName.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        DevName.Visibility = Visibility.Collapsed;
                    }

                    // Show Game Icon
                    if (!string.IsNullOrEmpty(game.Icon))
                    {
                        GameIcon.Source = new BitmapImage(new Uri("https://cdn.discordapp.com/game-assets/"+game.Id+"/"+game.Icon+".png"));
                        GameIcon.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        GameIcon.Visibility = Visibility.Collapsed;
                    }

                    // Show game splash
                    if (!string.IsNullOrEmpty(game.Splash))
                    {
                        SetupComposition(new Uri("https://cdn.discordapp.com/game-assets/" + game.Id + "/" + game.Splash + ".png?size=1024"));
                    }

                    // Summary
                    if (!string.IsNullOrEmpty(game.Summary))
                    {
                        GameDescription.Text = game.Summary;
                        GameDescription.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        GameDescription.Visibility = Visibility.Collapsed;
                    }

                    // Launch buttons
                    if (game.DistributorGames != null)
                    {
                        foreach (var distributor in game.DistributorGames)
                        {
                            if (distributor.Distributor == "steam")
                            {
                                if (!string.IsNullOrEmpty(distributor.Sku))
                                    AddDistributorButton("Launch with Steam", "steam://run/" + distributor.Sku);
                            }
                            else if (distributor.Distributor == "battlenet")
                            {
                                if (!string.IsNullOrEmpty(distributor.Sku))
                                    AddDistributorButton("Launch with Battlenet", "battlenet://" + distributor.Sku);
                            }
                            else if (distributor.Distributor == "uplay")
                            {
                                if (!string.IsNullOrEmpty(distributor.Sku))
                                    AddDistributorButton("Launch with Uplay", "uplay://launch/" + distributor.Sku + "/0");
                            }
                            else if (distributor.Distributor == "origin")
                            {
                                if (!string.IsNullOrEmpty(distributor.Sku))
                                    AddDistributorButton("Open in Origin",
                                        "origin2://game/launch/?offerIds=0&title=" + Uri.EscapeUriString(game.Name));
                            }
                        }
                    }

                    // Game News
                    try
                    {
                        var gamenews = await RESTCalls.GetGameNews(new string[] {game.Id});
                        if (gamenews != null && gamenews.Count > 0)
                        {
                            NewsFeed.Visibility = Visibility.Visible;
                            for (var i = 0; i < Math.Min(gamenews.Count, 6); i++)
                            {
                                NewsFeed.Items.Insert(0, gamenews[i]);
                            }

                            NewsFeed.SelectedIndex = 0;
                        }
                        else
                        {
                            NewsFeed.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch
                    {
                        NewsFeed.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        SpriteVisual _imageVisual;

        /// <summary>
        /// Show Compositon effects on image
        /// </summary>
        /// <param name="imageURL">Image to effect</param>
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

                // Saturate
                var saturationEffect = new SaturationEffect
                {
                    Saturation = 1,
                    Source = new CompositionEffectSourceParameter("image")
                };
                var effectFactory = _compositor.CreateEffectFactory(saturationEffect);
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("image", _imageBrush);

                // Blur
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
                _imageVisual.Size = new Vector2(Convert.ToSingle(BackgroundContainer.ActualWidth), Convert.ToSingle(BackgroundContainer.ActualHeight));

                ElementCompositionPreview.SetElementChildVisual(BackgroundContainer, _imageVisual);
            }
            catch
            {
                // I guess it'll just look meh
            }
        }

        /// <summary>
        /// Composition effects completed
        /// </summary>
        private void _loadedSurface_LoadCompleted(LoadedImageSurface sender, LoadedImageSourceLoadCompletedEventArgs args)
        {
            BackgroundContainer.Fade(0.35f, 300, 0, EasingType.Circle).Start();
        }

        public GameDetailsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Adjust image size with Controls
        /// </summary>
        private void BackgroundContainer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_imageVisual != null)
            {
                _imageVisual.Size = new Vector2(Convert.ToSingle(BackgroundContainer.ActualWidth), Convert.ToSingle(BackgroundContainer.ActualHeight));
            }
        }
    }
}
