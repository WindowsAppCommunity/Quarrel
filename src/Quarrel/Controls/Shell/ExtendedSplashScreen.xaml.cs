using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            
            // Status changed
            // Updates status text and ther appropiate actions
            Messenger.Default.Register<StartUpStatusMessage>(this, m =>
            {
                StatusBlock.Text = m.Status.ToString().ToUpper();

                // TODO: Stop Animation
                // Stops animation on failed or offline
                if (m.Status == Status.Failed || m.Status == Status.Offline)
                {
                    // Opens status page
                    if (!_Retry)
                    {
                        SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("DiscordStatusPage");
                    }
                    Animation.Stop();
                }


                // Shows Retry button 
                if (m.Status == Status.Failed)
                    RetryButton.Visibility = Visibility.Visible;
            });
            
            // Finished loading
            // Begins hiding splash
            Messenger.Default.Register<GatewayReadyMessage>(this, async _ => 
            {
                await DispatcherHelper.RunAsync(() => 
                {
                    LoadOut.Begin();
                });
            });

            LoadQuote();
        }

        /// <summary>
        /// Gets a random splash text quote
        /// </summary>
        public async void LoadQuote()
        {
            var splash = await Helpers.Constants.FromFile.GetRandomSplash();
            MessageBlock.Text = splash.Text;
            CreditBlock.Text = splash.Credit;
        }

        /// <summary>
        /// Alligns icon with old splash icon and begins animation
        /// </summary>
        /// <param name="ogSplash">Static splash screen data</param>
        public void InitializeAnimation(SplashScreen ogSplash)
        {
            // Setup icon
            AdjustSize(ogSplash);
            LoadIn.Begin();
        }

        /// <summary>
        /// Adjust ViewBox size
        /// </summary>
        /// <param name="ogSplash">Static splash screen data</param>
        public void AdjustSize(SplashScreen ogSplash)
        {
            try
            {
                var location = ogSplash.ImageLocation;
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
        /// Attempts (again) to try and open a connection to Discord
        /// </summary>
        private async void RetryConnecting(object sender, RoutedEventArgs e)
        {
            _Retry = true;
            RetryButton.Visibility = Visibility.Collapsed;
            string token = (string)await SimpleIoc.Default.GetInstance<ICacheService>()
                .Persistent.Roaming.TryGetValueAsync<object>(Constants.Cache.Keys.AccessToken);
            await SimpleIoc.Default.GetInstance<IDiscordService>().Login(token);
        }

        /// <summary>
        /// Indicates that a connection attempt is a retry
        /// </summary>
        private bool _Retry = false;

        /// <summary>
        /// Shows the Discord Status
        /// </summary>
        private void ShowDiscordStatus(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("DiscordStatusPage");
        }
    }
}
