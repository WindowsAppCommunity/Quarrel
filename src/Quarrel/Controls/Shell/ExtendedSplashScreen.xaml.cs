using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Messages.Gateway;
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
            // Updates status text
            Messenger.Default.Register<StartUpStatusMessage>(this, m =>
            {
                StatusBlock.Text = m.Status.ToString().ToUpper();
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
    }
}
