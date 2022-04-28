// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = ImagePartName, Type = typeof(Image))]
    public sealed class EmojiElement : MarkdownElement
    {
        private const string ImagePartName = "EmojiImage";

        private readonly IEmojiAST _emoji;

        internal EmojiElement(IEmojiAST emoji) : base(emoji)
        {
            this.DefaultStyleKey = typeof(EmojiElement);
            _emoji = emoji;
        }

        protected override void OnApplyTemplate()
        {
            var image = (Image)GetTemplateChild(ImagePartName);
            Render(image);
        }

        private void Render(Image image)
        {
            // TODO: Dynamically determine height
            double height = 22;
            image.Source = _emoji switch
            {
                SurrogateEmoji se => new SvgImageSource(new Uri($"ms-appx:///Quarrel.Markdown/Assets/Emoji/{EmojiTable.ToCodePoint(se.Surrogate).ToLower()}.svg")),
                Emoji e => new BitmapImage(new Uri($"https://cdn.discordapp.com/emojis/{e.Id}.webp?size={2 * height}&quality=lossless")),
                AnimatedEmoji ae => new BitmapImage(new Uri($"https://cdn.discordapp.com/emojis/{ae.Id}.gif?size={2 * height}&quality=lossless"))
                {
                    AutoPlay = true,
                },

                _ => null,
            };
        }
    }
}
