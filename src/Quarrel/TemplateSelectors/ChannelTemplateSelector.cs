// Adam Dernis © 2022

using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
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
        /// Gets or sets the category channel template.
        /// </summary>
        public DataTemplate CategoryChannelTemplate { get; set; }

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
                return channel switch
                {
                    BindableTextChannel => TextChannelTemplate,
                    BindableCategoryChannel => CategoryChannelTemplate,
                    _ => null,
                };
            }

            return null;
        }
    }
}
