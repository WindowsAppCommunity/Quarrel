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
                    BindableCategoryChannel => CategoryChannelTemplate,
                    BindableVoiceChannel => VoiceChannelTemplate,
                    _ => null,
                };
            }

            return null;
        }
    }
}
