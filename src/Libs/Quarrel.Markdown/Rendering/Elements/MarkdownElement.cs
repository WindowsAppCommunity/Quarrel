// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Markdown
{
    public abstract class MarkdownElement : Control
    {
        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register(
            nameof(TextDecorations), typeof(TextDecorations), typeof(MarkdownElement), new PropertyMetadata(TextDecorations.None));

        public static readonly DependencyProperty IsTextSelectionEnabledProperty = DependencyProperty.Register(
            nameof(IsTextSelectionEnabled), typeof(bool), typeof(MarkdownElement), new PropertyMetadata(false));

        internal MarkdownElement(IAST ast)
        {
        }

        public TextDecorations TextDecorations
        {
            get => (TextDecorations)GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }

        public bool IsTextSelectionEnabled
        {
            get => (bool)GetValue(IsTextSelectionEnabledProperty);
            set => SetValue(IsTextSelectionEnabledProperty, value);
        }
    }
}
