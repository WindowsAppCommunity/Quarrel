using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as AttachementControl;
            instance?.OnPropertyChanged(d, e.Property);
        }

        readonly string[] ImageFiletypes = { ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".png", ".bmp", ".gif", ".ico" };
        readonly string[] AudioFiletypes = { ".mp3", ".wav"};
        readonly string[] VideoFiletypes = { ".mp4"};

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

            bool IsImage = false;
            bool IsAudio = false;
            bool IsVideo = false;
            if (images)
            {
                if (ImageFiletypes.Contains("." + DisplayedAttachement.Filename.Split('.').Last().ToLower()))
                {
                    IsImage = true;
                    if (DisplayedAttachement.Filename.EndsWith(".svg"))
                    {
                        AttachedImageViewer.Source = new SvgImageSource(new Uri(DisplayedAttachement.Url));
                    }
                    else
                    {
                        AttachedImageViewer.Source = new BitmapImage(new Uri(DisplayedAttachement.Url));
                    }
                } else if (AudioFiletypes.Contains("." + DisplayedAttachement.Filename.Split('.').Last().ToLower()))
                {
                    IsAudio = true;
                    player.AudioCategory = AudioCategory.Media;
                    player.Source = new Uri(DisplayedAttachement.Url);
                } else if (VideoFiletypes.Contains("." + DisplayedAttachement.Filename.Split('.').Last().ToLower()))
                {
                    IsVideo = true;
                    player.AudioCategory = AudioCategory.Media;
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
                }
            }
            if (IsImage)
            {
                AttachedImageViewbox.Visibility = Visibility.Visible;
                LoadingImage.Visibility = Visibility.Visible;
                LoadingImage.IsActive = true;
            } else if (IsAudio)
            {
                player.Visibility = Visibility.Visible;
            } else if (IsVideo)
            {
                player.Visibility = Visibility.Visible;
            }
            else
            {
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
    }
}
