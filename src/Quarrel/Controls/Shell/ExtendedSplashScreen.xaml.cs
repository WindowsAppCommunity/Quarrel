using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quarrel.Messages.Navigation.SubFrame;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.Shell
{
    public sealed partial class ExtendedSplashScreen : UserControl
    {
        public ExtendedSplashScreen()
        {
            this.InitializeComponent();
            Messenger.Default.Register<GatewayReadyMessage>(this, async _ => 
            {
                await DispatcherHelper.RunAsync(() => 
                {
                    LoadOut.Begin();
                });
            });
        }

        public void InitializeAnimation(SplashScreen ogSplash)
        {
            var platformFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

            if (platformFamily != "Windows.Xbox")
            {
                // Setup icon
                AdjustSize(ogSplash.ImageLocation);
            }
            else
            {
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1920, 1080));
                var size = new Size((int)Math.Max(bounds.Width * scaleFactor, 1920), (int)Math.Max(bounds.Height * scaleFactor, 1080));
                AdjustSize(new Rect(0,0, size.Width, size.Height));
            }

            Animation.Begin();
        }


        /// <summary>
        /// Adjust ViewBox size
        /// </summary>
        public void AdjustSize(Rect rect)
        {
            try
            {
                var location = rect;
                viewbox.Width = location.Width;
                viewbox.Height = location.Height;

                //this.Focus(FocusState.Pointer);
                //stack.Margin = new Thickness(0, location.Bottom, 0, 0);
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
