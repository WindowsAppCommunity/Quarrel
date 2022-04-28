// Quarrel © 2022

using ColorCode;
using Humanizer;
using Quarrel.Bindables.Messages;
using Quarrel.Markdown.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = nameof(RichBlockPartName), Type = typeof(RichTextBlock))]
    public sealed class MessageRenderer : Control
    {
        private const string RichBlockPartName = "RichBlock";

        private const bool IsTextSelectable = false;
        private const bool IsCodeSelectable = true;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(MessageRenderer), new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty ContextProperty = DependencyProperty.Register(
            nameof(Context), typeof(BindableMessage), typeof(MessageRenderer), new PropertyMetadata(null, OnPropertyChanged));

        private RichTextBlock? _richBlock;

        public MessageRenderer()
        {
            this.DefaultStyleKey = typeof(MessageRenderer);
        }

        protected override void OnApplyTemplate()
        {
            _richBlock = (RichTextBlock)GetTemplateChild(RichBlockPartName);
        }

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

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var messageRenderer = (MessageRenderer)d;

            if (messageRenderer.Context is null)
            {
                // TODO: Allow context free rendering
                return;
            }

            var tree = Parser.ParseAST(messageRenderer.Text, true, false);
            var modTree = AdjustTree(tree);
            messageRenderer.RenderMarkdown(modTree);
        }

        private void RenderMarkdown(IList<ASTRoot> tree)
        {
            BlockCollection blocks = _richBlock.Blocks;
            blocks.Clear();

            foreach (var root in tree)
            {
                var paragraph = new Paragraph();
                blocks.Add(paragraph);
                InlineCollection inlineCollection = paragraph.Inlines;
                Stack<(IAST, InlineCollection)> stack = new Stack<(IAST, InlineCollection)>();
                foreach (IAST ast in root.Children.Reverse())
                {
                    stack.Push((ast, inlineCollection));
                }

                while (stack.TryPop(out var tuple))
                {
                    (IAST node, inlineCollection) = tuple;
                    switch (node)
                    {
                        case CodeBlock codeBlock:
                            {
                                var container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new CodeBlockElement(codeBlock)
                                {
                                    FontSize = container.FontSize,
                                    FontStretch = container.FontStretch,
                                    TextDecorations = container.TextDecorations,
                                };
                            }
                            break;
                        case BlockQuote blockQuote:
                            {
                                var container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                var element = new BlockQuoteElement(blockQuote)
                                {
                                    FontSize = container.FontSize,
                                    FontStretch = container.FontStretch,
                                    TextDecorations = container.TextDecorations,
                                };
                                container.Child = element;

                                foreach (IAST child in blockQuote.Children.Reverse())
                                {
                                    stack.Push((child, element.Paragraph.Inlines));
                                }
                            }
                            break;
                        case Url url:
                            inlineCollection.Add(new Hyperlink() { NavigateUri = new Uri(url.Content), Inlines = { new Run() { Text = url.Content } } });
                            break;
                        case IEmojiAST emoji:
                            {
                                var container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new EmojiElement(emoji);
                            }
                            break;
                        case Strong strong:
                            {
                                var inline = new Bold();
                                inlineCollection.Add(inline);
                                foreach (IAST child in strong.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case Em em:
                            {
                                var inline = new Italic();
                                inlineCollection.Add(inline);
                                foreach (IAST child in em.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case U u:
                            {
                                var inline = new Underline();
                                inlineCollection.Add(inline);
                                foreach (IAST child in u.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case S s:
                            {
                                var inline = new Span() { TextDecorations = TextDecorations.Strikethrough };
                                inlineCollection.Add(inline);
                                foreach (IAST child in s.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case InlineCode inlineCode:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Child = new TextBlock()
                                    {
                                        FontFamily = new FontFamily("Consolas"),
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsCodeSelectable,
                                        Text = inlineCode.Content
                                    }
                                };
                            }
                            break;
                        case Timestamp timeStamp:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Child = new TextBlock()
                                    {
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Text = timeStamp.Format switch
                                        {
                                            "F" or "" => timeStamp.Time.ToString("F"),
                                            "D" => timeStamp.Time.ToString("d MMMM yyyy"),
                                            "R" => timeStamp.Time.Humanize(),
                                            "T" => timeStamp.Time.ToString("T"),
                                            "d" => timeStamp.Time.ToString("d"),
                                            "f" => timeStamp.Time.ToString("MMMM yyyy HH:mm"),
                                            "t" => timeStamp.Time.ToString("t"),
                                        }
                                    }
                                };
                            }
                            break;
                        case RoleMention roleMention:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new HyperlinkButton()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(0),
                                    Content = new TextBlock()
                                    {
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Text = roleMention.RoleID
                                    }
                                };
                            }
                            break;
                        case UserMention mention:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new HyperlinkButton()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(0),
                                    Content = new TextBlock()
                                    {
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Text = Context.Users[mention.UserID].User.Username,
                                    }
                                };
                            }
                            break;
                        case GlobalMention globalMention:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new HyperlinkButton()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(0),
                                    Content = new TextBlock()
                                    {
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Text = globalMention.Target,
                                    }
                                };
                            }
                            break;
                        case ChannelMention channel:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new HyperlinkButton()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(0),
                                    Content = new TextBlock()
                                    {
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Text = channel.ChannelID
                                    }
                                };
                            }
                            break;
                        case Spoiler spoiler:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                var codeParagraph = new Paragraph();
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.Red),
                                    Child = new RichTextBlock()
                                    {
                                        FontFamily = container.FontFamily,
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = IsTextSelectable,
                                        Blocks = { codeParagraph },
                                    }
                                };
                                foreach (IAST child in spoiler.Children.Reverse())
                                {
                                    stack.Push((child, codeParagraph.Inlines));
                                }
                            }
                            break;
                        case Text textNode:
                            inlineCollection.Add(new Run() { Text = textNode.Content });
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Hacky function to force split code blocks and block quotes onto new lines
        /// </summary>
        private static IList<ASTRoot> AdjustTree(IList<IAST> tree)
        {
            // TODO: modify tree inpace
            List<ASTRoot> newTree = new List<ASTRoot>() { new ASTRoot(new List<IAST>()) };
            Stack<IAST> stack = new Stack<IAST>();
            foreach (IAST child in tree.Reverse())
            {
                stack.Push(child);
            }
            List<(int, ASTChildren)> depths = new List<(int, ASTChildren)>();
            depths.Add((stack.Count, newTree[0]));
            while (stack.TryPop(out IAST node))
            {
                int depth;
                ASTChildren current;
                do
                {
                    (depth, current) = depths[depths.Count - 1];
                    depths.RemoveAt(depths.Count - 1);
                } while (depth <= 0);
                depth--;
                depths.Add((depth, current));
                switch (node)
                {
                    case BlockQuote:
                    case CodeBlock:
                        {
                            (int, ASTChildren)[] newDepths = new (int, ASTChildren)[depths.Count];

                            ASTChildren currentCodeBlockNode = current with
                            {
                                Children = new List<IAST>() { node }
                            };

                            newDepths[newDepths.Length - 1] = (depth, current with { Children = new List<IAST>() });
                            for (int i = depths.Count - 2; i >= 0; i--)
                            {
                                (int oldDepth, ASTChildren oldNode) = depths[i];
                                newDepths[i] = (oldDepth,
                                    oldNode with { Children = new List<IAST>() { newDepths[i + 1].Item2 } });

                                currentCodeBlockNode = oldNode with
                                {
                                    Children = new List<IAST>() { currentCodeBlockNode }
                                };
                            }

                            newTree.Add((ASTRoot)currentCodeBlockNode);
                            depths = newDepths.ToList();
                            newTree.Add((ASTRoot)newDepths[0].Item2);
                        }
                        break;
                    case ASTChildren astChildrenNode:
                        {
                            foreach (IAST child in astChildrenNode.Children.Reverse())
                            {
                                stack.Push(child);
                            }

                            var tmp = astChildrenNode with { Children = new List<IAST>() };
                            current.Children.Add(tmp);
                            depths.Add((astChildrenNode.Children.Count, tmp));
                        }
                        break;
                    default:
                        current.Children.Add(node);
                        break;
                }
            }

            return newTree;
        }
    }
}
