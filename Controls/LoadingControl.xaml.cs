using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
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
            timer.Interval = TimeSpan.FromSeconds(6);
            timer.Tick += ShowReset;
            timer.Start();
            App.Splash.Dismissed += Splash_Dismissed;
        }

        private async void Splash_Dismissed(SplashScreen sender, object args)
        {
            if (App.shareop == null)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
       () =>
       {
           AdjustSize();
       });
            }
            else
                AdjustSize();
        }

        private void ShowReset(object sender, object e)
        {
            ResetButton.Visibility = Visibility.Visible;
            ResetButton.Fade(1).Start();
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

            AdjustSize();

            if (!Storage.Settings.ShowWelcomeMessage)
            {
                MessageBlock.Visibility = Visibility.Collapsed;
                Animation.Begin();
                return;
            }
            else
            {
                Animation.Begin();
            }
            MessageBlock.Text = message.Key.ToUpper();
            if (message.Value != "")
            {
                if (message.Value[0] == '@')
                {
                    CreditBlock.Text = App.GetString("/Main/SubmittedBy") + " " + message.Value;
                }
                else
                {
                    CreditBlock.Text = "- " + message.Value;
                }
            }
            Animation.Begin();
            App.StatusChangedHandler += App_StatusChangedHandler;

        }
        

        private void App_StatusChangedHandler(object sender, string e)
        {
            MessageBlock.Text = e;
        }

        public void AdjustSize()
        {
            try
            {

                var location = App.Splash.ImageLocation;
                viewbox.Width = location.Width;
                viewbox.Height = location.Height;
                Canvas.SetTop(viewbox, location.Top);
                Canvas.SetLeft(viewbox, location.Left);
                //this.Focus(FocusState.Pointer);
                stack.Margin = new Thickness(0, location.Bottom, 0, 0);
            }
            catch (Exception)
            {

            }
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
