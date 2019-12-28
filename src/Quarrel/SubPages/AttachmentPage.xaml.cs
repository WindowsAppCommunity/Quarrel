using DiscordAPI.Interfaces;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Quarrel.Helpers;
using Quarrel.Navigation;
using Quarrel.SubPages.Interfaces;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.SubPages
{
    public sealed partial class AttachmentPage : UserControl, IFullscreenSubPage, ITransparentSubPage
    {
        private ISubFrameNavigationService subFrameNavigationService = SimpleIoc.Default.GetInstance<ISubFrameNavigationService>();


        public AttachmentPage()
        {
            this.InitializeComponent();
            if (subFrameNavigationService.Parameter != null)
            {
                this.DataContext = subFrameNavigationService.Parameter;
            }

            if (ViewModel.ImageUrl.EndsWith(".svg"))
                ImageViewer.Source = new SvgImageSource(new Uri(ViewModel.ImageUrl));
            else if (ViewModel.ImageUrl.EndsWith(".pdf"))
                LoadPDF();
            else
                ImageViewer.Source = new BitmapImage(new Uri(ViewModel.ImageUrl));
        }

        private IPreviewableAttachment ViewModel => DataContext as IPreviewableAttachment;

        private async void LoadPDF()
        {
            ImageViewer.Visibility = Visibility.Collapsed;
            HttpClient client = new HttpClient();
            var stream = await
                client.GetStreamAsync(ViewModel.ImageUrl);
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

        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            subFrameNavigationService.GoBack();
        }

        private async void ImageOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            await ImageViewer.Fade(1, 200).StartAsync();
            LoadingRing.IsActive = false;
        }

        private void CopyLink(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(ViewModel.ImageUrl);
            Clipboard.SetContent(dataPackage);
        }

        private async void Open(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(ViewModel.ImageUrl));
        }

        private void Share(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender1, args) =>
            {
                DataRequest request = args.Request;
                //request.Data.Properties.Title = ViewModel.Filename;
                var rasr = RandomAccessStreamReference.CreateFromUri(new Uri(ViewModel.ImageUrl));
                request.Data.SetBitmap(rasr);
                request.Data.Properties.Thumbnail = rasr;
            };
            DataTransferManager.ShowShareUI();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var image = new BitmapImage(new Uri(ViewModel.ImageUrl));
            var fileSave = new FileSavePicker();
            fileSave.FileTypeChoices.Add("Image", new string[] { ".jpg" });
            var storageFile = await fileSave.PickSaveFileAsync();
            var uri = image.UriSource;

            var downloader = new BackgroundDownloader();
            var download = downloader.CreateDownload(uri, storageFile);
            await download.StartAsync();
        }

        #region Display

        public bool IsFile { get => ViewModel is Attachment; }

        public Attachment AsFile { get => ViewModel as Attachment; }

        #endregion

        #region ITransparentSubPage

        public bool Dimmed { get => true; }

        #endregion

        public bool Hideable { get => true; }

        private void ContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScaleImage();
        }

        private void ScaleImage()
        {
            double imageRatio = ViewModel.ImageHeight / ViewModel.ImageWidth;
            double viewRatio = Container.ActualHeight / Container.ActualWidth;
            if (imageRatio > viewRatio) ImageViewer.Height = ActualHeight * .7;
            else ImageViewer.Width = ActualWidth * .7;
        }
    }
}
