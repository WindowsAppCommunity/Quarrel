using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class RichPresenceControl : UserControl
    {
        /// <summary>
        /// Game of Rich Content
        /// </summary>
        public Game GameContent
        {
            get { return (Game)GetValue(GameContentProperty); }
            set { SetValue(GameContentProperty, value); }
        }
        public static readonly DependencyProperty GameContentProperty = DependencyProperty.Register(
            nameof(GameContent),
            typeof(Game),
            typeof(RichPresenceControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// Bool for if the Control is for a small or large display
        /// </summary>
        public bool IsLarge
        {
            get { return IsLarge; }
            set
            {
                if(value == true)
                {
                    LargeImgRect.Width = 104;
                    LargeImgRect.Height = 104;
                    LargeImgRect.RadiusX = 8;
                    LargeImgRect.RadiusY = 8;
                    LargeImgRect.Margin = new Thickness(0, 0, 18, 0);
                    SmallimgRect.Width = 36;
                    SmallimgRect.Height = 36;
                    SmallimgRect.RadiusY = 18;
                    SmallimgRect.RadiusX = 18;
                    SmallimgRect.Margin = new Thickness(0, 0, 9, -9);

                    GameTB.FontSize = 15;
                    DetailsTB.FontSize = 13.333;
                    TimeTB.FontSize = 13.333;
                    StateTB.FontSize = 13.333;
                }
            }
        }

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as RichPresenceControl;
            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        DispatcherTimer timer;
        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == GameContentProperty && GameContent != null)
            {
                SmallimgRect.RadiusX = 10;
                SmallimgRect.RadiusY = 10;
                SmallimgRect.Width = 20;
                SmallimgRect.Height = 20;
                SmallimgRect.Margin = new Thickness(0, 0, 5, -5);

                Game game = GameContent;

                if (game.ApplicationId != null && game.ApplicationId == "438122941302046720")
                    xboxlogo.Visibility = Visibility.Visible;
                else
                    xboxlogo.Visibility = Visibility.Collapsed;
                if (game.Type != 2)
                {
                    //Game title

                    if (game.Name != null)
                        GameTB.Content = game.Name;
                    else
                        GameTB.Visibility = Visibility.Collapsed;
                    //State
                    if (game.State != null)
                        StateTB.Text = game.State;
                    else
                        StateTB.Visibility = Visibility.Collapsed;
                    //Details
                    if (game.Details != null)
                        DetailsTB.Text = game.Details;
                    else
                        DetailsTB.Visibility = Visibility.Collapsed;

                    if (game.Party != null)
                    {
                        if (game.Party.Size != null)
                            StateTB.Text += " (" + game.Party.Size[0] + "/" + game.Party.Size[1] + ")";
                    }
                    if (game.TimeStamps != null && (game.State != "" || game.Details != "") && (game.TimeStamps.Start.HasValue || game.TimeStamps.End.HasValue))
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        UpdateTimer(null, null);
                        timer.Start();
                        timer.Tick += UpdateTimer;
                    }
                    else
                    {
                        TimeTB.Visibility = Visibility.Collapsed;
                    }
                    //Assets
                    if (game.Assets != null)
                    {
                        //Large image
                        if (game.Assets.LargeImage != null)
                            Largeimg.ImageSource = new BitmapImage(GetImageLink(game.Assets.LargeImage, game.ApplicationId));
                        else
                            LargeImgRect.Visibility = Visibility.Collapsed;
                        //Small image
                        if (game.Assets.SmallImage != null)
                            Smallimg.ImageSource = new BitmapImage(GetImageLink(game.Assets.SmallImage, game.ApplicationId));
                        else
                            SmallimgRect.Visibility = Visibility.Collapsed;
                        //Image tooltips
                        if (game.Assets.LargeText != null)
                            ToolTipService.SetToolTip(LargeImgRect, game.Assets.LargeText);
                        if (game.Assets.SmallImage != null)
                            ToolTipService.SetToolTip(SmallimgRect, game.Assets.SmallText);
                    }
                    else
                    {
                        LargeImgRect.Visibility = Visibility.Collapsed;
                        SmallimgRect.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (game.Assets != null)
                    {
                        if (game.Assets.LargeImage != null)
                            Largeimg.ImageSource = new BitmapImage(GetSpotifyImageLink(game.Assets.LargeImage));
                    }
                    else
                    {
                        LargeImgRect.Visibility = Visibility.Collapsed;
                        SmallimgRect.Visibility = Visibility.Collapsed;
                    }
                    //Artist = state
                    //Album = large_text
                    //Song = details
                    if (game.Details != null)
                        GameTB.Content = game.Details;
                    if (game.State != null)
                        DetailsTB.Text = "by " + game.State;
                    if (game.Assets != null && game.Assets.LargeText != null)
                        StateTB.Text =  game.Assets.LargeText;
                    TimeTB.Visibility = Visibility.Collapsed;

                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(1);
                    UpdateProgressBar(null, null);
                    timer.Start();
                    timer.Tick += UpdateProgressBar;
                    spotifyGrid.Visibility = Visibility.Visible;
                }
            }
            else
            {
                LargeImgRect.Visibility = Visibility.Collapsed;
                SmallimgRect.Visibility = Visibility.Collapsed;
            }

            if (GameContent?.ApplicationId != null && LocalModels.LocalState.SupportedGames.ContainsKey(GameContent.ApplicationId))
            {
                GameTB.IsEnabled = true;
            }
            else if (GameContent?.Name != null &&
                     LocalModels.LocalState.SupportedGamesNames.ContainsKey(GameContent.Name))
            {
                GameTB.IsEnabled = true;
                GameContent.ApplicationId = LocalModels.LocalState.SupportedGamesNames[GameContent.Name];
            }
            else
            {
                GameTB.IsEnabled = false;
            }
        }

        /// <summary>
        /// Runs every second to update timer progress bar
        /// </summary>
        private void UpdateTimer(object sender, object e)
        {
            // If the content has an end time
            if (GameContent.TimeStamps.End.HasValue)
            {
                // Get end time
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.End.Value);

                // Get as timespan from now
                var timeleft = t.Subtract(DateTimeOffset.Now);

                // Update time left
                if(timeleft.Hours == 0)
                    TimeTB.Text = timeleft.ToString(@"mm\:ss") + " left";
                else
                    TimeTB.Text = timeleft.ToString(@"hh\:mm\:ss") + " left";
            }
            else if (GameContent.TimeStamps.Start.HasValue)
            {
                // Get start time
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Start.Value);

                // Get as timespan from now
                var timeleft = DateTimeOffset.Now.Subtract(t);

                // Update time taken
                if (timeleft.Hours == 0)
                    TimeTB.Text = timeleft.ToString(@"mm\:ss") + " elapsed";
                else
                    TimeTB.Text = timeleft.ToString(@"hh\:mm\:ss") + " elapsed";
            }
        }

        /// <summary>
        /// Update progress bar if the content has a start and end time
        /// </summary>
        private void UpdateProgressBar(object sender, object e)
        {
            // Has no start value, no update
            if (!GameContent.TimeStamps.Start.HasValue)
                return;

            // Has no end value, no update
            if (!GameContent.TimeStamps.End.HasValue)
                return;

            // Get start and end time
            DateTimeOffset st, et;
            st = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Start.Value);
            et = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.End.Value);

            // Update end time text
            var length = et.Subtract(st);
            EndTime.Text = length.ToString(@"mm\:ss");

            // Update start time text
            var time = DateTimeOffset.Now.Subtract(st);
            StartTime.Text= time.ToString(@"mm\:ss");

            // Update progress bar
            progbar.Maximum = length.TotalMilliseconds;
            progbar.Value = time.TotalMilliseconds;
        }

        /// <summary>
        /// Get Image for game
        /// </summary>
        /// <param name="id">Image Id</param>
        /// <param name="gameid">Game Id</param>
        /// <param name="game">true if the image query if for a game</param>
        /// <param name="append">Query appending data</param>
        /// <returns>Image URL</returns>
        public Uri GetImageLink(string id, string gameid, bool game = false, string append = "?size=512")
        {
            // Set type in query 
            string type = "app";
            if (game) type = "game";

            // Query URL
            return new Uri("https://cdn.discordapp.com/" + type + "-assets/" + gameid + "/" + id + ".png" + append);
        }

        /// <summary>
        /// Get Image from Spotify
        /// </summary>
        /// <param name="id">Spotify song Id</param>
        /// <returns>Image URl</returns>
        public Uri GetSpotifyImageLink(string id)
        {
            // Query URL
            return new Uri("https://i.scdn.co/image/" + id.Remove(0, 8));
        }

        public RichPresenceControl()
        {
            this.InitializeComponent();
            Unloaded += RichPresenceControl_Unloaded;
        }

        /// <summary>
        /// Dispose of timer
        /// </summary>
        private void RichPresenceControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // If the timer isn't null, dispose
            if(timer != null)
            {
                timer.Stop();
                timer.Tick -= UpdateTimer;
            }
        }

        private void Spectate_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void AskToJoin_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        /// <summary>
        /// Dispose of RichPresenceControl
        /// </summary>
        public void Dispose()
        {
            Unloaded -= RichPresenceControl_Unloaded;
        }

        /// <summary>
        /// Show Game Flyout
        /// </summary>
        private void GameTB_OnClick(object sender, RoutedEventArgs e)
        {
            // If avaible, show game flyout
            if (!string.IsNullOrEmpty(GameContent?.ApplicationId))
            {
                App.ShowGameFlyout(sender, GameContent.ApplicationId);
            }
        }
    }
}
