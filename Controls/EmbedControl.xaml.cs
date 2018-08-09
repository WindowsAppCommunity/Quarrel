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
using Discord_UWP.SharedModels;
using Gregstoll;
using Discord_UWP.MarkdownTextBlock;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class EmbedControl : UserControl
    {
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

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            //bool everythingisnull = true;
            //Accent color
            if (EmbedContent.Color != 0)
                SideBorder.Background = Common.IntToColor(EmbedContent.Color);
            else
                SideBorder.Background = (SolidColorBrush)App.Current.Resources["LightBG"];

            //Author
            if (EmbedContent.Author != null)
            {
                //everythingisnull = false;
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
                //everythingisnull = false;
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
            //Description
            if (EmbedContent.Description != null)
            {
                //everythingisnull = false;
                DescriptionBlock.Visibility = Visibility.Visible;
                DescriptionBlock.Text = EmbedContent.Description;
            }
            else
            {
                DescriptionBlock.Visibility = Visibility.Collapsed;
            }

            //Fields
            UniversalWrapPanel previousWrapPanel= null;
            FieldsStacker.Children.Clear();
            if (EmbedContent.Fields != null)
            {
                //everythingisnull = false;
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

            //Image
            if (EmbedContent.Image != null)
            {
                //everythingisnull = false;
                ImageViewbox.Visibility = Visibility.Visible;
                ImageViewer.Source = new BitmapImage(new Uri(EmbedContent.Image.Url));
                if((EmbedContent.Author == null || (EmbedContent.Author.Name == null && EmbedContent.Author.IconUrl == null))
                    && EmbedContent.Description == null && EmbedContent.Fields != null
                    && EmbedContent.Fields.Count() == 0  && EmbedContent.Footer.Text == null
                    && EmbedContent.Footer.IconUrl == null && EmbedContent.Title == null)
                {
                    stacker.Margin = new Thickness(0);
                    SideBorder.Width = 0;
                }
            }
            else
            {
                ImageViewbox.Visibility = Visibility.Collapsed;
            }

            //Footer
            if (EmbedContent.Footer == null && EmbedContent.Timestamp == null)
                FooterSP.Visibility = Visibility.Collapsed;
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

            //Provider
            if (EmbedContent.Provider != null)
            {
                //everythingisnull = false;
                ProviderHyperlink.Visibility = Visibility.Visible;
                ProviderHyperlink.Content = EmbedContent.Provider.Name;
                if (EmbedContent.Provider.Url != null)
                    ProviderHyperlink.NavigateUri = new Uri(EmbedContent.Provider.Url);
            }
            else
            {
                ProviderHyperlink.Visibility = Visibility.Collapsed;
            }
            //Thumbnail
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
        private void ShareEmbed(object sender, RoutedEventArgs e)
        {
            transfermanager = Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView();
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
            transfermanager.DataRequested += EmbedControl_DataRequested;
        }

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

        public void Dispose()
        {
            if(transfermanager != null)
            transfermanager.DataRequested -= EmbedControl_DataRequested;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
