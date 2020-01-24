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
            Messenger.Default.Register<StartUpStatusMessage>(this, async m =>
            {
                StatusBlock.Text = m.Status.ToString().ToUpper();
            });

            Messenger.Default.Register<GatewayReadyMessage>(this, async _ => 
            {
                await DispatcherHelper.RunAsync(() => 
                {
                    LoadOut.Begin();
                });
            });

            LoadMessage();
        }

        public async void LoadMessage()
        {
            var splash = await Helpers.Constants.FromFile.GetRandomSplash();
            MessageBlock.Text = splash.Text;
            CreditBlock.Text = splash.Credit;
        }

        public void InitializeAnimation(SplashScreen ogSplash)
        {
            // Setup icon
            AdjustSize(ogSplash);
            Animation.Begin();
        }


        /// <summary>
        /// Adjust ViewBox size
        /// </summary>
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
