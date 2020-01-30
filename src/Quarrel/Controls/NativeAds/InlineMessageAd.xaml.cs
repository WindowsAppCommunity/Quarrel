using Microsoft.Advertising.WinRT.UI;
using Quarrel.ViewModels.Helpers;
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
using Windows.UI.Xaml.Navigation;

namespace Quarrel.Controls.NativeAds
{
    /// <summary>
    /// Ad "disguised" as message inline with MessageList
    /// </summary>
    public sealed partial class InlineMessageAd : UserControl
    {
        /// <summary>
        /// Create AdTemplate and begin loading ad
        /// </summary>
        public InlineMessageAd()
        {
            this.InitializeComponent();

            nativeAdsManager.AdReady += NativeAdsManager_AdReady;
            nativeAdsManager.ErrorOccurred += NativeAdsManager_ErrorOccurred;
            nativeAdsManager.RequestAd();
        }

        /// <summary>
        /// Ad Data
        /// </summary>
        public NativeAdV2 ViewModel => DataContext as NativeAdV2;

        /// <summary>
        /// Ad Manager for loading ads
        /// </summary>
        private NativeAdsManagerV2 nativeAdsManager = new NativeAdsManagerV2(Constants.Store.AppId, Constants.Store.NativeAdId);

        /// <summary>
        /// Dipslays Ad once loaded
        /// </summary>
        private async void NativeAdsManager_AdReady(object sender, NativeAdReadyEventArgs e)
        {
            DataContext = e.NativeAd;

            await Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.Bindings.Update();
            });

            e.NativeAd.RegisterAdContainer(rootGrid);
        }

        /// <summary>
        /// Hides template if error occurs
        /// </summary>
        private void NativeAdsManager_ErrorOccurred(object sender, NativeAdErrorEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
