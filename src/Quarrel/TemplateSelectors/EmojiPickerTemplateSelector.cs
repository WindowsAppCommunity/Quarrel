// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Emojis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the Emoji type.
    /// </summary>
    public sealed class EmojiPickerTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the unicode emoji template.
        /// </summary>
        public DataTemplate UnicodeEmoji { get; set; }

        /// <summary>
        /// Gets or sets the guild emoji template.
        /// </summary>
        public DataTemplate GuildEmoji { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="Emoji"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement && item is Emoji emoji)
            {
                if (emoji.CustomEmoji)
                {
                    return GuildEmoji;
                }
                else
                {
                    return UnicodeEmoji;
                }
            }

            return null;
        }
    }
}
