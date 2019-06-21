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
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class AttachementControl : UserControl
    {
        /// <summary>
        /// The API data Attachment to display
        /// </summary>
        public Attachment DisplayedAttachement
        {
            get { return (Attachment)GetValue(AttachementProperty); }
            set { SetValue(AttachementProperty, value); }
        }
        public static readonly DependencyProperty AttachementProperty = DependencyProperty.Register(
            nameof(DisplayedAttachement),
            typeof(Attachment),
            typeof(AttachementControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));
        
        /// <summary>
        /// True if the Attachement is in the MessageEditor
        /// </summary>
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

        /// <summary>
        /// Types of attachements
        /// </summary>
        private enum Type { Unknown, Image, Audio, Video, PDF};

        /// <summary>
        /// The type of this Attachment
        /// </summary>
        private Type type = Type.Unknown;

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as AttachementControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        #region Raw FileType lists
        readonly string[] ImageFiletypes = { ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".png", ".bmp", ".gif", ".ico", ".jxr", ".hdp", ".wdp" };
        readonly string[] AudioFiletypes = { ".mp3", ".wav", ".m4a", ".ac3", ".ec3", ".flac", ".3gp", ".amr"};
        readonly string[] VideoFiletypes = { ".3g2", ".3gp2", ".3gp", ".m4v", ".mp4v", ".mp4", ".mov", ".m2ts", ".asf", ".wm", ".wmv", ".avi"};
        #endregion

        private void OnPropertyChanged(DependencyObject d, DependencyProperty property)
        {
            if (property == AttachementProperty)
            {
                LoadAttachement(!IsFake);
            }
        }


        private void AttachedImageViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Open the Attachment to be viewed in full windows
            App.OpenAttachement(DisplayedAttachement);
        }

        /// <summary>
        /// Load the attachement
        /// </summary>
        /// <param name="images">True if the preview should be shown</param>
        private void LoadAttachement(bool images)
        {
            // Clear Attachment
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
                // Set the FileType icon and show preview if applicable
                string extension = "." + DisplayedAttachement.Filename.Split('.').Last().ToLower();
                if (ImageFiletypes.Contains(extension))
                {
                    type = Type.Image;
                    PreviewIcon.Glyph = "";
                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }
                }
                else if (AudioFiletypes.Contains(extension))
                {
                    type = Type.Audio;
                    PreviewIcon.Glyph = "";
                    player.AudioCategory = AudioCategory.Media;

                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }

                }
                else if (VideoFiletypes.Contains(extension))
                {
                    type = Type.Video;
                    PreviewIcon.Glyph = "";
                    player.AudioCategory = AudioCategory.Media;
                    if (!NetworkSettings.GetTTL())
                    {
                        ShowPreview();
                    }
                }
                else if (".pdf" == extension)
                {
                    type = Type.PDF;
                    PreviewIcon.Glyph = "";
                }
            }

            // Enable navigation if applicable
            if (!IsFake)
                FileName.NavigateUri = new Uri(DisplayedAttachement.Url);
            
            // Set FileName and byte size info
            FileName.Content = DisplayedAttachement.Filename;
            FileSize.Text = Common.HumanizeFileSize(DisplayedAttachement.Size);

            if (type == Type.Unknown || type == Type.PDF || NetworkSettings.GetTTL())
            { 
                // Show FileType Icon
                if (type != Type.Unknown) { PreviewButton.Visibility = Visibility.Visible; FileIcon.Visibility = Visibility.Collapsed; }
                AttachedFileViewer.Visibility = Visibility.Visible;
                player.Visibility = Visibility.Collapsed;
            }
            else if (type == Type.Audio || type == Type.Video)
            {
                // Show MediaPlayer
                player.Visibility = Visibility.Visible;
                if(type == Type.Audio)
                    player.Height = 48;
            }
        }

        /// <summary>
        /// Image successfully loaded
        /// </summary>
        private void AttachedImageViewer_ImageLoaded(object sender, RoutedEventArgs e)
        {
            AttachedImageViewer.ImageOpened -= AttachedImageViewer_ImageLoaded;
            AttachedImageViewer.ImageFailed -= AttachementImageViewer_ImageFailed;
            LoadingImage.IsActive = false;
            LoadingImage.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Image Failed to load
        /// </summary>
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

        /// <summary>
        /// Invoke remove Attachment event
        /// </summary>
        private void RemoveAttachement(object sender, RoutedEventArgs e)
        {
            Delete?.Invoke(this, null);
        }

        /// <summary>
        /// Open file
        /// </summary>
        private async void FileName_Click(object sender, RoutedEventArgs e)
        {
            if (IsFake)
            {
                await Launcher.LaunchUriAsync(new Uri("file:///" + Uri.EscapeUriString(DisplayedAttachement.Url.Replace('\\','/'))));
            }
        }
        
        /// <summary>
        /// Show "Save Image" flyout (Right Tapped)
        /// </summary>
        private void AttachedImageViewer_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, DisplayedAttachement.Url, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Show "Save Image" flyout (Holding)
        /// </summary>
        private void AttachedImageViewer_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, DisplayedAttachement.Url, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Show Attachment Preview
        /// </summary>
        private void ShowPreview(object sender, RoutedEventArgs e) => ShowPreview();

        /// <summary>
        /// Show Attachment Preview
        /// </summary>
        private async void ShowPreview()
        {
            switch (type)
            {
                // If it's an Image, begin Loading image and show Loading indicator
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

                // If it's Audio, begin loading audio and show MediaPlayer
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

                // If it's a Video, begin loading video and show MediaPlayer
                case Type.Video:
                    FileIcon.Visibility = Visibility.Collapsed;
                    FileName.FontSize = 14;
                    AttachedFileViewer.Visibility = Visibility.Visible;
                    PreviewButton.Visibility = Visibility.Collapsed;
                    player.Source = new Uri(DisplayedAttachement.Url);
                    player.Visibility = Visibility.Visible;
                    player.HorizontalAlignment = HorizontalAlignment.Left;
                    break;

                // If it's a PDF, show PDF Image preview
                case Type.PDF:
                    AttachedFileViewer.Visibility = Visibility.Collapsed;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;

                    // Get PDF (stream) from URL
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
        }

        /// <summary>
        /// Load a PDF preview image
        /// </summary>
        /// <param name="pdfDoc">PDF to load</param>
        async void LoadPDF(PdfDocument pdfDoc)
        {
            // Get PDF image preview from PdfDocument
            BitmapImage image = new BitmapImage();
            var page = pdfDoc.GetPage(0); //TODO: PasswordProtected PDF support
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
            }

            // Set the Attachmenet image to PDF preview
            AttachedImageViewer.Source = image;

            // TODO: ADD MORE COMMENTS 
            PDFPages.Text = App.GetString("/Controls/PDFPage").Replace("<page>", "1").Replace("<pagecount>", pdfDoc.PageCount.ToString());
            AttachedImageViewbox.Visibility = Visibility.Visible;
            PDFPages.Visibility = Visibility.Visible;
            LoadingImage.Visibility = Visibility.Collapsed;
            LoadingImage.IsActive = true;
        }
    }
}
