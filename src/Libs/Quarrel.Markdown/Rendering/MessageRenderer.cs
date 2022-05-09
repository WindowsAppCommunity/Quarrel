// Quarrel © 2022

using Quarrel.Markdown.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Quarrel.Markdown
{
    [TemplatePart(Name = nameof(RichBlockPartName), Type = typeof(RichTextBlock))]
    public sealed partial class MessageRenderer : Control
    {
        private const string RichBlockPartName = "RichBlock";

        private RichTextBlock? _richBlock;
        private Canvas? _backgroundCanvas;
        private Canvas? _overlayCanvas;
        private Grid? _grid;

        private List<Inline> _inlineCodeBlocks = new List<Inline>();
        private List<Inline> _spoilers = new List<Inline>();

        public MessageRenderer()
        {
            this.DefaultStyleKey = typeof(MessageRenderer);
        }

        protected override void OnApplyTemplate()
        {
            //_richBlock = (RichTextBlock)GetTemplateChild(RichBlockPartName);
            _backgroundCanvas = (Canvas)GetTemplateChild("BackgroundCanvas");
            _overlayCanvas = (Canvas)GetTemplateChild("OverlayCanvas");
            _grid = (Grid)GetTemplateChild("Grid");
        }

        private void RichBlock_SizeChanged(object sender, object e)
        {
            UpdateOverlays();
        }

        private TextPointer FindFirstNewLine(TextPointer start, TextPointer end)
        {
            var direction = LogicalDirection.Forward;
            
            while (start.Offset + 1 < end.Offset)
            {
                var lastEnd = end;
                var startRect = start.GetCharacterRect(direction);
                var endRect = end.GetCharacterRect(direction);
                while (Math.Abs(startRect.Top - endRect.Top) > 0.01)
                {
                    lastEnd = end;
                    end = start.GetPositionAtOffset((end.Offset - start.Offset) / 2, direction);
                    endRect = end.GetCharacterRect(direction);
                }

                start = end;
                end = lastEnd;
            }
            return start;
        }

        private PathFigure CreatePathFigure(TextPointer start, TextPointer textEnd)
        {
            var direction = LogicalDirection.Forward;
            int radius = 2;

            var pathFigure = new PathFigure() { StartPoint = new Point(radius, 0) };
            var segments = pathFigure.Segments;
            var firstRect = start.GetCharacterRect(direction);
            var lastPoint = new Point(0, 0);

            bool first = true;
            List<(Rect, Rect)> rects = new List<(Rect, Rect)>();
            while (start.Offset < textEnd.Offset)
            {
                var end = FindFirstNewLine(start, textEnd);
                var startRect = start.GetCharacterRect(direction);
                var endRect = end.GetCharacterRect(direction);
                rects.Add((startRect, endRect));

                bool inwards = (lastPoint.X - (endRect.Right - firstRect.Left)) >= 0;
                segments.Add(new ArcSegment()
                {
                    Point = new Point(lastPoint.X + (inwards ? -radius : radius), lastPoint.Y),
                    Size = new Size(radius, radius),
                    RotationAngle = 45,
                    SweepDirection = first || inwards ? SweepDirection.Clockwise : SweepDirection.Counterclockwise

                });
                segments.Add(new LineSegment()
                {
                    Point = new Point(endRect.Right - firstRect.Left + (inwards ? radius : -radius), lastPoint.Y),
                });
                inwards = (lastPoint.X - (endRect.Right - firstRect.Left)) >= 0;
                segments.Add(new ArcSegment()
                {
                    Point = new Point(endRect.Right - firstRect.Left, lastPoint.Y + radius),
                    Size = new Size(radius, radius),
                    RotationAngle = 45,
                    SweepDirection = inwards ? SweepDirection.Counterclockwise : SweepDirection.Clockwise

                });
                lastPoint = new Point(endRect.Right - firstRect.Left, endRect.Bottom - firstRect.Top);
                segments.Add(new LineSegment()
                {
                    Point = new Point(lastPoint.X, lastPoint.Y - radius)
                });

                start = end.GetPositionAtOffset(1, direction);
                first = false;
            }

            first = true;
            for (int i = rects.Count - 1; i >= 0; i--)
            {
                (Rect startRect, Rect endRect) = rects[i];
                if (Math.Abs(lastPoint.X - (startRect.Left - firstRect.Left)) < 0.1)
                {
                    segments.Add(new LineSegment()
                    {
                        Point = new Point(startRect.Left - firstRect.Left, endRect.Bottom - firstRect.Top)
                    });

                    lastPoint = new Point(startRect.Left - firstRect.Left, endRect.Top - firstRect.Top);
                    segments.Add(new LineSegment() { Point = lastPoint });
                }
                else
                {
                    bool inwards = (lastPoint.X - (startRect.Left - firstRect.Left)) < 0;
                    segments.Add(new ArcSegment()
                    {
                        Point = new Point(lastPoint.X + (first ? -radius : radius), lastPoint.Y),
                        Size = new Size(radius, radius),
                        RotationAngle = 45,
                        SweepDirection = SweepDirection.Clockwise
                    });
                    segments.Add(new LineSegment()
                    {
                        Point = new Point(startRect.Left - firstRect.Left + (inwards ? -radius : radius),
                            endRect.Bottom - firstRect.Top)
                    });

                    segments.Add(new ArcSegment()
                    {
                        Point = new Point(startRect.Right - firstRect.Left, endRect.Bottom - firstRect.Top - radius),
                        Size = new Size(radius, radius),
                        RotationAngle = 45,
                        SweepDirection = inwards ? SweepDirection.Counterclockwise : SweepDirection.Clockwise
                    });
                    lastPoint = new Point(startRect.Left - firstRect.Left, endRect.Top - firstRect.Top);
                    segments.Add(new LineSegment() { Point = new Point(lastPoint.X, lastPoint.Y + radius) });
                }

                first = false;
            }

            return pathFigure;
        }

        private void UpdateOverlays()
        {
            _backgroundCanvas.Children.Clear();
            _overlayCanvas.Children.Clear();
            foreach (var inline in _inlineCodeBlocks)
            {
                var direction = inline.ElementStart.LogicalDirection;
                var start = inline.ElementStart;
                var firstRect = start.GetCharacterRect(direction);
                var path = new InlineCodeElement()
                {
                    PathGeometry = new PathGeometry()
                    {
                        Figures =
                        {
                            CreatePathFigure(start, inline.ElementEnd)
                        }
                    }
                };

                _backgroundCanvas.Children.Add(path);
                Canvas.SetTop(path, firstRect.Top);
                Canvas.SetLeft(path, firstRect.Left);
            }

            foreach (var inline in _spoilers)
            {
                var direction = inline.ElementStart.LogicalDirection;
                var start = inline.ElementStart;
                var firstRect = start.GetCharacterRect(direction);
                var path = new SpoilerElement()
                {
                    PathGeometry = new PathGeometry()
                    {
                        Figures =
                        {
                            CreatePathFigure(start, inline.ElementEnd)
                        }
                    }
                };

                _overlayCanvas.Children.Add(path);
                Canvas.SetTop(path, firstRect.Top);
                Canvas.SetLeft(path, firstRect.Left);
            }
        }

        private void RenderMarkdown(IList<ASTRoot> tree)
        {

            if (_richBlock != null)
            {
                _richBlock.SizeChanged -= RichBlock_SizeChanged;
                _grid.Children.Remove(_richBlock);
            }

            _richBlock = new RichTextBlock() { Foreground = new SolidColorBrush(Color.FromArgb(255, 220, 222, 222))};
            _richBlock.SizeChanged += RichBlock_SizeChanged;
            _inlineCodeBlocks.Clear();
            _spoilers.Clear();
            BlockCollection blocks = _richBlock.Blocks;

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
                                    FontWeight = container.FontWeight,
                                    FontStretch = container.FontStretch,
                                    TextDecorations = container.TextDecorations,
                                    Style = CodeBlockStyle,
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
                                    FontWeight = container.FontWeight,
                                    FontStretch = container.FontStretch,
                                    TextDecorations = container.TextDecorations,
                                    Style = BlockQuoteStyle,
                                };
                                container.Child = element;

                                foreach (IAST child in blockQuote.Children.Reverse())
                                {
                                    stack.Push((child, element.Paragraph.Inlines));
                                }
                            }
                            break;
                        case Url url:
                            inlineCollection.Add(new Hyperlink()
                            {
                                NavigateUri = Uri.TryCreate(url.Content, UriKind.RelativeOrAbsolute, out Uri uri) ? uri : null,
                                Inlines = { new Run() { Text = url.Content } }
                            });
                            break;
                        case IEmojiAST emoji:
                            {
                                var container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new EmojiElement(emoji)
                                {
                                    Style = EmojiStyle,
                                };
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
                                var inline = new Span()
                                {
                                    Inlines =
                                    {
                                        new Run() { Text = " " },
                                        new Run()
                                        {
                                            FontFamily = new FontFamily("Consolas"),
                                            Text = inlineCode.Content,
                                        },
                                        new Run() { Text = " " }
                                    }
                                };
                                inlineCollection.Add(inline);
                                _inlineCodeBlocks.Add(inline);
                            }
                            break;
                        case Timestamp timeStamp:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new TimestampElement(timeStamp)
                                {
                                    Style = TimestampStyle,
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
                                        Text = roleMention.RoleID
                                    }
                                };
                            }
                            break;
                        case UserMention mention:
                            {
                                InlineUIContainer container = new InlineUIContainer();
                                inlineCollection.Add(container);
                                container.Child = new UserMentionElement(mention, Context)
                                {
                                    Style = UserMentionStyle,
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
                                        Text = channel.ChannelID
                                    }
                                };
                            }
                            break;
                        case Spoiler spoiler:
                            {
                                var inline = new Span();

                                foreach (IAST child in spoiler.Children.Reverse())
                                {
                                    stack.Push((child, inline.Inlines));
                                }
                                inlineCollection.Add(inline);
                                _spoilers.Add(inline);
                            }
                            break;
                        case Text textNode:
                            inlineCollection.Add(new Run() { Text = textNode.Content });
                            break;
                    }
                }
            }
            _grid.Children.Insert(1, _richBlock);
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
