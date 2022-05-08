// Quarrel © 2022

using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.Channels
{
    /// <summary>
    /// A template selector for the channel type.
    /// </summary>
    public class ChannelTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the text channel template.
        /// </summary>
        public DataTemplate? TextChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the voice channel template.
        /// </summary>
        public DataTemplate? VoiceChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the direct channel template.
        /// </summary>
        public DataTemplate? DirectChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the group channel template.
        /// </summary>
        public DataTemplate? GroupChannelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the category channel template.
        /// </summary>
        public DataTemplate? CategoryChannelTemplate { get; set; }

        /// <inheritdoc/>
        protected override DataTemplate? SelectTemplateCore(object item)
        {
            if (item is BindableChannel channel)
            {
                return channel switch
                {
                    BindableTextChannel => TextChannelTemplate,
                    BindableVoiceChannel => VoiceChannelTemplate,
                    BindableDirectChannel => DirectChannelTemplate,
                    BindableGroupChannel => GroupChannelTemplate,
                    BindableCategoryChannel => CategoryChannelTemplate,
                    _ => null,
                };
            }

            return null;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
