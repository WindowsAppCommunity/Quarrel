// Quarrel © 2022

using ColorCode;
using Humanizer;
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

namespace Quarrel.Controls.Message
{
    public sealed partial class MessageRenderer : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(MessageRenderer),
            new PropertyMetadata(null, OnTextChanged)
        );

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var messageRenderer = (MessageRenderer)d;
            messageRenderer.textBlock.Blocks.Clear();
            var parser = new Parser();
            var tree = Parser.ParseAST((string)e.NewValue, true, false);
            var modTree = AdjustTree(tree);
            Parse(modTree, messageRenderer.textBlock.Blocks);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public MessageRenderer()
        {
            this.InitializeComponent();
        }

        private static void Parse(IList<ASTRoot> tree, BlockCollection blocks)
        {
            foreach (var root in tree)
            {
                var paragraph = new Paragraph();
                blocks.Add(paragraph);
                InlineCollection inlineCollection = paragraph.Inlines;
                Stack<(AST, InlineCollection)> stack = new Stack<(AST, InlineCollection)>();
                foreach (AST ast in root.Children.Reverse())
                {
                    stack.Push((ast, inlineCollection));
                }

                while (stack.TryPop(out var tuple))
                {
                    (AST node, inlineCollection) = tuple;
                    switch (node)
                    {
                        case CodeBlock codeBlock when !string.IsNullOrEmpty(codeBlock.Language) &&
                                                      Languages.FindById(codeBlock.Language) is { } language:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                var codeParagraph = new Paragraph();
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(4),
                                    Child = new RichTextBlock()
                                    {
                                        FontFamily = new FontFamily("Consolas"),
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        Blocks = { codeParagraph }
                                    }
                                };
                                RichTextBlockFormatter a = new RichTextBlockFormatter(ElementTheme.Dark);
                                a.FormatInlines(codeBlock.Content, language, codeParagraph.Inlines);
                            }
                            break;
                        case CodeBlock codeBlock:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Background = new SolidColorBrush(Colors.DarkGray),
                                    Padding = new Thickness(4),
                                    Child = new TextBlock()
                                    {
                                        FontFamily = new FontFamily("Consolas"),
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        IsTextSelectionEnabled = true,
                                        Text = codeBlock.Content
                                    }
                                };
                            }
                            break;
                        case BlockQuote blockQuote:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                var codeParagraph = new Paragraph();
                                container.Child = new Border()
                                {
                                    RenderTransform = new TranslateTransform { Y = 4 },
                                    Padding = new Thickness(10, 0, 0, 0),
                                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                                    BorderThickness = new Thickness(2, 0, 0, 0),
                                    Child = new RichTextBlock()
                                    {
                                        FontFamily = container.FontFamily,
                                        FontWeight = container.FontWeight,
                                        FontSize = container.FontSize,
                                        FontStretch = container.FontStretch,
                                        TextDecorations = container.TextDecorations,
                                        Blocks = { codeParagraph }
                                    }
                                };
                                foreach (AST child in blockQuote.Children.Reverse())
                                {
                                    stack.Push((child, codeParagraph.Inlines));
                                }

                            }
                            break;
                        case Url url:
                            inlineCollection.Add(new Hyperlink() { NavigateUri = new Uri(url.Content), Inlines = { new Run() { Text = url.Content } } });
                            break;
                        case SurrogateEmoji surrogateEmoji:
                            {
                                int height = 22;
                                InlineUIContainer container = new InlineUIContainer()
                                {
                                    Child = new Image()
                                    {
                                        RenderTransform = new TranslateTransform { Y = 4 },
                                        Height = height,
                                        Source = new SvgImageSource(new Uri($"ms-appx:///Assets/Emoji/{EmojiTable.ToCodePoint(surrogateEmoji.Surrogate).ToLower()}.svg"))
                                    }
                                };
                                inlineCollection.Add(container);
                            }
                            break;
                        case Emoji emoji:
                            {
                                int height = 22;
                                InlineUIContainer container = new InlineUIContainer()
                                {
                                    Child = new Image()
                                    {
                                        RenderTransform = new TranslateTransform { Y = 4 },
                                        Height = height,
                                        Source = new BitmapImage()
                                        {
                                            UriSource = new Uri($"https://cdn.discordapp.com/emojis/{emoji.Id}.webp?size={2 * height}&quality=lossless")
                                        }
                                    }
                                };
                                inlineCollection.Add(container);
                            }
                            break;
                        case AnimatedEmoji emoji:
                            {
                                int height = 22;
                                InlineUIContainer container = new InlineUIContainer()
                                {
                                    Child = new Image()
                                    {
                                        RenderTransform = new TranslateTransform { Y = 4 },
                                        Height = height,
                                        Source = new BitmapImage()
                                        {
                                            UriSource = new Uri($"https://cdn.discordapp.com/emojis/{emoji.Id}.gif?size={2 * height}&quality=lossless"),
                                            AutoPlay = true
                                        }
                                    }
                                };
                                inlineCollection.Add(container);
                            }
                            break;
                        case Strong strong:
                            {
                                var inline = new Bold();
                                inlineCollection.Add(inline);
                                foreach (AST child in strong.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case Em em:
                            {
                                var inline = new Italic();
                                inlineCollection.Add(inline);
                                foreach (AST child in em.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case U u:
                            {
                                var inline = new Underline();
                                inlineCollection.Add(inline);
                                foreach (AST child in u.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                            }
                            break;
                        case S s:
                            {
                                var inline = new Span() { TextDecorations = TextDecorations.Strikethrough };
                                inlineCollection.Add(inline);
                                foreach (AST child in s.Children.Reverse())
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
                                        IsTextSelectionEnabled = true,
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
                                        IsTextSelectionEnabled = true,
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
                                        IsTextSelectionEnabled = true,
                                        Text = roleMention.RoleID
                                    }
                                };
                            }
                            break;
                        case Mention mention:
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
                                        IsTextSelectionEnabled = true,
                                        Text = mention.UserID
                                    }
                                };
                            }
                            break;
                        case Channel channel:
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
                                        IsTextSelectionEnabled = true,
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
                                        Blocks = { codeParagraph }
                                    }
                                };
                                foreach (AST child in spoiler.Children.Reverse())
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
        private static IList<ASTRoot> AdjustTree(IList<AST> tree)
        {
            // TODO: modify tree inpace
            List<ASTRoot> newTree = new List<ASTRoot>() { new ASTRoot(new List<AST>()) };
            Stack<AST> stack = new Stack<AST>();
            foreach (AST child in tree.Reverse())
            {
                stack.Push(child);
            }
            List<(int, ASTChildren)> depths = new List<(int, ASTChildren)>();
            depths.Add((stack.Count, newTree[0]));
            while (stack.TryPop(out AST node))
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
                                Children = new List<AST>() { node }
                            };

                            newDepths[newDepths.Length - 1] = (depth, current with { Children = new List<AST>() });
                            for (int i = depths.Count - 2; i >= 0; i--)
                            {
                                (int oldDepth, ASTChildren oldNode) = depths[i];
                                newDepths[i] = (oldDepth,
                                    oldNode with { Children = new List<AST>() { newDepths[i + 1].Item2 } });

                                currentCodeBlockNode = oldNode with
                                {
                                    Children = new List<AST>() { currentCodeBlockNode }
                                };
                            }

                            newTree.Add((ASTRoot)currentCodeBlockNode);
                            depths = newDepths.ToList();
                            newTree.Add((ASTRoot)newDepths[0].Item2);
                        }
                        break;
                    case ASTChildren astChildrenNode:
                        {
                            foreach (AST child in astChildrenNode.Children.Reverse())
                            {
                                stack.Push(child);
                            }

                            var tmp = astChildrenNode with { Children = new List<AST>() };
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
