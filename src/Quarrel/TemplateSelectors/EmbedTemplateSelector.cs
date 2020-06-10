// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Models.Interfaces;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the Embed type to show.
    /// </summary>
    public sealed class EmbedTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the pure image Embed template.
        /// </summary>
        public DataTemplate ImageEmbedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the pure Gifv Embed template.
        /// </summary>
        public DataTemplate GifvEmbedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the pure YouTube Embed template.
        /// </summary>
        public DataTemplate YoutubeEmbedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the rich Embed template.
        /// </summary>
        public DataTemplate RichEmbedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the dynamic Embed template.
        /// </summary>
        public DataTemplate DefaultEmbedTemplate { get; set; }

        /// <summary>
        /// Gets or sets the invite template.
        /// </summary>
        public DataTemplate InviteTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="IEmbed"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BindableInvite invite)
            {
                return InviteTemplate;
            }

            if (item is BindableEmbed embed)
            {
                switch (embed.Model.Type)
                {
                    case "image": return ImageEmbedTemplate;
                    case "gifv": return GifvEmbedTemplate;
                    case "video":
                        {
                            if (Regex.IsMatch(embed.Model.Video?.Url, ViewModels.Helpers.Constants.Regex.YouTubeURLRegex))
                            {
                                return YoutubeEmbedTemplate;
                            }

                            return DefaultEmbedTemplate;
                        }

                    case "rich": return RichEmbedTemplate;
                    default: return DefaultEmbedTemplate;
                }
            }

            return null;
        }
    }
}
