using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class LoadingControl : UserControl
    {
        public string Message
        { get => MessageBlock.Text; set => MessageBlock.Text = value; }

        public string Status
        { get => StatusBlock.Text; set =>UpdateStatus(value); }
        private async void UpdateStatus(string val)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                StatusBlock.Text = val;
            });
       }
        public LoadingControl()
        {
            InitializeComponent();
            initialize();
            DispatcherTimer timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(6)};
            timer.Tick += ShowReset;
            timer.Start();
            App.Splash.Dismissed += Splash_Dismissed;
        }

        private async void Splash_Dismissed(SplashScreen sender, object args)
        {
            if (App.shareop == null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, AdjustSize);
            }
            else
                AdjustSize();
        }

        private void ShowReset(object sender, object e)
        {
            ResetButton.Visibility = Visibility.Visible;
            ResetButton.Fade(1).Start();
        }

        public async void initialize()
        {
            var message = await EntryMessages.GetMessage();

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

            Animation.Begin();

            if (!Storage.Settings.ShowWelcomeMessage)
            {
                MessageBlock.Visibility = Visibility.Collapsed;
                return;
            }

            #region Special Shit
            switch (message.Key)
            {
                case "Now with Comic Sans":
                    MessageBlock.FontFamily = new FontFamily("Comic Sans MS");
                    break;
            }
            #endregion

            MessageBlock.Text = message.Key.ToUpper();
            if (message.Value != "")
            {
                if (message.Value[0] == '@')
                {
                    CreditBlock.Text = App.GetString("/Main/SubmittedBy") + " " + message.Value;
                }
                else
                {
                    CreditBlock.Text = "-" + message.Value;
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

                //this.Focus(FocusState.Pointer);
                stack.Margin = new Thickness(0, location.Bottom, 0, 0);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public async void Show(bool animate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Visibility = Visibility.Visible;
                if (animate) LoadIn.Begin();
            });
        }
        public async void Hide(bool animate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (animate)
                {
                    LoadOut.Begin();
                }
                else Visibility = Visibility.Collapsed;
            });

        }
        private void LoadIn_Completed(object sender, object e)
        {
            Animation.Begin();
        }

        private void LoadOut_Completed(object sender, object e)
        {
            Visibility = Visibility.Collapsed;
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

        public void Dipose()
        {
            App.Splash.Dismissed -= Splash_Dismissed;
            App.StatusChangedHandler -= App_StatusChangedHandler;
        }
    }
}
