// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.SubPages.Interfaces;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.ViewModels.SubPages;
using System;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Pdf;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.SubPages
{
    /// <summary>
    /// The sub page for displaying an attachment or embedded image full screen.
    /// </summary>
    public sealed partial class AttachmentPage : UserControl, IFullscreenSubPage, ITransparentSubPage
    {
        private IAnalyticsService _analyticsService = null;
        private ISubFrameNavigationService _subFrameNavigationService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentPage"/> class.
        /// </summary>
        public AttachmentPage()
        {
            this.InitializeComponent();

            // Use navigation parameter for Image in ViewModel
            if (SubFrameNavigationService.Parameter != null)
            {
                var image = (IPreviewableImage)SubFrameNavigationService.Parameter;
                this.DataContext = new AttachmentPageViewModel(image);

                AnalyticsService.Log(
                    Constants.Analytics.Events.OpenAttachment,
                    ("image-url", image.ImageUrl));
            }
        }

        /// <summary>
        /// Gets the Attachment's information.
        /// </summary>
        public AttachmentPageViewModel ViewModel => DataContext as AttachmentPageViewModel;

        /// <inheritdoc/>
        public bool Dimmed => true;

        /// <inheritdoc/>
        public bool Hideable { get => true; }

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private ISubFrameNavigationService SubFrameNavigationService => _subFrameNavigationService ?? (_subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>());

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsImageAnimated)
            {
                VideoViewer.Source = new Uri(ViewModel.Image.AnimatedImageUrl);
            }
            else
            {
                // Show SVGs in SVGImageSource
                if (ViewModel.Image.ImageUrl.EndsWith(".svg"))
                {
                    ImageViewer.Source = new SvgImageSource(new Uri(ViewModel.Image.ImageUrl));
                }

                // Show PDFs as images
                else if (ViewModel.Image.ImageUrl.EndsWith(".pdf"))
                {
                    LoadPDF();
                }
                else
                {
                    ImageViewer.Source = new BitmapImage(new Uri(ViewModel.Image.ImageUrl));
                }
            }
        }

        /// <summary>
        /// Converts a PDF to png and uses that as the image.
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
                using (InMemoryRandomAccessStream rastream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(rastream);
                    await image.SetSourceAsync(rastream);
                }

                Image uiImage = new Image();
                uiImage.Source = image;
                uiImage.Margin = new Thickness(0, 0, 0, 12);

                // Add Page to View
                MultiPageStacker.Children.Add(uiImage);
            }
        }

        /// <summary>
        /// Fades in image and hiding loading ring.
        /// </summary>
        private async void ImageOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            await ImageViewer.Fade(1, 200).StartAsync();
            LoadingRing.IsActive = false;
        }

        /// <summary>
        /// Fades in video and hiding loading ring when it starts to play.
        /// </summary>
        private async void VideoStateChanged(object sender, RoutedEventArgs e)
        {
            if (VideoViewer.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
            {
                await VideoViewer.Fade(1, 200).StartAsync();
                LoadingRing.IsActive = false;
            }
        }

        /// <summary>
        /// Copies Image Url to clipboard.
        /// </summary>
        private void CopyLink(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(ViewModel.Image.AnimatedImageUrl ?? ViewModel.Image.ImageUrl);
        }

        /// <summary>
        /// Opens Image URL in browser.
        /// </summary>
        private async void Open(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(ViewModel.Image.AnimatedImageUrl ?? ViewModel.Image.ImageUrl));
        }

        /// <summary>
        /// Opens Windows Share prompt for image.
        /// </summary>
        private void Share(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender1, args) =>
            {
                DataRequest request = args.Request;
                if (!ViewModel.IsImageAnimated)
                {
                    var rasr = RandomAccessStreamReference.CreateFromUri(new Uri(ViewModel.Image.ImageUrl));
                    request.Data.SetBitmap(rasr);
                    request.Data.Properties.Thumbnail = rasr;
                    request.Data.Properties.Title = "Image.png";
                }
                else
                {
                    // It may be preferable to find some way to share the video file itself instead of a link
                    request.Data.SetUri(new Uri(ViewModel.Image.AnimatedImageUrl));
                    request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(ViewModel.Image.ImageUrl));
                    request.Data.Properties.Title = "Animation.mp4";
                }
            };
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Saves image to selected location.
        /// </summary>
        private async void Save(object sender, RoutedEventArgs e)
        {
            var fileSave = new FileSavePicker();

            if (!ViewModel.IsImageAnimated)
            {
                fileSave.FileTypeChoices.Add("Image", new string[] { ".jpg", ".png" });
            }
            else
            {
                fileSave.FileTypeChoices.Add("Video", new string[] { ".mp4" });
            }

            var storageFile = await fileSave.PickSaveFileAsync();

            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(new Uri(ViewModel.Image.AnimatedImageUrl ?? ViewModel.Image.ImageUrl), storageFile);
            await download.StartAsync();
        }

        /// <summary>
        /// Handles the UI Size changing.
        /// </summary>
        private void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleImage();
        }

        /// <summary>
        /// Closes SubPage when the Background is tapped.
        /// </summary>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SubFrameNavigationService.GoBack();
        }

        /// <summary>
        /// Scales the image to take up 70% of the screen.
        /// </summary>
        private void ScaleImage()
        {
            double imageRatio = ViewModel.Image.ImageHeight / ViewModel.Image.ImageWidth;
            double viewRatio = Container.ActualHeight / Container.ActualWidth;
            if (!ViewModel.IsImageAnimated)
            {
                if (imageRatio > viewRatio)
                {
                    ImageViewer.Height = ActualHeight * .7;
                }
                else
                {
                    ImageViewer.Width = ActualWidth * .7;
                }
            }
            else
            {
                if (imageRatio > viewRatio)
                {
                    VideoViewer.Height = ActualHeight * .7;
                }
                else
                {
                    VideoViewer.Width = ActualWidth * .7;
                }
            }
        }
    }
}
