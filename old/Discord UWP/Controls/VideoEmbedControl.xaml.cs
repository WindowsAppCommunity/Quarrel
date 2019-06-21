using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

using myTube.Playback.Handlers;
using DiscordAPI.SharedModels;
using Ryken.Media.Core;
using Ryken.UI;
using RykenTube;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class VideoEmbedControl : UserControl
    {
        /// <summary>
        /// Embedded Content
        /// </summary>
        public Embed EmbedContent
        {
            get { return (Embed)GetValue(EmbedContentProperty); }
            set { SetValue(EmbedContentProperty, value); }
        }
        public static readonly DependencyProperty EmbedContentProperty = DependencyProperty.Register(
            nameof(EmbedContent),
            typeof(Embed),
            typeof(VideoEmbedControl),
            new PropertyMetadata(null, OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VideoEmbedControl;
            // Defer to the instance method.
            instance?.OnPropertyChanged(d, e.Property);
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            //Accent color
            if (EmbedContent.Color != 0)
                SideBorder.Background = Common.IntToColor(EmbedContent.Color);
            else
                SideBorder.Background = (SolidColorBrush)App.Current.Resources["LightBG"];

            //Author
            if (EmbedContent.Author != null)
            {
                AuthorSP.Visibility=Visibility.Visible;
                AuthorText.Content = EmbedContent.Author.Name;
                if(EmbedContent.Author.Url != null)
                    AuthorText.NavigateUri = new Uri(EmbedContent.Author.Url);
                if(EmbedContent.Author.IconUrl == null) AuthorImage.Visibility = Visibility.Collapsed;
                else
                { AuthorImage.Visibility = Visibility.Visible; AuthorImageBrush.ImageSource = new BitmapImage(new Uri(EmbedContent.Author.IconUrl)); }
            }
            else
            {
                AuthorSP.Visibility=Visibility.Collapsed;
            }
            //Title
            if (EmbedContent.Title != null)
            {
                if (EmbedContent.Url == null)
                {
                    // Hide title and url content
                    UrlTitleBlock.Visibility = Visibility.Collapsed;
                    PlainTitleBlock.Visibility = Visibility.Visible;
                    PlainTitleBlock.Text = EmbedContent.Title;
                }
                else
                {
                    // Show title and url
                    PlainTitleBlock.Visibility = Visibility.Collapsed;
                    UrlTitleBlock.Visibility = Visibility.Visible;
                    UrlTitleBlock.Content = EmbedContent.Title;
                    UrlTitleBlock.NavigateUri = new Uri(EmbedContent.Url);
                    
                    // For some reason, the creators update will crash if you show the icon in the wrong way
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
                // No title, collapse url and title
                PlainTitleBlock.Visibility = Visibility.Collapsed;
                UrlTitleBlock.Visibility = Visibility.Collapsed;
            }

            //Description
            if (EmbedContent.Description != null)
            {
                DescriptionBlock.Visibility = Visibility.Visible;
                DescriptionBlock.Text = EmbedContent.Description;
            }
            else
            {
                DescriptionBlock.Visibility = Visibility.Collapsed;
            }

            //Fields
            UniversalWrapPanel previousWrapPanel= null;

            // Clear fields
            FieldsStacker.Children.Clear();

            // If there are fields
            if (EmbedContent.Fields != null)
            {
                // Show field stacker
                FieldsStacker.Visibility = Visibility.Visible;
                foreach (var field in EmbedContent.Fields)
                {
                    // Add feed in line
                    if (field.Inline)
                    {
                        if (previousWrapPanel == null)
                            previousWrapPanel = new UniversalWrapPanel(){Orientation=Orientation.Horizontal};
                        previousWrapPanel.Children.Add(GenerateField(field));
                    }
                    else
                    {
                        // Add feed at end
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
                // No feeds, hide stacker
                FieldsStacker.Visibility = Visibility.Collapsed;
            }

            //Image
            if (EmbedContent.Image != null)
            {
                // Show Image
                ImageViewbox.Visibility = Visibility.Visible;
                ImageViewer.Source = new BitmapImage(new Uri(EmbedContent.Image.Url));

                // Show with no border if all other fields are null
                if(EmbedContent.Author.Name == null && EmbedContent.Author.IconUrl == null
                    && EmbedContent.Description == null && EmbedContent.Fields.Count() == 0 
                    && EmbedContent.Footer.Text == null && EmbedContent.Footer.IconUrl == null
                    && EmbedContent.Title == null)
                {
                    stacker.Margin = new Thickness(0);
                    SideBorder.Width = 0;
                }
            }
            else
            {
                // Hide image
                ImageViewbox.Visibility = Visibility.Collapsed;
            }

            //Footer
            // If there's no footer and no timestamp
            if (EmbedContent.Footer == null && EmbedContent.Timestamp == null)
            {
                // Hide footer
                FooterSP.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Show footer
                FooterSP.Visibility = Visibility.Visible;
                string footertext = "";

                // Footer
                if (EmbedContent.Footer.Text != null)
                    footertext = EmbedContent.Footer.Text;

                // Timestamp
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

                // Icon
                if (EmbedContent.Footer != null)
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
                // Show domain
                ProviderHyperlink.Visibility = Visibility.Visible;
                ProviderHyperlink.Content = EmbedContent.Provider.Name;
                if (EmbedContent.Provider.Url != null)
                    ProviderHyperlink.NavigateUri = new Uri(EmbedContent.Provider.Url);
            }
            else
            {
                // Hide domain view
                ProviderHyperlink.Visibility = Visibility.Collapsed;
            }

            // Thumbnail
            if (EmbedContent.Thumbnail != null)
            {
                // Article Thumbnail
                if (EmbedContent.Type == "article")
                {
                    // Hide thumbnail view
                    ThumbnailColumn.Width = new GridLength(0, GridUnitType.Pixel);
                    ThumbnailImage.Visibility = Visibility.Collapsed;

                    // Show article thumbnail
                    ImageViewbox.Visibility = Visibility.Visible;
                    ImageViewer.Source = new BitmapImage(new Uri(EmbedContent.Thumbnail.Url));
                }
                else
                {
                    // Other thumbnails
                    ThumbnailColumn.Width = new GridLength(1, GridUnitType.Star);
                    ThumbnailImage.Visibility = Visibility.Visible;
                    ThumbnailImage.Source = new BitmapImage(new Uri(EmbedContent.Thumbnail.Url));
                }
            }
            else
            {
                // Hide thumbnail view
                ThumbnailColumn.Width = new GridLength(0, GridUnitType.Pixel);
                ThumbnailImage.Visibility = Visibility.Collapsed;
            }

            // Show RykenTube player
            Regex YouTubeRegex = new Regex(@"(?:https:\/\/)?(?:(?:www\.)?youtube\.com\/watch\?.*?v=([\w\-]+)|youtu\.be\/([\w\-]+))", RegexOptions.Compiled);
            var match = YouTubeRegex.Match(EmbedContent.Url);
            if (match.Success)
            {
                EmbedView.Visibility = Visibility.Collapsed;
                RykenPlayer.Visibility = Visibility.Visible;

                if (RykenPlayer.CurrentMediaHandler == null)
                {
                    myTubeHandlerContainer mediaHandler = new myTubeHandlerContainer();
                    RykenPlayer.CurrentMediaHandler = mediaHandler;
                    await mediaHandler.SetCurrentVideoHandler(new MediaPlayerHandler { UseMediaPlayerElement = false });
                    mediaHandler.CurrentVideoHandler.HandlesTransportControls = true; // Disable the media transport controls
                    mediaHandler.CurrentVideoHandler.StopOnMediaEnded = false; // Keep video loaded when ended
                    await mediaHandler.CurrentVideoHandler.OpenVideo(new YouTubeEntry { ID = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value }, YouTubeQuality.HD);
                    await mediaHandler.CurrentVideoHandler.Pause();
                }
            }
        }
        /// <summary>
        /// Make StackPanel control from <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field to parse</param>
        /// <returns>StackPanel of Field Control</returns>
        private StackPanel GenerateField(EmbedField field)
        {
            StackPanel sp = new StackPanel();
            
            // If there's a name, add a name
            if (field.Name != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock() { Text = field.Name, FontSize = 13, EnableHiddenLinks = true, FontWeight = FontWeights.SemiBold });

            // If there's content, add content
            if (field.Value != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock() { Text = field.Value, FontSize = 13, Opacity = 0.75, EnableHiddenLinks = true });

            // If it's line, set a max width a height
            if (field.Inline)
            {
                sp.MinWidth = 150;
                sp.MaxWidth = 204;
            }

            // Add margin
            sp.Margin = new Thickness(0, 6, 0, 0);

            return sp;
        }

        public VideoEmbedControl()
        {
            this.InitializeComponent();
            RegisterPropertyChangedCallback(EmbedContentProperty, OnPropertyChanged);
        }

        /// <summary>
        /// Open image in SubPage
        /// </summary>
        private void AttachedImageViewer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // If there's an image
            if (EmbedContent.Image.Url != null)
            {
                // Open Image
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
            // If there's a thumbnail
            else if(EmbedContent.Thumbnail.Url != null)
            {
                // Open Thumbnail
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

        /// <summary>
        /// Open share prompt for embedded content
        /// </summary>
        private void ShareEmbed(object sender, RoutedEventArgs e)
        {
            // Show prompt
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += EmbedControl_DataRequested;
        }

        /// <summary>
        /// Requested from share prompt
        /// </summary>
        private void EmbedControl_DataRequested(Windows.ApplicationModel.DataTransfer.DataTransferManager sender, Windows.ApplicationModel.DataTransfer.DataRequestedEventArgs args)
        {
            // If there's content
            if (!string.IsNullOrEmpty(EmbedContent.Url))
            {
                // Share content
                args.Request.Data.SetText(EmbedContent.Url);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            }
            else
            {
                // Nothing to share
                args.Request.FailWithDisplayText("Nothing to share");
            }

        }

        /// <summary>
        /// Open in myTube
        /// </summary>
        private async void OpenInmyTube(object sender, RoutedEventArgs e)
        {
            // TODO: Switch to regex

            string videoID = "";
            if (EmbedContent.Url.Contains("youtube"))
            {
                int index = EmbedContent.Url.IndexOf("?v=");

                videoID = EmbedContent.Url.Substring(index + 3);

                if (videoID.Contains("&"))
                {
                    videoID = videoID.Substring(0, videoID.LastIndexOf("&"));
                }
            } else
            {
                int index = EmbedContent.Url.LastIndexOf('/');

                videoID = EmbedContent.Url.Substring(index + 1);

                if (videoID.Contains("?"))
                {
                    videoID = videoID.Substring(0, videoID.LastIndexOf("?"));
                }
            }
            
            // Plug in uri and launch
            var uri = new Uri(@"rykentube:Video?ID=" + videoID);
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
        
        /// <summary>
        /// Unload control
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Dispose of Control
        /// </summary>
        public void Dispose()
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested -= EmbedControl_DataRequested;
            RykenPlayer.CurrentMediaHandler.Stop();
        }

        
    }
}
