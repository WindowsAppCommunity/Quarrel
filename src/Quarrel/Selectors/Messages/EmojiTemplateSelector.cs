// Quarrel © 2022

using Quarrel.Client.Models.Emojis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.Messages
{
    public class EmojiTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? EmojiTemplate { get; set; }

        public DataTemplate? DiscordEmojiTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
        {
            return item is DiscordEmoji ? DiscordEmojiTemplate : EmojiTemplate;
        }
    }
}
