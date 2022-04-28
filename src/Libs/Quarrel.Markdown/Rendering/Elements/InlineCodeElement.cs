// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;

namespace Quarrel.Markdown
{
    public sealed class InlineCodeElement : MarkdownElement
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(MarkdownElement), new PropertyMetadata(string.Empty));

        internal InlineCodeElement(InlineCode inlineCode) : base(inlineCode)
        {
            this.DefaultStyleKey = typeof(InlineCodeElement);
            Text = inlineCode.Content;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
