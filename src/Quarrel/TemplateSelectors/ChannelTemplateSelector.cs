// Copyright (c) Quarrel. All rights reserved.

using Quarrel.ViewModels.Models.Bindables.Channels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    /// <summary>
    /// A template selector for the channel type.
    /// </summary>
    public class ChannelTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the text channel template.
        /// </summary>
        public DataTemplate TextChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the DM channel template.
        /// </summary>
        public DataTemplate DMChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the voice channel template.
        /// </summary>
        public DataTemplate VoiceChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the group DM channel template.
        /// </summary>
        public DataTemplate GroupDMChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the category channel template.
        /// </summary>
        public DataTemplate CategoryChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the news channel template.
        /// </summary>
        public DataTemplate NewsChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the store channel template.
        /// </summary>
        public DataTemplate StoreChannelTemplate { get; set; }

        /// <summary>
        /// Selects a <see cref="DataTemplate"/> based on the details from <paramref name="item"/>.
        /// </summary>
        /// <param name="item">A <see cref="BindableChannel"/>.</param>
        /// <param name="container">The parent of the resulting <see cref="DataTemplate"/>.</param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>'s type.</returns>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is BindableChannel channel)
            {
                switch (channel.Model.Type)
                {
                    case 0:
                        return TextChannelTemplate;
                    case 1:
                        return DMChannelTemplate;
                    case 2:
                        return VoiceChannelTemplate;
                    case 3:
                        return GroupDMChannelTemplate;
                    case 4:
                        return CategoryChannelTemplate;
                    case 5:
                        return NewsChannelTemplate;
                    case 6:
                        return StoreChannelTemplate;
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
