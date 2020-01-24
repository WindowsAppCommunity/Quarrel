using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Quarrel.Controls.Messages.Embeds;
using DiscordAPI.Models;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the line to display in the console view
    /// </summary>
    public sealed class EmbedTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageEmbedTemplate { get; set; }
        public DataTemplate GifvEmbedTemplate { get; set; }
        public DataTemplate YoutubeEmbedTemplate { get; set; }
        public DataTemplate DefaultEmbedTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is Embed embed)
            {
                switch (embed.Type)
                {
                    case "image": return ImageEmbedTemplate;
                    case "gifv": return GifvEmbedTemplate;
                    case "video":
                    {
                        if (Regex.IsMatch(embed.Video?.Url, ViewModels.Helpers.Constants.Regex.YouTubeURLRegex))
                        {
                            return YoutubeEmbedTemplate;
                        }
                        break;
                    }
                    default: return DefaultEmbedTemplate;
                }
            }

            return null;
        }
    }
}
