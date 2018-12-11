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

        readonly string[] ImageFiletypes = { ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".png", ".bmp", ".gif", ".ico", ".jxr", ".hdp", ".wdp" };
        readonly string[] AudioFiletypes = { ".mp3", ".wav", ".m4a", ".ac3", ".ec3", ".flac", ".3gp", ".amr"};
        readonly string[] VideoFiletypes = { ".3g2", ".3gp2", ".3gp", ".m4v", ".mp4v", ".mp4", ".mov", ".m2ts", ".asf", ".wm", ".wmv", ".avi"};

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
            if (!IsFake)
                FileName.NavigateUri = new Uri(DisplayedAttachement.Url);
            FileName.Content = DisplayedAttachement.Filename;
            FileSize.Text = Common.HumanizeFileSize(DisplayedAttachement.Size);
            if (type == Type.Unknown || type == Type.PDF || NetworkSettings.GetTTL())
            { 
                if (type != Type.Unknown) { PreviewButton.Visibility = Visibility.Visible; FileIcon.Visibility = Visibility.Collapsed; }
                AttachedFileViewer.Visibility = Visibility.Visible;
                player.Visibility = Visibility.Collapsed;
            }
            else if (type == Type.Audio || type == Type.Video)
            {
                player.Visibility = Visibility.Visible;
                if(type == Type.Audio)
                    player.Height = 48;
                
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

        private void player_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            if(DisplayedAttachement.Width.HasValue && DisplayedAttachement.Height.HasValue)
            {
                double aspectratio = (double)DisplayedAttachement.Width.Value / (double)DisplayedAttachement.Height.Value;
                if (aspectratio < 1)
                {
                    //Player should be higher than it is wide
                    player.Height = player.ActualWidth / aspectratio;
                }
                else
                {
                    //Player should be wider than it is high
                    player.Width = player.ActualHeight / aspectratio;
                }
            }*/
            
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
                    AttachedFileViewer.Visibility = Visibility.Collapsed;
                    AttachedImageViewbox.Visibility = Visibility.Visible;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;
                    break;
                case Type.Audio:
                    FileIcon.Visibility = Visibility.Collapsed;
                    FileName.FontSize = 14;
                    AttachedFileViewer.Visibility = Visibility.Visible;
                    PreviewButton.Visibility = Visibility.Collapsed;
                    player.HorizontalAlignment = HorizontalAlignment.Stretch;
                    player.Source = new Uri(DisplayedAttachement.Url);
                    player.Visibility = Visibility.Visible;
                    player.Height = 48;
                    break;
                case Type.Video:
                    FileIcon.Visibility = Visibility.Collapsed;
                    FileName.FontSize = 14;
                    AttachedFileViewer.Visibility = Visibility.Visible;
                    PreviewButton.Visibility = Visibility.Collapsed;
                    player.Source = new Uri(DisplayedAttachement.Url);
                    player.Visibility = Visibility.Visible;
                    player.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Type.PDF:
                    AttachedFileViewer.Visibility = Visibility.Collapsed;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;
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
            
            //AttachedFileViewer.Visibility = Visibility.Collapsed;
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
            PDFPages.Text = App.GetString("/Controls/PDFPage").Replace("<page>", "1").Replace("<pagecount>", pdfDoc.PageCount.ToString());
            AttachedImageViewbox.Visibility = Visibility.Visible;
            PDFPages.Visibility = Visibility.Visible;
            LoadingImage.Visibility = Visibility.Collapsed;
            LoadingImage.IsActive = true;
        }
    }
}
