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

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is Embed embed)
            {
                switch (embed.Type)
                {
                    case "image": return parent.FindResource<DataTemplate>("ImageEmbedTemplate");
                    case "gifv": return parent.FindResource<DataTemplate>("GifvEmbedTemplate");
                    case "video":
                    {
                        if (Regex.IsMatch(embed.Video?.Url, Helpers.Constants.Regex.YouTubeRegex))
                        {
                            return parent.FindResource<DataTemplate>("YoutubeEmbedTemplate");
                        }
                        break;
                    }
                    default: return parent.FindResource<DataTemplate>("DefaultEmbedTemplate");
                }
            }

            return null;
        }
    }
}
