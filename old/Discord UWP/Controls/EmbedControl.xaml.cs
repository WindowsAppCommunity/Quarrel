using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Gregstoll;
using Quarrel.MarkdownTextBlock;
using Windows.ApplicationModel.DataTransfer;
using DiscordAPI.SharedModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class EmbedControl : UserControl
    {
        // API Embed Data to display
        public Embed EmbedContent
        {
            get { return (Embed)GetValue(EmbedContentProperty); }
            set { SetValue(EmbedContentProperty, value); }
        }
        public static readonly DependencyProperty EmbedContentProperty = DependencyProperty.Register(
            nameof(EmbedContent),
            typeof(Embed),
            typeof(EmbedControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as EmbedControl;
            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        /// <summary>
        /// Types of Embeds
        /// </summary>
        private enum Type { Unknown, Image, Audio, Video, PDF };

        /// <summary>
        /// Type of embed this embed displays
        /// </summary>
        private Type type = Type.Unknown;

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            // Accent color
            if (EmbedContent.Color != 0)
            {
                SideBorder.Background = Common.IntToColor(EmbedContent.Color);
            }
            else
            {
                SideBorder.Background = (SolidColorBrush)App.Current.Resources["LightBG"];
            }

            // Author
            if (EmbedContent.Author != null)
            {
                AuthorSP.Visibility=Visibility.Visible;
                AuthorText.Content = EmbedContent.Author.Name;
                if(EmbedContent.Author.Url != null)
                    AuthorText.NavigateUri = new Uri(EmbedContent.Author.Url);
                if (EmbedContent.Author.IconUrl == null)
                {
                    AuthorImage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AuthorImage.Visibility = Visibility.Visible;
                    AuthorImageBrush.ImageSource = new BitmapImage(new Uri(EmbedContent.Author.IconUrl));
                }
            }
            else
            {
                AuthorSP.Visibility=Visibility.Collapsed;
            }

            // Title
            if (EmbedContent.Title != null)
            {
                if (EmbedContent.Url == null)
                {
                    UrlTitleBlock.Visibility = Visibility.Collapsed;
                    PlainTitleBlock.Visibility = Visibility.Visible;
                    PlainTitleBlock.Text = EmbedContent.Title;
                }
                else
                {
                    PlainTitleBlock.Visibility = Visibility.Collapsed;
                    UrlTitleBlock.Visibility = Visibility.Visible;
                    UrlTitleBlock.Content = EmbedContent.Title;
                    UrlTitleBlock.NavigateUri = new Uri(EmbedContent.Url);
                    
                    // Share Icon (CU is weird here)
                    if (App.FCU)
                    {
                        ShareButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        CUShareButton.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                PlainTitleBlock.Visibility = Visibility.Collapsed;
                UrlTitleBlock.Visibility = Visibility.Collapsed;
            }

            // Description
            if (EmbedContent.Description != null)
            {
                DescriptionBlock.Visibility = Visibility.Visible;
                DescriptionBlock.Text = EmbedContent.Description;
            }
            else
            {
                DescriptionBlock.Visibility = Visibility.Collapsed;
            }

            // Fields
            UniversalWrapPanel previousWrapPanel= null;
            FieldsStacker.Children.Clear();
            if (EmbedContent.Fields != null)
            {
                FieldsStacker.Visibility = Visibility.Visible;
                foreach (var field in EmbedContent.Fields)
                {
                    if (field.Inline)
                    {
                        if (previousWrapPanel == null)
                            previousWrapPanel = new UniversalWrapPanel(){Orientation=Orientation.Horizontal};
                        previousWrapPanel.Children.Add(GenerateField(field));
                    }
                    else
                    {
                        if (previousWrapPanel != null)
                        {
                            FieldsStacker.Children.Add(previousWrapPanel);
                            previousWrapPanel = null;
                        }
                        FieldsStacker.Children.Add(GenerateField(field));
                    }
                }
                if (previousWrapPanel != null)
                {
                    FieldsStacker.Children.Add(previousWrapPanel);
                }
            }
            else
            {
                FieldsStacker.Visibility = Visibility.Collapsed;
            }

            // Content
            if (EmbedContent.Image != null)
            {
                type = Type.Image;
                ImageViewbox.Visibility = Visibility.Collapsed;
                player.Visibility = Visibility.Collapsed;
                PreviewIcon.Glyph = "";
                if (!NetworkSettings.GetTTL())
                {
                    ShowPreview();
                } else
                {
                    PreviewButton.Visibility = Visibility.Visible;
                }
                if((EmbedContent.Author == null || (EmbedContent.Author.Name == null && EmbedContent.Author.IconUrl == null))
                    && EmbedContent.Description == null && EmbedContent.Fields != null
                    && EmbedContent.Fields.Count() == 0  && EmbedContent.Footer.Text == null
                    && EmbedContent.Footer.IconUrl == null && EmbedContent.Title == null)
                {
                    stacker.Margin = new Thickness(0);
                    SideBorder.Width = 0;
                }
            }
            else if (EmbedContent.Video != null)
            {
                type = Type.Video;
                ImageViewbox.Visibility = Visibility.Collapsed;
                player.Visibility = Visibility.Collapsed;
                PreviewIcon.Glyph = "";
                if (!NetworkSettings.GetTTL())
                {
                    ShowPreview();
                } else
                {
                    PreviewButton.Visibility = Visibility.Visible;
                }
                player.Height = Math.Min(EmbedContent.Video.Height, this.Height);
                player.Width = Math.Min(EmbedContent.Video.Width, this.Width);
                if ((EmbedContent.Author == null || (EmbedContent.Author.Name == null && EmbedContent.Author.IconUrl == null))
                    && EmbedContent.Description == null && EmbedContent.Fields != null
                    && EmbedContent.Fields.Count() == 0 && EmbedContent.Footer.Text == null
                    && EmbedContent.Footer.IconUrl == null && EmbedContent.Title == null)
                {
                    stacker.Margin = new Thickness(0);
                    SideBorder.Width = 0;
                }
            }
            else
            {
                ImageViewbox.Visibility = Visibility.Collapsed;
                player.Visibility = Visibility.Collapsed;
            }

            // Footer
            if (EmbedContent.Footer == null && EmbedContent.Timestamp == null)
            {
                FooterSP.Visibility = Visibility.Collapsed;
            }
            else
            {
                FooterSP.Visibility = Visibility.Visible;
                string footertext = "";
                if (EmbedContent.Footer?.Text != null)
                    footertext = EmbedContent.Footer.Text;
                if (EmbedContent.Timestamp != null)
                {
                    if (footertext != "") footertext += " | ";
                    footertext += Common.HumanizeDate(DateTime.Parse(EmbedContent.Timestamp), null);
                    FooterText.Text = footertext;
                }
                else
                {
                    FooterText.Text = footertext;
                }
                if (EmbedContent.Footer != null && EmbedContent.Footer.IconUrl != null)
                {
                    FooterImage.Visibility = Visibility.Visible;
                    FooterImage.Source = new BitmapImage(new Uri(EmbedContent.Footer.IconUrl));
                }
                else
                {
                    FooterImage.Visibility = Visibility.Collapsed;
                }
            }

            // Provider
            if (EmbedContent.Provider != null)
            {
                ProviderHyperlink.Visibility = Visibility.Visible;
                ProviderHyperlink.Content = EmbedContent.Provider.Name;
                if (EmbedContent.Provider.Url != null)
                    ProviderHyperlink.NavigateUri = new Uri(EmbedContent.Provider.Url);
            }
            else
            {
                ProviderHyperlink.Visibility = Visibility.Collapsed;
            }

            // Thumbnail
            if (EmbedContent.Thumbnail != null)
            {
                if (EmbedContent.Type == "article")
                {
                    ThumbnailColumn.Width = new GridLength(0, GridUnitType.Pixel);
                    ThumbnailImage.Visibility = Visibility.Collapsed;
                    ImageViewbox.Visibility = Visibility.Visible;
                    ImageViewer.Source = new BitmapImage(new Uri(EmbedContent.Thumbnail.Url));
                }
                else
                {
                    ThumbnailColumn.Width = new GridLength(1, GridUnitType.Star);
                    ThumbnailImage.Visibility = Visibility.Visible;
                    ThumbnailImage.Source = new BitmapImage(new Uri(EmbedContent.Thumbnail.Url));
                }
            }
            else
            {
                ThumbnailColumn.Width = new GridLength(0, GridUnitType.Pixel);
                ThumbnailImage.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Creates a UIElement (StackPanel) for field in Embed
        /// </summary>
        /// <param name="field">Field to add</param>
        /// <returns>StackPanel display of field</returns>
        private StackPanel GenerateField(EmbedField field)
        {
            StackPanel sp = new StackPanel();
            if (field.Name != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock() { Text = field.Name, FontSize = 13, EnableHiddenLinks = true, FontWeight = FontWeights.SemiBold });
            if (field.Value != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock() { Text = field.Value, FontSize = 13, Opacity = 0.75, EnableHiddenLinks = true });
            if (field.Inline)
            {
                sp.MinWidth = 150;
                sp.MaxWidth = 204;
            }
            sp.Margin = new Thickness(0, 6, 0, 0);
            return sp;
        }

        public EmbedControl()
        {
            this.InitializeComponent();
            RegisterPropertyChangedCallback(EmbedContentProperty, OnPropertyChanged);
        }

        /// <summary>
        /// Open Attachment full window
        /// </summary>
        private void AttachedImageViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (EmbedContent.Image != null)
            {
                App.OpenAttachement(new Attachment()
                {
                    Filename = new Uri(EmbedContent.Image.Url).Segments.Last(),
                    Height = EmbedContent.Image.Height,
                    Width = EmbedContent.Image.Width,
                    Url = EmbedContent.Image.Url,
                    ProxyUrl = EmbedContent.Image.ProxyUrl,
                    Size = 0
                });
            }
            else if(EmbedContent.Thumbnail != null)
            {
                App.OpenAttachement(new Attachment()
                {
                    Filename = new Uri(EmbedContent.Thumbnail.Url).Segments.Last(),
                    Height = EmbedContent.Thumbnail.Height,
                    Width = EmbedContent.Thumbnail.Width,
                    Url = EmbedContent.Thumbnail.Url,
                    ProxyUrl = EmbedContent.Thumbnail.ProxyUrl,
                    Size = 0
                });
            }
            
        }

        DataTransferManager transfermanager;

        #region Sharing
        /// <summary>
        /// Share with Windows Sharing
        /// </summary>
        private void ShareEmbed(object sender, RoutedEventArgs e)
        {
            transfermanager = Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView();
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            transfermanager.DataRequested += EmbedControl_DataRequested;
        }
        #endregion

        /// <summary>
        /// Give Data for Windows Share
        /// </summary>
        private void EmbedControl_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            if (!string.IsNullOrEmpty(EmbedContent.Url))
            {
                args.Request.Data.SetText(EmbedContent.Url);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            }
            else
            {
                args.Request.FailWithDisplayText("Nothing to share");
            }
        }

        /// <summary>
        /// Unloaded
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of this object
        /// </summary>
        public void Dispose()
        {
            if (transfermanager != null)
                transfermanager.DataRequested -= EmbedControl_DataRequested;
        }

        /// <summary>
        /// Save Picture flyout (right-tapped)
        /// </summary>
        private void ImageViewer_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                App.ShowMenuFlyout(this, EmbedContent.Image.Url, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Save Picture flyout (holding)
        /// </summary>
        private void ImageViewer_Holding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                App.ShowMenuFlyout(this, EmbedContent.Image.Url, e.GetPosition(this));
            }
        }

        /// <summary>
        /// Show Embed Preview
        /// </summary>
        private void ShowPreview(object sender, RoutedEventArgs e)
        {
            ShowPreview();
        }

        /// <summary>
        /// Show Embed Preview
        /// </summary>
        private async void ShowPreview()
        {
            switch (type)
            {
                case Type.Image:
                    if (EmbedContent.Image.Url.EndsWith(".svg"))
                    {
                        ImageViewer.Source = new SvgImageSource(new Uri(EmbedContent.Image.Url));
                    }
                    else
                    {
                        ImageViewer.Source = new BitmapImage(new Uri(EmbedContent.Image.Url));
                    }
                    PreviewButton.Visibility = Visibility.Collapsed;
                    ImageViewbox.Visibility = Visibility.Visible;
                    LoadingImage.Visibility = Visibility.Visible;
                    LoadingImage.IsActive = true;
                    break;
                case Type.Audio:
                    PreviewButton.Visibility = Visibility.Collapsed;
                    player.HorizontalAlignment = HorizontalAlignment.Stretch;
                    player.Source = new Uri(EmbedContent.Video.Url);
                    player.Visibility = Visibility.Visible;
                    player.Height = 48;
                    break;
                case Type.Video:
                    PreviewButton.Visibility = Visibility.Collapsed;
                    player.Source = new Uri(EmbedContent.Video.Url);
                    player.Visibility = Visibility.Visible;
                    player.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                //case Type.PDF:
                //    LoadingImage.Visibility = Visibility.Visible;
                //    LoadingImage.IsActive = true;
                //    HttpClient client = new HttpClient();
                //    var stream = await
                //        client.GetStreamAsync(DisplayedAttachement.Url);
                //    var memStream = new MemoryStream();
                //    await stream.CopyToAsync(memStream);
                //    memStream.Position = 0;
                //    PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());
                //    LoadPDF(doc);
                //    break;
            }
            
        }

        /// <summary>
        /// Image Loaded Successfully
        /// </summary>
        private void ImageOpened(object sender, RoutedEventArgs e)
        {
            LoadingImage.Visibility = Visibility.Collapsed;
            LoadingImage.IsActive = false;
        }
    }
}
