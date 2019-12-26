using Microsoft.Advertising.WinRT.UI;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls.NativeAds
{
    public sealed partial class NativeAdContainer : UserControl
    {
        public NativeAdContainer()
        {
            this.InitializeComponent();

            adsManager.AdReady += AdsManager_AdReady;
            adsManager.RequestAd();
        }

        //NativeAdsManagerV2 adsManager = new NativeAdsManagerV2("d25517cb-12d4-4699-8bdc-52040c712cab", "test");
        NativeAdsManagerV2 adsManager = new NativeAdsManagerV2(Helpers.Constants.Store.AppId, Helpers.Constants.Store.AppId);

        private void AdsManager_AdReady(object sender, NativeAdReadyEventArgs e)
        {
            Ad = e.NativeAd;
            LoadAd();
            Ad.RegisterAdContainer(this);
        }

        private void LoadAd()
        {
            if (Ad.AdIcon != null)
            {
                AdIconImage.Source = Ad.AdIcon.Source;

                // Adjust the Image control to the height and width of the 
                // provided ad icon.
                AdIconImage.Height = Ad.AdIcon.Height;
                AdIconImage.Width = Ad.AdIcon.Width;
            }

            // Show the ad title.
            TitleTextBlock.Text = Ad.Title;

            // Show the ad description.
            if (!string.IsNullOrEmpty(Ad.Description))
            {
                DescriptionTextBlock.Text = Ad.Description;
                DescriptionTextBlock.Visibility = Visibility.Visible;
            }

            // Display the first main image for the ad. Note that the service
            // might provide multiple main images. 
            //if (Ad.MainImages.Count > 0)
            //{
            //    NativeImage mainImage = Ad.MainImages[0];
            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.UriSource = new Uri(mainImage.Url);
            //    MainImageImage.Source = bitmapImage;

            //    // Adjust the Image control to the height and width of the 
            //    // main image.
            //    MainImageImage.Height = mainImage.Height;
            //    MainImageImage.Width = mainImage.Width;
            //    MainImageImage.Visibility = Visibility.Visible;
            //}

            // Add the call to action string to the button.
            //if (!string.IsNullOrEmpty(Ad.CallToActionText))
            //{
            //    CallToActionButton.Content = Ad.CallToActionText;
            //    CallToActionButton.Visibility = Visibility.Visible;
            //}

            //// Show the ad sponsored by value.
            //if (!string.IsNullOrEmpty(Ad.SponsoredBy))
            //{
            //    SponsoredByTextBlock.Text = Ad.SponsoredBy;
            //    SponsoredByTextBlock.Visibility = Visibility.Visible;
            //}

            //// Show the icon image for the ad.
            //if (Ad.IconImage != null)
            //{
            //    BitmapImage bitmapImage = new BitmapImage();
            //    bitmapImage.UriSource = new Uri(Ad.IconImage.Url);
            //    IconImageImage.Source = bitmapImage;

            //    // Adjust the Image control to the height and width of the 
            //    // icon image.
            //    IconImageImage.Height = Ad.IconImage.Height;
            //    IconImageImage.Width = Ad.IconImage.Width;
            //    IconImageImage.Visibility = Visibility.Visible;
            //}
        }

        public NativeAdV2 Ad { get; set; }
    }
}
