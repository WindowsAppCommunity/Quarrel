// Adam Dernis © 2022

using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors
{
    /// <summary>
    /// A template selector for the channel type.
    /// </summary>
    public class ChannelStyleSelector : StyleSelector
    {
        /// <summary>
        /// Gets or sets the text channel style.
        /// </summary>
        public Style TextChannelStyle { get; set; }

        /// <summary>
        /// Gets or sets the category channel style.
        /// </summary>
        public Style CategoryChannelStyle { get; set; }

        /// <inheritdoc/>
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is BindableChannel channel)
            {
                return channel switch
                {
                    BindableTextChannel => TextChannelStyle,
                    BindableCategoryChannel => CategoryChannelStyle,
                    _ => null,
                };
            }

            return null;
        }
    }
}
