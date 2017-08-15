using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class InAppPurchases : Page
    {
        public InAppPurchases()
        {
            this.InitializeComponent();
        }
        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }
        private async void MakePurchase(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            switch ((sender as Button).Tag.ToString())
            {
                case "RemoveAds":
                    if (!licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("RemoveAds");

                            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnPurchased"));
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnNotPurchased"));
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["RemoveAds"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                App.ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnError"));
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Polite":
                    if (!licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("Polite Dontation");

                            if (licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnPurchased"));
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnNotPurchased"));
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["Polite Dontation"].IsActive)
                            {
                                BuyAdRemovalButton.Visibility = Visibility.Collapsed;
                                App.ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnError"));
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Significant":
                    if (!licenseInformation.ProductLicenses["SignificantDontation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("SignificantDonation ");

                            if (licenseInformation.ProductLicenses["SignificantDonation "].IsActive)
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnPurchased"));
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnNotPurchased"));
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["SignificantDontation"].IsActive)
                            {
                                App.ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnError"));
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "OMGTHX":
                    if (!licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("OMGTHXDonation");

                            if (licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnPurchased"));
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnNotPurchased"));
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["OMGTHXDonation"].IsActive)
                            {
                                App.ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnError"));
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
                case "Ridiculous":
                    if (!licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                    {
                        try
                        {
                            // The customer doesn't own this feature, so
                            // show the purchase dialog.
                            PurchaseResults purchase = await CurrentApp.RequestProductPurchaseAsync("RidiculousDonation");

                            if (licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnPurchased"));
                                await msg.ShowAsync();
                            }
                            else
                            {
                                MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnNotPurchased"));
                                await msg.ShowAsync();
                            }

                            licenseInformation = CurrentApp.LicenseInformation;

                            if (licenseInformation.ProductLicenses["RidiculousDonation"].IsActive)
                            {
                                App.ShowAds = false;
                            }
                            //Check the license state to determine if the in-app purchase was successful.
                        }
                        catch (Exception)
                        {
                            MessageDialog msg = new MessageDialog(App.GetString("/Dialogs/AddOnError"));
                            await msg.ShowAsync();
                        }
                    }
                    else
                    {
                        // The customer already owns this feature.
                    }
                    break;
            }
        }
    }
}
