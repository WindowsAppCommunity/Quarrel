// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;

namespace Quarrel.Markdown
{
    public abstract class MarkdownTextElement : MarkdownElement
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(MarkdownTextElement), new PropertyMetadata(string.Empty));

        internal MarkdownTextElement(IAST ast) : base(ast)
        {
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
