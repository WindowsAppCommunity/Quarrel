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
        public Game? GameContent
        {
            get { return (Game?)GetValue(GameContentProperty); }
            set { SetValue(GameContentProperty, value); }
        }
        public static readonly DependencyProperty GameContentProperty = DependencyProperty.Register(
            nameof(GameContent),
            typeof(Game?),
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
            if (prop == GameContentProperty && GameContent.HasValue)
            {
                SmallimgRect.RadiusX = 10;
                SmallimgRect.RadiusY = 10;
                SmallimgRect.Width = 20;
                SmallimgRect.Height = 20;
                SmallimgRect.Margin = new Thickness(0, 0, 5, -5);

                Game? game = GameContent;

                if (game.Value.ApplicationId == "438122941302046720")
                    xboxlogo.Visibility = Visibility.Visible;
                else
                    xboxlogo.Visibility = Visibility.Collapsed;
                if (game.Value.Type != 2)
                {
                    //Game title

                    if (game.Value.Name != null)
                        GameTB.Text = game.Value.Name;
                    else
                        GameTB.Visibility = Visibility.Collapsed;
                    //State
                    if (game.Value.State != null)
                        StateTB.Text = game.Value.State;
                    else
                        StateTB.Visibility = Visibility.Collapsed;
                    //Details
                    if (game.Value.Details != null)
                        DetailsTB.Text = game.Value.Details;
                    else
                        DetailsTB.Visibility = Visibility.Collapsed;

                    if (game.Value.Party.HasValue)
                    {
                        if (game.Value.Party.Value.Size != null)
                            StateTB.Text += " (" + game.Value.Party.Value.Size[0] + "/" + game.Value.Party.Value.Size[1] + ")";
                    }
                    if (game.Value.TimeStamps.HasValue && (game.Value.State != "" || game.Value.Details != "") && (game.Value.TimeStamps.Value.Start.HasValue || game.Value.TimeStamps.Value.End.HasValue))
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
                    if (game.Value.Assets.HasValue)
                    {
                        //Large image
                        if (game.Value.Assets.Value.LargeImage != null)
                            Largeimg.ImageSource = new BitmapImage(GetImageLink(game.Value.Assets.Value.LargeImage, game.Value.ApplicationId));
                        else
                            LargeImgRect.Visibility = Visibility.Collapsed;
                        //Small image
                        if (game.Value.Assets.Value.SmallImage != null)
                            Smallimg.ImageSource = new BitmapImage(GetImageLink(game.Value.Assets.Value.SmallImage, game.Value.ApplicationId));
                        else
                            SmallimgRect.Visibility = Visibility.Collapsed;
                        //Image tooltips
                        if (game.Value.Assets.Value.LargeText != null)
                            ToolTipService.SetToolTip(LargeImgRect, game.Value.Assets.Value.LargeText);
                        if (game.Value.Assets.Value.SmallImage != null)
                            ToolTipService.SetToolTip(SmallimgRect, game.Value.Assets.Value.SmallText);
                    }
                    else
                    {
                        LargeImgRect.Visibility = Visibility.Collapsed;
                        SmallimgRect.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (game.Value.Assets.HasValue)
                    {
                        if (game.Value.Assets.Value.LargeImage != null)
                            Largeimg.ImageSource = new BitmapImage(GetSpotifyImageLink(game.Value.Assets.Value.LargeImage));
                    }
                    else
                    {
                        LargeImgRect.Visibility = Visibility.Collapsed;
                        SmallimgRect.Visibility = Visibility.Collapsed;
                    }
                    //Artist = state
                    //Album = large_text
                    //Song = details
                    if (game.Value.Details != null)
                        GameTB.Text = game.Value.Details;
                    if (game.Value.State != null)
                        DetailsTB.Text = "by " + game.Value.State;
                    if (game.Value.Assets != null && game.Value.Assets.Value.LargeText != null)
                        StateTB.Text =  game.Value.Assets.Value.LargeText;
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

           

            /* else if (game.Value.Name != null)
             {

                 GameList? gli = LocalModels.LocalState.SupportedGames.FirstOrDefault(x => x.Name == game.Value.Name);
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
            if (GameContent.Value.TimeStamps.Value.End.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.Value.TimeStamps.Value.End.Value);

                var timeleft = t.Subtract(DateTimeOffset.Now);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " left";
            }
            else if (GameContent.Value.TimeStamps.Value.Start.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.Value.TimeStamps.Value.Start.Value);
                var timeleft = DateTimeOffset.Now.Subtract(t);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " elapsed";
            }
        }
        private void UpdateProgressBar(object sender, object e)
        {
            var st = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.Value.TimeStamps.Value.Start.Value);
            var et = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.Value.TimeStamps.Value.End.Value);

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
    }
}
