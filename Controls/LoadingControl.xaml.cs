using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
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
    public sealed partial class LoadingControl : UserControl
    {
        public string Message
        { get => MessageBlock.Text; set => MessageBlock.Text = value; }

        public string Status
        { get => StatusBlock.Text; set => StatusBlock.Text = value; }

        public LoadingControl()
        {
            this.InitializeComponent();
            initialize();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(5000);
            timer.Tick += ShowReset;
            timer.Start();
        }

        private void ShowReset(object sender, object e)
        {
            ResetButton.Visibility = Visibility.Visible;
        }

        public void initialize()
        {
            var message = EntryMessages.GetMessage();

            //else if (message.Key.Substring(0, 7) == "(Audio)")
            //{
            //    App.mediaPlayer = new MediaPlayer();
            //    App.mediaPlayer.SystemMediaTransportControls.IsEnabled = false;
            //    App.mediaPlayer.Source = MediaSource.CreateFromUri((new Uri("ms-appx:///Assets/EntryMessages/" + message.Key.Substring(7))));
            //    App.mediaPlayer.Play();
            //    App.mediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
            //    MessageBlock.Visibility = Visibility.Collapsed;
            //}

            var location = App.Splash.ImageLocation;
            viewbox.Width = location.Width;
            
            if (!Storage.Settings.ShowWelcomeMessage)
            {
                MessageBlock.Visibility = Visibility.Collapsed;
                Animation.Begin();
                return;
            }
            if (message.Key.Length>7 && message.Key.Substring(0, 7) == "(Image)")
            {
                Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/EntryMessages/" + message.Key.Substring(7)));
                MessageBlock.Visibility = Visibility.Collapsed;
                viewbox.Visibility = Visibility.Collapsed;
                Image.Visibility = Visibility.Visible;
                GifIn.Begin();
            }
            else
            {
                Animation.Begin();
            }
            MessageBlock.Text = message.Key.ToUpper();
            if (message.Value != "")
                CreditBlock.Text = App.GetString("/Main/SubmittedBy") + " " + message.Value;
            Animation.Begin();
            App.StatusChangedHandler += App_StatusChangedHandler;

        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.PlaybackSession != null && sender.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                sender.Dispose();
            }
        }

        private void App_StatusChangedHandler(object sender, string e)
        {
            MessageBlock.Text = e;
        }

        public void AdjustSize()
        {
            var location = App.Splash.ImageLocation;
            viewbox.Width = location.Width;
            //this.Focus(FocusState.Pointer);
            //stack.Margin = new Thickness(0, location.Top - 32, 0, 0);
        }
        public void Show(bool animate)
        {
            this.Visibility = Visibility.Visible;
            if(animate) LoadIn.Begin();
        }
        public void Hide(bool animate)
        {
            if (animate)
            {
                //TODO: Shrink Image
                LoadOut.Begin();
            }
            else this.Visibility = Visibility.Collapsed;
        }
        private void LoadIn_Completed(object sender, object e)
        {
            Animation.Begin();
        }

        private void LoadOut_Completed(object sender, object e)
        {
            this.Visibility = Visibility.Collapsed;
            Animation.Stop();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustSize();
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            await App.RequestReset();
        }
    }
}
