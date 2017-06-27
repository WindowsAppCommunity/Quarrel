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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class EmbedControl : UserControl
    {
        public Embed Content
        {
            get { return (Embed)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
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
            bool everythingisnull = true;
            //Accent color
            if (Content.Color != 0)
                SideBorder.Background = Common.IntToColor(Content.Color);
            else
                SideBorder.Background = (SolidColorBrush)App.Current.Resources["LightBG"];

            //Author
            if (Content.Author.Name != null)
            {
                everythingisnull = false;
                AuthorSP.Visibility=Visibility.Visible;
                AuthorText.Content = Content.Author.Name;
                if(Content.Author.Url != null)
                    AuthorText.NavigateUri = new Uri(Content.Author.Url);
                if(Content.Author.IconUrl == null) AuthorImage.Visibility = Visibility.Collapsed;
                else
                { AuthorImage.Visibility = Visibility.Visible; AuthorImageBrush.ImageSource = new BitmapImage(new Uri(Content.Author.IconUrl)); }
            }
            else
            {
                AuthorSP.Visibility=Visibility.Collapsed;
            }
            //Title
            if (Content.title != null)
            {
                everythingisnull = false;
                if (Content.Url == null)
                {
                    UrlTitleBlock.Visibility = Visibility.Collapsed;
                    PlainTitleBlock.Visibility = Visibility.Visible;
                    PlainTitleBlock.Text = Content.title;
                }
                else
                {
                    PlainTitleBlock.Visibility = Visibility.Collapsed;
                    UrlTitleBlock.Visibility = Visibility.Visible;
                    UrlTitleBlock.Content = Content.title;
                    UrlTitleBlock.NavigateUri = new Uri(Content.Url);
                }
            }
            else
            {
                PlainTitleBlock.Visibility = Visibility.Collapsed;
                UrlTitleBlock.Visibility = Visibility.Collapsed;
            }
            //Description
            if (Content.Description != null)
            {
                everythingisnull = false;
                DescriptionBlock.Visibility = Visibility.Visible;
                DescriptionBlock.Text = Content.Description;
            }
            else
            {
                DescriptionBlock.Visibility = Visibility.Collapsed;
            }

            //Fields
            UniversalWrapPanel previousWrapPanel= null;
            FieldsStacker.Children.Clear();
            if (Content.Fields != null)
            {
                everythingisnull = false;
                FieldsStacker.Visibility = Visibility.Visible;
                foreach (var field in Content.Fields)
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
            if (Content.Image.Url != null)
            {
                everythingisnull = false;
                ImageViewbox.Visibility = Visibility.Visible;
                ImageViewer.Source = new BitmapImage(new Uri(Content.Image.Url));
            }
            else
            {
                ImageViewbox.Visibility = Visibility.Collapsed;
            }

            //Footer
            if (Content.Footer.Text == null && Content.Timestamp == null)
                FooterSP.Visibility = Visibility.Collapsed;
            else
            {
                FooterSP.Visibility = Visibility.Visible;
                string footertext = "";
                if (Content.Footer.Text != null)
                    footertext = Content.Footer.Text;
                if (Content.Timestamp != null)
                {
                    if (footertext != "") footertext += " | ";
                    footertext += Common.HumanizeDate(DateTime.Parse(Content.Timestamp), null);
                    FooterText.Text = footertext;
                }
                else
                {
                    FooterText.Text = footertext;
                }
                if (Content.Footer.IconUrl != null)
                {
                    FooterImage.Visibility = Visibility.Visible;
                    FooterImage.Source = new BitmapImage(new Uri(Content.Footer.IconUrl));
                }
                else
                {
                    FooterImage.Visibility = Visibility.Collapsed;
                }
            }

            //Provider
            if (Content.Provider.Name != null)
            {
                everythingisnull = false;
                ProviderHyperlink.Visibility = Visibility.Visible;
                ProviderHyperlink.Content = Content.Provider.Name;
                if (Content.Provider.Url != null)
                    ProviderHyperlink.NavigateUri = new Uri(Content.Provider.Url);
            }
            else
            {
                ProviderHyperlink.Visibility = Visibility.Collapsed;
            }
            //Thumbnail
            if (Content.Thumbnail.Url != null)
            {
                if (everythingisnull)
                {
                    HeaderGrid.Visibility = Visibility.Collapsed;
                    ImageViewbox.Visibility = Visibility.Visible;
                    ImageViewer.Source  = new BitmapImage(new Uri(Content.Thumbnail.Url));
                }
                else
                {
                    //If the aspect ratio of the thumbnail is higher than 1.5 and there is no image, display the thumbnail at the place of the large image
                    if (Content.Thumbnail.Width / Content.Thumbnail.Height > 1.4 && ImageViewbox.Visibility == Visibility.Collapsed)
                    {
                        ThumbnailColumn.Width = new GridLength(0, GridUnitType.Pixel);
                        ThumbnailImage.Visibility = Visibility.Collapsed;
                        ImageViewbox.Visibility = Visibility.Visible;
                        ImageViewer.Source = new BitmapImage(new Uri(Content.Thumbnail.Url));
                    }
                    else
                    {
                        ThumbnailColumn.Width = new GridLength(1, GridUnitType.Star);
                        ThumbnailImage.Visibility = Visibility.Visible;
                        ThumbnailImage.Source = new BitmapImage(new Uri(Content.Thumbnail.Url));
                    }

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
            double minwidth = 0;
            double maxwidth = Double.NaN;
            
            if(field.Name != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock(){ Text=field.Name, FontSize=13, EnableHiddenLinks=true, FontWeight=FontWeights.SemiBold });
            if(field.Value != null)
                sp.Children.Add(new MarkdownTextBlock.MarkdownTextBlock() { Text=field.Value, FontSize=13, Opacity=0.75, EnableHiddenLinks=true});
            if (field.Inline)
            {
                sp.MinWidth = 150;
                sp.MaxWidth = 204;
            }
            sp.Margin = new Thickness(0,6,0,0);
            return sp;
        }
        public EmbedControl()
        {
            this.InitializeComponent();
            RegisterPropertyChangedCallback(ContentProperty, OnPropertyChanged);
        }
    }
}
