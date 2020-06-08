// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Suggesitons;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the suggestion type to show.
    /// </summary>
    public sealed class SuggestionTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the pure image attachment template.
        /// </summary>
        public DataTemplate ChannelSuggestionTemplate { get; set; }

        /// <summary>
        /// Gets or sets the pure image attachment template.
        /// </summary>
        public DataTemplate UserSuggestionTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on details from the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An <see cref="ISuggestion"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement parent && item is ISuggestion attachment)
            {
                switch (item)
                {
                    case ChannelSuggestion _: return ChannelSuggestionTemplate;
                    case UserSuggestion _: return UserSuggestionTemplate;
                }
            }

            return null;
        }
    }
}
