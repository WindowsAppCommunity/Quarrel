using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class LoadingControl : UserControl
    {
        /// <summary>
        /// Cure message
        /// </summary>
        public string Message
        { get => MessageBlock.Text; set => MessageBlock.Text = value; }

        /// <summary>
        /// Loading status
        /// </summary>
        public string Status
        { get => StatusBlock.Text; set => UpdateStatus(value); }

        private async void UpdateStatus(string val)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                // Update the status text
                StatusBlock.Text = val;
            });
       }
        public LoadingControl()
        {
            InitializeComponent();

            // Initialize animation
            initialize();

            // Timer for assuming issues
            DispatcherTimer timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(6)};
            timer.Tick += ShowReset;
            timer.Start();

            // Event Subscriptions
            App.Splash.Dismissed += Splash_Dismissed;
        }

        /// <summary>
        /// Adjust the size of SVG image
        /// </summary>
        private async void Splash_Dismissed(SplashScreen sender, object args)
        {
            if (App.shareop == null)
            {
                // Run on UI thread
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, AdjustSize);
            }
            else
            {
                AdjustSize();
            }
        }

        /// <summary>
        /// Display Reset button
        /// </summary>
        private void ShowReset(object sender, object e)
        {
            ResetButton.Visibility = Visibility.Visible;
            ResetButton.Fade(1).Start();
        }

        /// <summary>
        /// Set up display
        /// </summary>
        public async void initialize()
        {
            // Setup icon
            AdjustSize();
            Animation.Begin();

            // Hide message if applicable
            if (!Storage.Settings.ShowWelcomeMessage)
            {
                MessageBlock.Visibility = Visibility.Collapsed;
                return;
            }


            // Setup message
            var message = await EntryMessages.GetMessage();

            #region Changes by message
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

            // Event Subscriptions
            App.StatusChangedHandler += App_StatusChangedHandler;
        }
        
        /// <summary>
        /// Update the status
        /// </summary>
        /// <param name="e">New status</param>
        private void App_StatusChangedHandler(object sender, string e)
        {
            MessageBlock.Text = e;
        }

        /// <summary>
        /// Adjust ViewBox size
        /// </summary>
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

        /// <summary>
        /// Show Control
        /// </summary>
        /// <param name="animate">Fade in</param>
        public async void Show(bool animate)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Visibility = Visibility.Visible;
                if (animate) LoadIn.Begin();
            });
        }

        /// <summary>
        /// Hide entire Controls
        /// </summary>
        /// <param name="animate">Fade out</param>
        public async void Hide(bool animate)
        {
            /// Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (animate)
                {
                    LoadOut.Begin();
                }
                else Visibility = Visibility.Collapsed;
            });

        }

        /// <summary>
        /// Finish Load in
        /// </summary>
        private void LoadIn_Completed(object sender, object e)
        {
            Animation.Begin();
        }

        /// <summary>
        /// Finish Load out
        /// </summary>
        private void LoadOut_Completed(object sender, object e)
        {
            Visibility = Visibility.Collapsed;
            Animation.Stop();
        }

        /// <summary>
        /// Adjust size when size changes (duh)
        /// </summary>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustSize();
        }
        
        /// <summary>
        /// Prompt app reset
        /// </summary>
        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            await App.RequestReset();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dipose()
        {
            App.Splash.Dismissed -= Splash_Dismissed;
            App.StatusChangedHandler -= App_StatusChangedHandler;
        }
    }
}
