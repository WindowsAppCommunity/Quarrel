using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using DiscordAPI.SharedModels;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class PreviewAttachement : Page
    {
        public PreviewAttachement()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private Attachment attachement;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Attachment)
            {
                attachement = (Attachment)e.Parameter;
                if (attachement.Url.EndsWith(".svg"))
                    ImageViewer.Source = new SvgImageSource(new Uri(attachement.Url));
                else if (attachement.Url.EndsWith(".pdf"))
                    LoadPDF();
                else
                    ImageViewer.Source = new BitmapImage(new Uri(attachement.Url));
            }
            base.OnNavigatedTo(e);
        }

        private async void LoadPDF()
        {
            ImageViewer.Visibility = Visibility.Collapsed;
            HttpClient client = new HttpClient();
            var stream = await
                client.GetStreamAsync(attachement.Url);
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());

            for (uint i = 0; i < doc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();

                var page = doc.GetPage(i);

                using (InMemoryRandomAccessStream RAstream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(RAstream);
                    await image.SetSourceAsync(RAstream);
                }

                Image UIImage = new Image();
                UIImage.Source = image;
                UIImage.Margin = new Thickness(0, 0, 0, 12);
                MultiPageStacker.Children.Add(UIImage);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private async void ImageViewer_ImageOpened(object sender, RoutedEventArgs e)
        {
            await ImageViewer.Fade(1, 200).StartAsync();
            LoadingRing.IsActive = false;
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sc = (sender as ScrollViewer);
            if (sc.ZoomFactor > 1 && cbContainer.Opacity == 1)
                cbContainer.Fade(0, 100).Start();
            else if (sc.ZoomFactor == 1)
                cbContainer.Fade(1, 100).Start();
        }

        private void ScrollViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var sc = (sender as ScrollViewer);
            if (cbContainer.Opacity == 1)
                cbContainer.Fade(0, 200).Start();
            else if (cbContainer.Opacity == 0)
                cbContainer.Fade(1, 200).Start();
        }

        private void PreviewAttachement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (attachement.Width != null && attachement.Width < ActualWidth)
                ImageViewer.MaxWidth = (int)attachement.Width;
            else if (ActualWidth < 700)
                ImageViewer.MaxWidth = ActualWidth;
            else
                ImageViewer.MaxWidth = 700;

            if (attachement.Height != null && attachement.Height < ActualHeight)
                ImageViewer.MaxHeight = (int)attachement.Height;
            else if (ActualHeight < 700)
                ImageViewer.MaxHeight = ActualHeight;
            else
                ImageViewer.MaxHeight = 700;
        }

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null,null);
        }

        private void Filename_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = attachement.Filename;
        }

        private void Filesize_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = Common.HumanizeFileSize(attachement.Size);
        }

        private void Width_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = attachement.Width.ToString();
        }

        private void Height_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = attachement.Height.ToString();
        }

        private void CopyLink_OnClick(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(attachement.Url);
            Clipboard.SetContent(dataPackage);
        }

        private async void Open_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(attachement.Url));
        }

        private void Share_OnClick(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender1, args) =>
            {
                DataRequest request = args.Request;
                request.Data.Properties.Title = attachement.Filename;
                var rasr = RandomAccessStreamReference.CreateFromUri(new Uri(attachement.Url));
                request.Data.SetBitmap(rasr);
                request.Data.Properties.Thumbnail = rasr;
            };
            DataTransferManager.ShowShareUI();
        }

        private async void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var image = new BitmapImage(new Uri(attachement.Url));
            var fileSave = new FileSavePicker();
            fileSave.FileTypeChoices.Add("Image", new string[] { ".jpg" });
            var storageFile = await fileSave.PickSaveFileAsync();
            var uri = image.UriSource;

            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(uri, storageFile);
            await download.StartAsync();
        }
    }
}
