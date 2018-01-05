﻿using Discord_UWP.SharedModels;
using DiscordAPI.API.Game;
using System;
using System.Collections.Generic;
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
            if (prop == GameContentProperty)
            {
                SmallimgRect.RadiusX = 10;
                SmallimgRect.RadiusY = 10;
                SmallimgRect.Width = 20;
                SmallimgRect.Height = 20;
                SmallimgRect.Margin = new Thickness(0, 0, 5, -5);

                var game = GameContent;
                //Game title
                if(game.Name != null)          
                    GameTB.Text = game.Name;
                else                
                    GameTB.Visibility = Visibility.Collapsed;
                //State
                if (game.State != null)                
                    StateTB.Text = game.State;                
                else               
                    StateTB.Visibility = Visibility.Collapsed;
                //Details
                if(game.Details != null)
                    DetailsTB.Text = game.Details;               
                else               
                    DetailsTB.Visibility = Visibility.Collapsed;

                if (game.Party.HasValue)
                {
                    if(game.Party.Value.Size != null)
                        StateTB.Text += " (" + game.Party.Value.Size[0] + "/" + game.Party.Value.Size[1] + ")";  
                }
                if (game.TimeStamps.HasValue && (game.State!="" || game.Details !="") && (game.TimeStamps.Value.Start.HasValue || game.TimeStamps.Value.End.HasValue))
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
                if(game.Assets.HasValue)
                {
                    //Large image
                    if (game.Assets.Value.LargeImage != null)
                        Largeimg.ImageSource = new BitmapImage(GetImageLink(game.Assets.Value.LargeImage, game.ApplicationId));
                    else
                        LargeImgRect.Visibility = Visibility.Collapsed;
                    //Small image
                    if (game.Assets.Value.SmallImage != null)
                        Smallimg.ImageSource = new BitmapImage(GetImageLink(game.Assets.Value.SmallImage, game.ApplicationId));
                    else
                        SmallimgRect.Visibility = Visibility.Collapsed;
                    //Image tooltips
                    if (game.Assets.Value.LargeText != null)
                        ToolTipService.SetToolTip(LargeImgRect, game.Assets.Value.LargeText);
                    if (game.Assets.Value.SmallImage != null)
                        ToolTipService.SetToolTip(SmallimgRect, game.Assets.Value.SmallText);
                }
                else if(game.Name != null)
                {
                   
                    GameList? gli = LocalModels.LocalState.SupportedGames.FirstOrDefault(x => x.Name == game.Name);
                    if(!gli.HasValue)
                        gli = LocalModels.LocalState.SupportedGames.FirstOrDefault(x => x.Id == game.ApplicationId);
                    if (gli.HasValue)
                    {
                        if (gli.Value.Splash != null) {
                            Largeimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Splash, gli.Value.Id, true, ""));
                            if (gli.Value.Icon != null)
                            {
                                Smallimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Icon, gli.Value.Id, true, ""));
                                SmallimgRect.RadiusX = 4;
                                SmallimgRect.RadiusY = 4;
                                SmallimgRect.Width = 24;
                                SmallimgRect.Height = 24;
                                SmallimgRect.Margin = new Thickness(0, 0, 7, -7);
                            }
                                
                            else
                                SmallimgRect.Visibility = Visibility.Collapsed;
                        }
                        else if(gli.Value.Icon != null)
                        {
                            Largeimg.ImageSource = new BitmapImage(GetImageLink(gli.Value.Icon, gli.Value.Id,true, ""));
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

                }
                else
                {
                    LargeImgRect.Visibility = Visibility.Collapsed;
                    SmallimgRect.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UpdateTimer(object sender, object e)
        {
            if (GameContent.TimeStamps.Value.End.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Value.End.Value);

                var timeleft = t.Subtract(DateTimeOffset.Now);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " left";
            }
            else if (GameContent.TimeStamps.Value.Start.HasValue)
            {
                var t = DateTimeOffset.FromUnixTimeMilliseconds(GameContent.TimeStamps.Value.Start.Value);
                var timeleft = DateTimeOffset.Now.Subtract(t);
                TimeTB.Text = timeleft.ToString(@"mm\:ss") + " elapsed";
            }
        }
        public Uri GetImageLink(string id, string gameid, bool game = false, string append = "?size=512")
        {
            string type = "app";
            if (game) type = "game";
            return new Uri("https://cdn.discordapp.com/"+type+"-assets/" + gameid + "/" + id + ".png"+append);
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
