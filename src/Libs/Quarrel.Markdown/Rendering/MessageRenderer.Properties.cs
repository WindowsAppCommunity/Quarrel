// Quarrel © 2022

using Quarrel.Bindables.Messages;
using Quarrel.Markdown.Parsing;
using Windows.UI.Xaml;

namespace Quarrel.Markdown
{
    public sealed partial class MessageRenderer
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(MessageRenderer), new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(
            nameof(Context), typeof(BindableMessage), typeof(MessageRenderer), new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty CodeBlockStyleProperty = DependencyProperty.Register(
            nameof(CodeBlockStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty BlockQuoteStyleProperty = DependencyProperty.Register(
            nameof(BlockQuoteStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty EmojiStyleProperty = DependencyProperty.Register(
            nameof(EmojiStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty InlineCodeStyleProperty = DependencyProperty.Register(
            nameof(InlineCodeStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty SpoilerStyleProperty = DependencyProperty.Register(
            nameof(SpoilerStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty TimestampStyleProperty = DependencyProperty.Register(
            nameof(TimestampStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public static readonly DependencyProperty UserMentionStyleProperty = DependencyProperty.Register(
            nameof(UserMentionStyle), typeof(Style), typeof(MessageRenderer), new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public BindableMessage? Context
        {
            get => (BindableMessage)GetValue(ContextProperty);
            set => SetValue(ContextProperty, value);
        }

        public Style? CodeBlockStyle
        {
            get => (Style)GetValue(CodeBlockStyleProperty);
            set => SetValue(CodeBlockStyleProperty, value);
        }

        public Style? BlockQuoteStyle
        {
            get => (Style)GetValue(BlockQuoteStyleProperty);
            set => SetValue(BlockQuoteStyleProperty, value);
        }

        public Style? EmojiStyle
        {
            get => (Style)GetValue(EmojiStyleProperty);
            set => SetValue(EmojiStyleProperty, value);
        }

        public Style? InlineCodeStyle
        {
            get => (Style)GetValue(EmojiStyleProperty);
            set => SetValue(EmojiStyleProperty, value);
        }

        public Style? SpoilerStyle
        {
            get => (Style)GetValue(EmojiStyleProperty);
            set => SetValue(EmojiStyleProperty, value);
        }

        public Style? TimestampStyle
        {
            get => (Style)GetValue(EmojiStyleProperty);
            set => SetValue(EmojiStyleProperty, value);
        }

        public Style? UserMentionStyle
        {
            get => (Style)GetValue(EmojiStyleProperty);
            set => SetValue(EmojiStyleProperty, value);
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var messageRenderer = (MessageRenderer)d;
            var tree = Parser.ParseAST(messageRenderer.Text, true, false);
            messageRenderer.RenderMarkdown(tree);
        }
    }
}
