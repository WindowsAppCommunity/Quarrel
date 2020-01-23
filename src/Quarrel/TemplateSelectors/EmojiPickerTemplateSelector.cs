using Quarrel.ViewModels.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.TemplateSelectors
{
    public sealed class EmojiPickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UnicodeEmoji { get; set; }
        public DataTemplate GuildEmoji { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement && item is Emoji emoji)
            {
                if (emoji.CustomEmoji)
                    return GuildEmoji;
                else
                    return UnicodeEmoji;
            }

            return null;
        }
    }
}
