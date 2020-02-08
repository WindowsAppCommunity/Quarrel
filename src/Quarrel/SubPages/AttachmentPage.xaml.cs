using DiscordAPI.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.ViewModels.SubPages;
using System;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Pdf;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.SubPages
{
    public sealed partial class AttachmentPage : UserControl, IFullscreenSubPage, ITransparentSubPage
    {
        private ISubFrameNavigationService _SubFrameNavigationService => SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();

        public AttachmentPage()
        {
            this.InitializeComponent();
            
            // Use navigation parameter for Image in ViewModel
            if (_SubFrameNavigationService.Parameter != null)
            {
                this.DataContext = new AttachmentPageViewModel((IPreviewableImage)_SubFrameNavigationService.Parameter);
            }

            // Show SVGs in SVGImageSource
            if (ViewModel.Image.ImageUrl.EndsWith(".svg"))
                ImageViewer.Source = new SvgImageSource(new Uri(ViewModel.Image.ImageUrl));
            // Show PDFs as images
            else if (ViewModel.Image.ImageUrl.EndsWith(".pdf"))
                LoadPDF();
            else
                ImageViewer.Source = new BitmapImage(new Uri(ViewModel.Image.ImageUrl));
        }

        private AttachmentPageViewModel ViewModel => DataContext as AttachmentPageViewModel;

        /// <summary>
        /// Converts a PDF to png and uses that as the image
        /// </summary>
        private async void LoadPDF()
        {
            ImageViewer.Visibility = Visibility.Collapsed;

            // Stream PDF
            HttpClient client = new HttpClient();
            var stream = await
                client.GetStreamAsync(ViewModel.Image.ImageUrl);
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());

            // Convert each page to image
            for (uint i = 0; i < doc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();

                var page = doc.GetPage(i);

                // Render page
                using (InMemoryRandomAccessStream RAstream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(RAstream);
                    await image.SetSourceAsync(RAstream);
                }

                Image UIImage = new Image();
                UIImage.Source = image;
                UIImage.Margin = new Thickness(0, 0, 0, 12);

                // Add Page to View
                MultiPageStacker.Children.Add(UIImage);
            }
        }

        /// <summary>
        /// Closes SubPage when the Background is tapped
        /// </summary>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _SubFrameNavigationService.GoBack();
        }

        /// <summary>
        /// Fades in image and hiding loading ring
        /// </summary>
        private async void ImageOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            await ImageViewer.Fade(1, 200).StartAsync();
            LoadingRing.IsActive = false;
        }

        /// <summary>
        /// Copies Image Url to clipboard
        /// </summary>
        private void CopyLink(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(ViewModel.Image.ImageUrl);
        }

        /// <summary>
        /// Opens Image URL in browser
        /// </summary>
        private async void Open(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(ViewModel.Image.ImageUrl));
        }

        /// <summary>
        /// Opens Windows Share prompt for image
        /// </summary>
        private void Share(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender1, args) =>
            {
                DataRequest request = args.Request;
                //request.Data.Properties.Title = ViewModel.Filename;
                var rasr = RandomAccessStreamReference.CreateFromUri(new Uri(ViewModel.Image.ImageUrl));
                request.Data.SetBitmap(rasr);
                request.Data.Properties.Thumbnail = rasr;
            };
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Saves image to selected location
        /// </summary>
        private async void Save(object sender, RoutedEventArgs e)
        {
            var image = new BitmapImage(new Uri(ViewModel.Image.ImageUrl));
            var fileSave = new FileSavePicker();
            fileSave.FileTypeChoices.Add("Image", new string[] { ".jpg" });
            var storageFile = await fileSave.PickSaveFileAsync();
            var uri = image.UriSource;

            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(uri, storageFile);
            await download.StartAsync();
        }

        #region Interfaces

        #region ITransparentSubPage

        public bool Dimmed { get => true; }

        #endregion

        #region IFullScreenSubPagex

        public bool Hideable { get => true; }

        #endregion

        #endregion

        /// <summary>
        /// Handles the UI Size changing
        /// </summary>
        private void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleImage();
        }

        /// <summary>
        /// Scales the image to take up 70% of the screen
        /// </summary>
        private void ScaleImage()
        {
            double imageRatio = ViewModel.Image.ImageHeight / ViewModel.Image.ImageWidth;
            double viewRatio = Container.ActualHeight / Container.ActualWidth;
            if (imageRatio > viewRatio) ImageViewer.Height = ActualHeight * .7;
            else ImageViewer.Width = ActualWidth * .7;
        }
    }
}
