using Discord_UWP.SharedModels;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class RichPresenceControl : UserControl
    {
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

            /* else if (game.Value.Name != null)
             {

                 GameListItem? gli = LocalModels.LocalState.SupportedGames.FirstOrDefault(x => x.Name == game.Value.Name);
                 if (!gli.HasValue)
                     gli = LocalModels.LocalState.SupportedGames.FirstOrDefault(x => x.Id == game.Value.ApplicationId);
                 if (gli.HasValue)
                 {
                     if (gli.Value.Splash != null)
                     {
                         Largeimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Splash, gli.Value.Id, true, ""));
                         if (gli.Value.Icon != null)
                         {
                             Smallimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Icon, gli.Value.Id, true, ""));
                             if (!IsLarge)
                             {
                                 SmallimgRect.RadiusX = 4;
                                 SmallimgRect.RadiusY = 4;
                                 SmallimgRect.Width = 24;
                                 SmallimgRect.Height = 24;
                                 SmallimgRect.Margin = new Thickness(0, 0, 7, -7);
                             }
                             else
                             {
                                 SmallimgRect.RadiusX = 8;
                                 SmallimgRect.RadiusY = 8;
                                 SmallimgRect.Width = 48;
                                 SmallimgRect.Height = 48;
                                 SmallimgRect.Margin = new Thickness(0, 0, 14, -14);
                             }
                         }

                         else
                             SmallimgRect.Visibility = Visibility.Collapsed;
                     }
                     else if (gli.Value.Icon != null)
                     {
                         Largeimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Icon, gli.Value.Id, true, ""));
                     }
                     else
                     {
                         LargeImgRect.Visibility = Visibility.Collapsed;
                         SmallimgRect.Visibility = Visibility.Collapsed;
                     }

                 }
                 else
                 {
                     LargeImgRect.Visibility = Visibility.Collapsed;
                     SmallimgRect.Visibility = Visibility.Collapsed;
                 }

             }*/



        }

        private void UpdateTimer(object sender, object e)
        {
            if (GameContent.TimeStamps.End.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.End.Value);

                var timeleft = t.Subtract(DateTimeOffset.Now);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " left";
            }
            else if (GameContent.TimeStamps.Start.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Start.Value);
                var timeleft = DateTimeOffset.Now.Subtract(t);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " elapsed";
            }
        }
        private void UpdateProgressBar(object sender, object e)
        {
            DateTimeOffset st, et;
            if (!GameContent.TimeStamps.Start.HasValue)
                return;
            if (!GameContent.TimeStamps.End.HasValue)
                return;
            st = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Start.Value);
            et = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.End.Value);

            //full length
            var length = et.Subtract(st);
            EndTime.Text = length.ToString(@"mm\:ss");

            //time left
            var time = DateTimeOffset.Now.Subtract(st);
            StartTime.Text= time.ToString(@"mm\:ss");

            progbar.Maximum = length.TotalMilliseconds;
            progbar.Value = time.TotalMilliseconds;
        }
        public Uri GetImageLink(string id, string gameid, bool game = false, string append = "?size=512")
        {
            string type = "app";
            if (game) type = "game";
            var uri= new Uri("https://cdn.discordapp.com/" + type + "-assets/" + gameid + "/" + id + ".png" + append);
            return uri;
        }
        public Uri GetSpotifyImageLink(string id)
        {
            return new Uri("https://i.scdn.co/image/" + id.Remove(0, 8));
        }
        public RichPresenceControl()
        {
            this.InitializeComponent();
            Unloaded += RichPresenceControl_Unloaded;
        }

        private void RichPresenceControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if(timer != null)
            {
                timer.Stop();
                timer.Tick -= UpdateTimer;
            }
        }

        private void Spectate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AskToJoin_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Dispose()
        {
            Unloaded -= RichPresenceControl_Unloaded;
        }

        private void GameTB_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GameContent?.ApplicationId))
            {
                App.ShowGameFlyout(sender, GameContent.ApplicationId);
            }
        }
    }
}
