using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace Discord_UWP.Controls
{
    public sealed partial class AttachementControl : UserControl
    {

        public SharedModels.Attachment DisplayedAttachement
        {
            get { return (SharedModels.Attachment)GetValue(AttachementProperty); }
            set { SetValue(AttachementProperty, value); }
        }
        public static readonly DependencyProperty AttachementProperty = DependencyProperty.Register(
            nameof(DisplayedAttachement),
            typeof(SharedModels.Attachment),
            typeof(AttachementControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        public bool IsFake
        {
            get { return (bool)GetValue(IsFakeProperty); }
            set { SetValue(IsFakeProperty, value); }
        }
        public static readonly DependencyProperty IsFakeProperty = DependencyProperty.Register(
            nameof(IsFake),
            typeof(bool),
            typeof(AttachementControl),
            new PropertyMetadata(false, OnPropertyChangedStatic));

        public event EventHandler<EventArgs> Delete;

        private enum Type { Unknown, Image, Audio, Video, PDF};

        private Type type = Type.Unknown;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as AttachementControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        readonly string[] ImageFiletypes = { ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".png", ".bmp", ".gif", ".ico" };
        readonly string[] AudioFiletypes = { ".mp3", ".wav" };
        readonly string[] VideoFiletypes = { ".mp4", ".wmv" };

        private void OnPropertyChanged(DependencyObject d, DependencyProperty property)
        {
            if (property == AttachementProperty)
            {
                LoadAttachement(!IsFake);
            }
        }

        private void AttachedImageViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.OpenAttachement(DisplayedAttachement);
        }

        private void LoadAttachement(bool images)
        {
            AttachedImageViewer.Source = null;
            AttachedImageViewbox.Visibility = Visibility.Collapsed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
            AttachedFileViewer.Visibility = Visibility.Collapsed;
            ClearButton.Visibility = Visibility.Collapsed;
            if (IsFake) ClearButton.Visibility = Visibility.Visible;

            if (DisplayedAttachement == null) return;
            
            if (images)
            {
                string extension = "." + DisplayedAttachement.Filename.Split('.').Last().ToLower();
                if (ImageFiletypes.Contains(extension))
                {
                    type = Type.Image;
                    PreviewIcon.Glyph = "";
                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }
                } else if (AudioFiletypes.Contains(extension))
                {
                    type = Type.Audio;
                    PreviewIcon.Glyph = "";
                    player.AudioCategory = AudioCategory.Media;

                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }

                } else if (VideoFiletypes.Contains(extension))
                {
                    type = Type.Video;
                    PreviewIcon.Glyph = "";
                    player.AudioCategory = AudioCategory.Media;
                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }
                } else if (".pdf" == extension)
                {
                    type = Type.PDF;
                    PreviewIcon.Glyph = "";
                }
            }
            if (type == Type.Unknown || type == Type.PDF || NetworkSettings.GetTTL())
            { 
                if (type != Type.Unknown) { PreviewButton.Visibility = Visibility.Visible; FileIcon.Visibility = Visibility.Collapsed; }
                if(!IsFake)
                    FileName.NavigateUri = new Uri(DisplayedAttachement.Url);
                FileName.Content = DisplayedAttachement.Filename;
                FileSize.Text = Common.HumanizeFileSize(DisplayedAttachement.Size);
                AttachedFileViewer.Visibility = Visibility.Visible;
                player.Visibility = Visibility.Collapsed;
            }
        }

        private void AttachedImageViewer_ImageLoaded(object sender, RoutedEventArgs e)
        {
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
        }

        private void AttachementImageViewer_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
            //Reload attachements but with images disabled
            LoadAttachement(false);
        }

        public AttachementControl()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(this, null);
        }

        private async void FileName_Click(object sender, RoutedEventArgs e)
        {
            if (IsFake)
            {
                await Launcher.LaunchUriAsync(new Uri("file:///" + Uri.EscapeUriString(DisplayedAttachement.Url.Replace('\\','/'))));
            }
        }

        private void AttachedImageViewer_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, DisplayedAttachement.Url, e.GetPosition(this));
            }
        }

        private void AttachedImageViewer_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, DisplayedAttachement.Url, e.GetPosition(this));
            }
        }

        private void ShowPreview(object sender, RoutedEventArgs e)
        {
            ShowPreview();
        }

        private async void ShowPreview()
        {
            switch (type)
            {
                case Type.Image:
                    if (DisplayedAttachement.Filename.EndsWith(".svg"))
                    {
                        AttachedImageViewer.Source = new SvgImageSource(new Uri(DisplayedAttachement.Url));
                    }
                    else
                    {
                        AttachedImageViewer.Source = new BitmapImage(new Uri(DisplayedAttachement.Url));
                    }
                    AttachedImageViewbox.Visibility = Visibility.Visible;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;
                    break;
                case Type.Audio:
                    player.Source = new Uri(DisplayedAttachement.Url);
                    player.Visibility = Visibility.Visible;
                    break;
                case Type.Video:
                    player.Source = new Uri(DisplayedAttachement.Url);
                    if (DisplayedAttachement.Height.HasValue)
                    {
                        player.Height = DisplayedAttachement.Height.Value;
                    }
                    if (DisplayedAttachement.Width.HasValue)
                    {
                        player.Width = DisplayedAttachement.Width.Value;
                    }
                    if (player.Height > 300)
                    {
                        player.Width = player.Width / (player.Height / 300);
                        player.Height = 300;
                    }
                    if (player.Width > 300)
                    {
                        player.Height = player.Height / (player.Width / 300);
                        player.Width = 300;
                    }
                    player.Visibility = Visibility.Visible;
                    break;
                case Type.PDF:
                    HttpClient client = new HttpClient();
                    var stream = await
                        client.GetStreamAsync(DisplayedAttachement.Url);
                    var memStream = new MemoryStream();
                    await stream.CopyToAsync(memStream);
                    memStream.Position = 0;
                    PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
                    LoadPDF(doc);
                    break;
            }
            AttachedFileViewer.Visibility = Visibility.Collapsed;
        }

        async void LoadPDF(PdfDocument pdfDoc)
        {
            BitmapImage image = new BitmapImage();

            var page = pdfDoc.GetPage(0); //TODO: PasswordProtected PDF support

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
            }

            AttachedImageViewer.Source = image;
            PDFPages.Text = "Page <page> of <pagecount>".Replace("<page>", "1").Replace("<pagecount>", pdfDoc.PageCount.ToString()); //TODO: Translate
            AttachedImageViewbox.Visibility = Visibility.Visible;
            PDFPages.Visibility = Visibility.Visible;
        }
    }
}
