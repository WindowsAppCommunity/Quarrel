// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Quarrel.Controls.Markdown.Parse;
using Quarrel.Controls.Markdown.Parse.Blocks;
using Quarrel.Controls.Markdown.Parse.Inlines;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using QuarrelSmartColor.Extensions.Windows.UI;
using Emoji = NeoSmart.Unicode.Emoji;
using Quarrel.ViewModels.Models.Bindables.Channels;

namespace Quarrel.Controls.Markdown.Display
{
    /// <summary>
    /// Handles rendering markdown in a <see cref="RichTextBlock"/>.
    /// </summary>
    internal class XamlRenderer
    {
        private static bool? _textDecorationsSupported = null;

        // This is super hacky, but it's a way of keeping intertia and passing scroll data to the parent;
        private static MethodInfo pointerWheelChanged = typeof(ScrollViewer).GetMethod("OnPointerWheelChanged", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// The markdown document that will be rendered.
        /// </summary>
        private readonly MarkdownDocument _document;

        /// <summary>
        /// An interface that is used to register hyperlinks.
        /// </summary>
        private readonly ILinkRegister _linkRegister;

        private string _messageid;
        private bool _halfopacity;
        private IEnumerable<User> _users;
        private IChannelsService _channelsService = SimpleIoc.Default.GetInstance<IChannelsService>();
        private IGuildsService _guildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        private ICurrentUserService _currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlRenderer"/> class.
        /// </summary>
        /// <param name="document">The <see cref="MarkdownDocument"/>.</param>
        /// <param name="linkRegister">The <see cref="ILinkRegister"/>.</param>
        /// <param name="users">The list of users mentioned in the markdown.</param>
        /// <param name="messageId">The id of the message.</param>
        /// <param name="codeBlockResolver">The <see cref="ICodeBlockResolver"/>.</param>
        /// <param name="border">The root markdown.</param>
        /// <param name="halfopacity">Whether or not the markdown is in half opacity mode.</param>
        public XamlRenderer(MarkdownDocument document, ILinkRegister linkRegister, IEnumerable<User> users, string messageId, ICodeBlockResolver codeBlockResolver, ref Border border, bool halfopacity)
        {
            _document = document;
            _halfopacity = halfopacity;

            _linkRegister = linkRegister;
            _messageid = messageId;
            CodeBlockResolver = codeBlockResolver;
            Root = border;
            _users = users;
        }

        /// <summary>
        /// Gets or sets the stretch used for images.
        /// </summary>
        public Stretch ImageStretch { get; set; }

        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        public Brush Background { get; set; }

        /// <summary>
        /// Gets or sets a brush that describes the border fill of a control.
        /// </summary>
        public Brush BorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the border thickness of a control.
        /// </summary>
        public Thickness BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the uniform spacing between characters, in units of 1/1000 of an em.
        /// </summary>
        public int CharacterSpacing { get; set; }

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Gets or sets the degree to which a font is condensed or expanded on the screen.
        /// </summary>
        public FontStretch FontStretch { get; set; }

        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        public Brush Foreground { get; set; }

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        public Brush BoldForeground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether text selection is enabled.
        /// </summary>
        public bool IsTextSelectionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the RightTapped event is handled.
        /// </summary>
        public bool IsRightTapHandled { get; set; }

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// Gets or sets the brush used to fill the background of a code block.
        /// </summary>
        public Brush CodeBackground { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the border fill of a code block.
        /// </summary>
        public Brush CodeBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the border around code blocks.
        /// </summary>
        public Thickness CodeBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the text inside a code block.  If this is
        /// <c>null</c>, then <see cref="Foreground"/> is used.
        /// </summary>
        public Brush CodeForeground { get; set; }

        /// <summary>
        /// Gets or sets the font used to display code.  If this is <c>null</c>, then
        /// <see cref="FontFamily"/> is used.
        /// </summary>
        public FontFamily CodeFontFamily { get; set; }

        /// <summary>
        /// Gets or sets the space outside of code blocks.
        /// </summary>
        public Thickness CodeMargin { get; set; }

        /// <summary>
        /// Gets or sets the space between the code border and the text.
        /// </summary>
        public Thickness CodePadding { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 1 headers.
        /// </summary>
        public FontWeight Header1FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 1 headers.
        /// </summary>
        public double Header1FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 1 headers.
        /// </summary>
        public Thickness Header1Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 1 headers.
        /// </summary>
        public Brush Header1Foreground { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 2 headers.
        /// </summary>
        public FontWeight Header2FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 2 headers.
        /// </summary>
        public double Header2FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 2 headers.
        /// </summary>
        public Thickness Header2Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 2 headers.
        /// </summary>
        public Brush Header2Foreground { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 3 headers.
        /// </summary>
        public FontWeight Header3FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 3 headers.
        /// </summary>
        public double Header3FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 3 headers.
        /// </summary>
        public Thickness Header3Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 3 headers.
        /// </summary>
        public Brush Header3Foreground { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 4 headers.
        /// </summary>
        public FontWeight Header4FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 4 headers.
        /// </summary>
        public double Header4FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 4 headers.
        /// </summary>
        public Thickness Header4Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 4 headers.
        /// </summary>
        public Brush Header4Foreground { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 5 headers.
        /// </summary>
        public FontWeight Header5FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 5 headers.
        /// </summary>
        public double Header5FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 5 headers.
        /// </summary>
        public Thickness Header5Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 5 headers.
        /// </summary>
        public Brush Header5Foreground { get; set; }

        /// <summary>
        /// Gets or sets the font weight to use for level 6 headers.
        /// </summary>
        public FontWeight Header6FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the font size for level 6 headers.
        /// </summary>
        public double Header6FontSize { get; set; }

        /// <summary>
        /// Gets or sets the margin for level 6 headers.
        /// </summary>
        public Thickness Header6Margin { get; set; }

        /// <summary>
        /// Gets or sets the foreground brush for level 6 headers.
        /// </summary>
        public Brush Header6Foreground { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render a horizontal rule.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush HorizontalRuleBrush { get; set; }

        /// <summary>
        /// Gets or sets the margin used for horizontal rules.
        /// </summary>
        public Thickness HorizontalRuleMargin { get; set; }

        /// <summary>
        /// Gets or sets the vertical thickness of the horizontal rule.
        /// </summary>
        public double HorizontalRuleThickness { get; set; }

        /// <summary>
        /// Gets or sets the margin used by lists.
        /// </summary>
        public Thickness ListMargin { get; set; }

        /// <summary>
        /// Gets or sets the width of the space used by list item bullets/numbers.
        /// </summary>
        public double ListGutterWidth { get; set; }

        /// <summary>
        /// Gets or sets the space between the list item bullets/numbers and the list item content.
        /// </summary>
        public double ListBulletSpacing { get; set; }

        /// <summary>
        /// Gets or sets the margin used for paragraphs.
        /// </summary>
        public Thickness ParagraphMargin { get; set; }

        /// <summary>
        /// Gets or sets the brush used to fill the background of a quote block.
        /// </summary>
        public Brush QuoteBackground { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render a quote border.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush QuoteBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of quote borders.
        /// </summary>
        public Thickness QuoteBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render the text inside a quote block.  If this is
        /// <c>null</c>, then <see cref="Foreground"/> is used.
        /// </summary>
        public Brush QuoteForeground { get; set; }

        /// <summary>
        /// Gets or sets the space outside of quote borders.
        /// </summary>
        public Thickness QuoteMargin { get; set; }

        /// <summary>
        /// Gets or sets the space between the quote border and the text.
        /// </summary>
        public Thickness QuotePadding { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render table borders.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush TableBorderBrush { get; set; }

        /// <summary>
        /// Gets or sets the thickness of any table borders.
        /// </summary>
        public double TableBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the padding inside each cell.
        /// </summary>
        public Thickness TableCellPadding { get; set; }

        /// <summary>
        /// Gets or sets the margin used by tables.
        /// </summary>
        public Thickness TableMargin { get; set; }

        /// <summary>
        /// Gets or sets the word wrapping behavior.
        /// </summary>
        public TextWrapping TextWrapping { get; set; }

        /// <summary>
        /// Gets or sets the line height.
        /// </summary>
        public double LineHeight { get; set; }

        /// <summary>
        /// Gets or sets the brush used to render links.  If this is <c>null</c>, then
        /// <see cref="Foreground"/> is used.
        /// </summary>
        public Brush LinkForeground { get; set; }

        /// <summary>
        /// Gets or sets the root for the renderer.
        /// </summary>
        public Border Root { get; set; }

        /// <summary>
        /// Gets a resolver for the style to use in rendering a code block.
        /// </summary>
        protected ICodeBlockResolver CodeBlockResolver { get; }

        private static bool TextDecorationsSupported => (bool)(_textDecorationsSupported ?? (_textDecorationsSupported = ApiInformation.IsTypePresent("Windows.UI.Text.TextDecorations")));

        /// <summary>
        /// Called externally to render markdown to a text block.
        /// </summary>
        /// <returns> A XAML UI element. </returns>
        public UIElement Render()
        {
            var stackPanel = new StackPanel();
            RenderBlocks(_document.Blocks, stackPanel.Children, new RenderContext { Foreground = Foreground });

            // Set background and border properties.
            stackPanel.Background = Background;
            stackPanel.BorderBrush = BorderBrush;
            stackPanel.BorderThickness = BorderThickness;
            stackPanel.Padding = Padding;

            return stackPanel;
        }

        /// <summary>
        /// Renders a list of block elements.
        /// </summary>
        private void RenderBlocks(IEnumerable<MarkdownBlock> blockElements, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            foreach (MarkdownBlock element in blockElements)
            {
                RenderBlock(element, blockUIElementCollection, context);
            }

            // Remove the top margin from the first block element, the bottom margin from the last block element,
            // and collapse adjacent margins.
            FrameworkElement previousFrameworkElement = null;
            for (int i = 0; i < blockUIElementCollection.Count; i++)
            {
                var frameworkElement = blockUIElementCollection[i] as FrameworkElement;
                if (frameworkElement != null)
                {
                    if (i == 0)
                    {
                        // Remove the top margin.
                        frameworkElement.Margin = new Thickness(
                            frameworkElement.Margin.Left,
                            0,
                            frameworkElement.Margin.Right,
                            frameworkElement.Margin.Bottom);
                    }
                    else if (previousFrameworkElement != null)
                    {
                        // Remove the bottom margin.
                        frameworkElement.Margin = new Thickness(
                            frameworkElement.Margin.Left,
                            Math.Max(frameworkElement.Margin.Top, previousFrameworkElement.Margin.Bottom),
                            frameworkElement.Margin.Right,
                            frameworkElement.Margin.Bottom);
                        previousFrameworkElement.Margin = new Thickness(
                            previousFrameworkElement.Margin.Left,
                            previousFrameworkElement.Margin.Top,
                            previousFrameworkElement.Margin.Right,
                            0);
                    }
                }

                previousFrameworkElement = frameworkElement;
            }
        }

        /// <summary>
        /// Called to render a block element.
        /// </summary>
        private void RenderBlock(MarkdownBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            switch (element.Type)
            {
                case MarkdownBlockType.Paragraph:
                    RenderParagraph((ParagraphBlock)element, blockUIElementCollection, context);
                    break;
                /*   case MarkdownBlockType.Quote:
                       RenderQuote((QuoteBlock)element, blockUIElementCollection, context);*/
                case MarkdownBlockType.Code:
                    RenderCode((CodeBlock)element, blockUIElementCollection, context);
                    break;
                    /*   case MarkdownBlockType.Header:
                           RenderHeader((HeaderBlock)element, blockUIElementCollection, context);
                           break;
                       case MarkdownBlockType.List:
                           RenderListElement((ListBlock)element, blockUIElementCollection, context);
                           break;
                       case MarkdownBlockType.HorizontalRule:
                           RenderHorizontalRule(blockUIElementCollection, context);
                           break;
                       case MarkdownBlockType.Table:
                           RenderTable((TableBlock)element, blockUIElementCollection, context);
                           break;*/
            }
        }

        /// <summary>
        /// Renders a paragraph element.
        /// </summary>
        private void RenderParagraph(ParagraphBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var paragraph = new Paragraph
            {
                Margin = ParagraphMargin,
            };
            context.TrimLeadingWhitespace = true;
            if (RenderInlineChildren(paragraph.Inlines, element.Inlines, paragraph, context))
            {
                paragraph.Margin = new Thickness(ParagraphMargin.Left, 0, ParagraphMargin.Right, ParagraphMargin.Bottom);
            }

            var textBlock = CreateOrReuseRichTextBlock(blockUIElementCollection, context);
            textBlock.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Renders a code element.
        /// </summary>
        private void RenderCode(CodeBlock element, UIElementCollection blockUIElementCollection, RenderContext context)
        {
            var brush = context.Foreground;
            if (CodeForeground != null)
            {
                brush = CodeForeground;
            }

            var textBlock = new RichTextBlock
            {
                FontFamily = CodeFontFamily ?? FontFamily,
                Foreground = brush,
                LineHeight = FontSize * 1,
            };

            textBlock.PointerWheelChanged += Preventative_PointerWheelChanged;

            var paragraph = new Paragraph();
            textBlock.Blocks.Add(paragraph);

            // Allows external Syntax Highlighting
            var hasCustomSyntax = CodeBlockResolver.ParseSyntax(paragraph.Inlines, element.Text, element.CodeLanguage);
            if (!hasCustomSyntax)
            {
                paragraph.Inlines.Add(new Run { Text = element.Text });
            }

            // Ensures that Code has Horizontal Scroll and doesn't wrap.
            var viewer = new ScrollViewer
            {
                Background = CodeBackground,
                BorderBrush = CodeBorderBrush,
                BorderThickness = CodeBorderThickness,
                Padding = CodePadding,
                Margin = CodeMargin,
                Content = textBlock,
            };

            viewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            viewer.HorizontalScrollMode = ScrollMode.Auto;
            viewer.VerticalScrollMode = ScrollMode.Disabled;
            viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            // Add it to the blocks
            blockUIElementCollection.Add(viewer);
        }

        private void Preventative_PointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint((UIElement)sender);
            if (pointerPoint.Properties.IsHorizontalMouseWheel)
            {
                e.Handled = false;
                return;
            }

            var rootViewer = VisualTree.FindAscendant<ScrollViewer>(Root);
            if (rootViewer != null)
            {
                pointerWheelChanged?.Invoke(rootViewer, new object[] { e });
                e.Handled = true;
            }
        }

        /// <summary>
        /// Renders all of the children for the given element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="inlineElements"> The parsed inline elements to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private bool RenderInlineChildren(InlineCollection inlineCollection, IList<MarkdownInline> inlineElements, TextElement parent, RenderContext context)
        {
            bool largeEmojis = true;
            bool mixedlarge = false;
            int maxemojicount = 28;
            foreach (MarkdownInline inline in inlineElements)
            {
                if (inline.Type == MarkdownInlineType.TextRun)
                {
                    string inlinecontent = ((TextRunInline)inline).Text.Replace(" ", string.Empty);
                    if (!string.IsNullOrEmpty(inlinecontent) &&
                        !Emoji.IsEmoji(((TextRunInline)inline).Text.Replace(" ", string.Empty), maxemojicount))
                    {
                        largeEmojis = false;
                        maxemojicount -= inlinecontent.Length;
                        break;
                    }
                    else
                    {
                        mixedlarge = true;
                    }
                }
                else if (inline.Type == MarkdownInlineType.Emoji)
                {
                    maxemojicount--;
                }
                else
                {
                    largeEmojis = false;
                    break;
                }
            }

            foreach (MarkdownInline element in inlineElements)
            {
                RenderInline(inlineCollection, element, parent, context, largeEmojis, mixedlarge);
            }

            return largeEmojis;
        }

        /// <summary>
        /// Called to render an inline element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to.</param>
        /// <param name="element"> The parsed inline element to render.</param>
        /// <param name="parent"> The container element.</param>
        /// <param name="context"> Persistent state.</param>
        private void RenderInline(InlineCollection inlineCollection, MarkdownInline element, TextElement parent, RenderContext context, bool largeemojis, bool mixedlarge = false)
        {
            switch (element.Type)
            {
                case MarkdownInlineType.TextRun:
                    RenderTextRun(inlineCollection, (TextRunInline)element, context, largeemojis);
                    break;
                case MarkdownInlineType.Italic:
                    RenderItalicRun(inlineCollection, (ItalicTextInline)element, context);
                    break;
                case MarkdownInlineType.Bold:
                    RenderBoldRun(inlineCollection, (BoldTextInline)element, context);
                    break;
                case MarkdownInlineType.MarkdownLink:
                    RenderMarkdownLink(inlineCollection, (MarkdownLinkInline)element, parent, context);
                    break;
                case MarkdownInlineType.Emoji:
                    RenderEmoji(inlineCollection, (EmojiInline)element, parent, context, largeemojis, mixedlarge);
                    break;
                case MarkdownInlineType.RawHyperlink:
                    RenderHyperlink(inlineCollection, (HyperlinkInline)element, context);
                    break;
                case MarkdownInlineType.Strikethrough:
                    RenderStrikethroughRun(inlineCollection, (StrikethroughTextInline)element, parent, context);
                    break;
                case MarkdownInlineType.Code:
                    RenderCodeRun(inlineCollection, (CodeInline)element, context);
                    break;
                case MarkdownInlineType.Image:
                    RenderImage(inlineCollection, (ImageInline)element, context);
                    break;
                case MarkdownInlineType.Underline:
                    RenderUnderlineRun(inlineCollection, (UnderlineTextInline)element, context);
                    break;
            }
        }

        /// <summary>
        /// Renders a custom emoji.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderEmoji(InlineCollection inlineCollection, EmojiInline element, TextElement parent, RenderContext context, bool large, bool mixedlarge = false)
        {
            // If the emoji is the only content of the message
            InlineUIContainer imageRun = new InlineUIContainer();
            string extension = ".png";
            if (element.IsAnimated)
            {
                extension = ".gif";
            }

            Thickness imagemargin = new Thickness(0, 0, 0, 0);
            if (mixedlarge)
            {
                imagemargin = new Thickness(0, 0, 0, -8);
            }
            else if (large)
            {
                imagemargin = new Thickness(0, 0, 0, 0);
            }

            if (large)
            {
                imageRun.Child = new Image()
                {
                    Margin = imagemargin,
                    Width = 32,
                    Height = 32,
                    Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + element.Id + extension)),
                };
            }
            else
            {
                imageRun.Child = new Image()
                {
                    Margin = imagemargin,
                    Width = 20,
                    Height = 20,
                    Source = new BitmapImage(new Uri("https://cdn.discordapp.com/emojis/" + element.Id + extension)),
                };
            }

            // Add the tooltip of the emoji's name
            ToolTipService.SetToolTip(imageRun, element.Name);

            // Add it
            inlineCollection.Add(imageRun);
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderTextRun(InlineCollection inlineCollection, TextRunInline element, RenderContext context, bool large)
        {
            // Create the text run
            Run textRun = new Run
            {
                Text = element.Text,
            };

            if (large)
            {
                textRun.FontSize = 28;
            }

            // Add it
            inlineCollection.Add(textRun);
        }

        /// <summary>
        /// Renders a bold run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderBoldRun(InlineCollection inlineCollection, BoldTextInline element, RenderContext context)
        {
            // Create the text run
            FontWeight weight = FontWeights.Bold;
            if (_halfopacity)
            {
                weight = FontWeights.SemiBold;
            }

            Span boldSpan = new Span
            {
                FontWeight = weight,
                Foreground = BoldForeground,
            };

            // Render the children into the bold inline.
            var newcontext = context.Clone();
            newcontext.Foreground = BoldForeground;
            RenderInlineChildren(boldSpan.Inlines, element.Inlines, boldSpan, newcontext);

            // Add it to the current inlines
            inlineCollection.Add(boldSpan);
        }

        /// <summary>
        /// Renders a link element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="parent"> The container element. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderMarkdownLink(InlineCollection inlineCollection, MarkdownLinkInline element, TextElement parent, RenderContext context)
        {
            // Avoid crash when link text is empty.
            if (element.Inlines.Count == 0)
            {
                return;
            }

            // Attempt to resolve references.
            element.ResolveReference(_document);
            if (element.Url == null || _document.EnableHiddenLinks == false)
            {
                // The element couldn't be resolved, just render it as text.
                RenderInlineChildren(inlineCollection, element.Inlines, parent, context);
                return;
            }

            // Regular ol' hyperlink.
            var link = new Hyperlink();

            // Register the link
            _linkRegister.RegisterNewHyperLink(link, element.Url);

            // Render the children into the link inline.
            var childContext = context.Clone();
            childContext.WithinHyperlink = true;

            if (LinkForeground != null)
            {
                link.Foreground = LinkForeground;
            }

            RenderInlineChildren(link.Inlines, element.Inlines, link, childContext);
            context.TrimLeadingWhitespace = childContext.TrimLeadingWhitespace;

            // Add it to the current inlines
            inlineCollection.Add(link);
        }

        /// <summary>
        /// Renders an image element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderImage(InlineCollection inlineCollection, ImageInline element, RenderContext context)
        {
            var image = new Windows.UI.Xaml.Controls.Image();
            var imageContainer = new InlineUIContainer() { Child = image };

            // if url is not absolute we have to return as local images are not supported
            if (!element.Url.StartsWith("http") && !element.Url.StartsWith("ms-app"))
            {
                RenderTextRun(inlineCollection, new TextRunInline { Text = element.Text, Type = MarkdownInlineType.TextRun }, context, false);
                return;
            }

            image.Source = new BitmapImage(new Uri(element.Url));
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;
            image.Stretch = ImageStretch;

            ToolTipService.SetToolTip(image, element.Tooltip);

            // Try to add it to the current inlines
            // Could fail because some containers like Hyperlink cannot have inlined images
            try
            {
                inlineCollection.Add(imageContainer);
            }
            catch
            {
                // Ignore error
            }
        }

        /// <summary>
        /// Renders a raw link element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderHyperlink(InlineCollection inlineCollection, HyperlinkInline element, RenderContext context)
        {
            var link = new HyperlinkButton();

            if (element.LinkType == HyperlinkType.DiscordUserMention || element.LinkType == HyperlinkType.DiscordChannelMention || element.LinkType == HyperlinkType.DiscordRoleMention || element.LinkType == HyperlinkType.DiscordNickMention || element.LinkType == HyperlinkType.QuarrelColor)
            {
                var content = element.Text;
                bool enabled = true;
                SolidColorBrush foreground = (SolidColorBrush)SimpleIoc.Default.GetInstance<IResourceService>().GetResource("Blurple");

                try
                {
                    if (element.LinkType == HyperlinkType.DiscordUserMention || element.LinkType == HyperlinkType.DiscordNickMention)
                    {
                        string mentionid = element.Text.Remove(0, element.LinkType == HyperlinkType.DiscordNickMention ? 2 : 1);
                        if (_users != null)
                        {
                            foreach (var user in _users)
                            {
                                if (user.Id == mentionid)
                                {
                                    link.Tag = user;

                                    if (_halfopacity)
                                    {
                                        content = user.Username;
                                    }
                                    else
                                    {
                                        content = "@" + user.Username;
                                    }

                                    if (_guildsService.CurrentGuild.Model.Name != "DM")
                                    {
                                        var member = _guildsService.GetGuildMember(mentionid, _guildsService.CurrentGuild.Model.Id);
                                        if (!string.IsNullOrWhiteSpace(member?.DisplayName))
                                        {
                                            if (_halfopacity)
                                            {
                                                content = member.DisplayName;
                                            }
                                            else
                                            {
                                                content = "@" + member.DisplayName;
                                            }

                                            foreground = ColorExtensions.IntToBrush(member.TopRole.Color);
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    else if (element.LinkType == HyperlinkType.DiscordChannelMention)
                    {
                        var key = element.Text.Remove(0, 1);
                        BindableChannel value = _channelsService.GetChannel(key);
                        content = "#" + (value?.Model?.Name ?? "deleted-channel");
                        enabled = value != null;

                        link.Tag = value;
                    }
                    else if (element.LinkType == HyperlinkType.DiscordRoleMention)
                    {
                        var role = _guildsService.CurrentGuild.Model.Roles.FirstOrDefault(x => x.Id == element.Text.Remove(0, 2));
                        if (role != null)
                        {
                            if (_halfopacity)
                            {
                                content = role.Name;
                            }
                            else
                            {
                                content = "@" + role.Name;
                            }

                            foreground = ColorExtensions.IntToBrush(role.Color);
                        }
                        else
                        {
                            enabled = false;
                            content = "@deleted-role";
                        }
                    }
                    else if (element.LinkType == HyperlinkType.QuarrelColor)
                    {
                        string intcolor = element.Text.Replace("@$QUARREL-color", string.Empty);
                        try
                        {
                            var color = ColorExtensions.IntToBrush(int.Parse(intcolor));
                            inlineCollection.Add(new InlineUIContainer
                            {
                                Child = new Ellipse()
                                {
                                    Height = FontSize,
                                    Width = FontSize,
                                    Fill = color,
                                    Margin = new Thickness(0, 0, 2, -2),
                                },
                            });
                            inlineCollection.Add(new Run
                            {
                                FontWeight = FontWeights.SemiBold,
                                Foreground = BoldForeground,
                                Text = color.Color.ToString(),
                            });
                            return;
                        }
                        catch
                        {
                        }
                    }
                }
                catch (Exception)
                {
                    content = "<Invalid Mention>";
                }

                link.Content = CollapseWhitespace(context, content);
                link.Foreground = foreground;
                link.FontSize = FontSize;
                if (_halfopacity)
                {
                    link.Style = (Style)Application.Current.Resources["DiscordMentionHyperlinkBold"];
                }
                else
                {
                    link.Style = (Style)Application.Current.Resources["DiscordMentionHyperlink"];
                }

                link.IsEnabled = enabled;
                _linkRegister.RegisterNewHyperLink(link, element.Url);
                InlineUIContainer linkContainer = new InlineUIContainer { Child = link };
                inlineCollection.Add(linkContainer);
            }
            else
            {
                var hLink = new Hyperlink();
                _linkRegister.RegisterNewHyperLink(hLink, element.Url);

                Run linkText = new Run
                {
                    Text = CollapseWhitespace(context, element.Text),
                    Foreground = LinkForeground ?? context.Foreground,
                };

                hLink.Inlines.Add(linkText);

                // Add it to the current inlines
                inlineCollection.Add(hLink);
            }
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderItalicRun(InlineCollection inlineCollection, ItalicTextInline element, RenderContext context)
        {
            // Create the text run
            Span italicSpan = new Span
            {
                FontStyle = FontStyle.Italic,
            };

            // Render the children into the italic inline.
            RenderInlineChildren(italicSpan.Inlines, element.Inlines, italicSpan, context);

            // Add it to the current inlines
            inlineCollection.Add(italicSpan);
        }

        /// <summary>
        /// Renders a strikethrough element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderStrikethroughRun(InlineCollection inlineCollection, StrikethroughTextInline element, TextElement parent, RenderContext context)
        {
            Span strikeSpan = new Span
            {
                TextDecorations = TextDecorations.Strikethrough,
            };

            // Render the children into the bold inline.
            RenderInlineChildren(strikeSpan.Inlines, element.Inlines, strikeSpan, context);
            inlineCollection.Add(strikeSpan);
        }

        private void RenderUnderlineRun(InlineCollection inlineCollection, UnderlineTextInline element, RenderContext context)
        {
            Span underlineSpan = new Span
            {
                TextDecorations = TextDecorations.Underline,
            };

            // Render the children into the bold inline.
            RenderInlineChildren(underlineSpan.Inlines, element.Inlines, underlineSpan, context);
            inlineCollection.Add(underlineSpan);
        }

        /// <summary>
        /// Renders a code element.
        /// </summary>
        /// <param name="inlineCollection"> The list to add to. </param>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        private void RenderCodeRun(InlineCollection inlineCollection, CodeInline element, RenderContext context)
        {
            var color = ((SolidColorBrush)Foreground).Color;
            color.A = 160;
            var run = new Run
            {
                FontFamily = CodeFontFamily ?? FontFamily,
                Text = CollapseWhitespace(context, element.Text),
                Foreground = new SolidColorBrush(color),
            };

            // Add it to the current inlines
            inlineCollection.Add(run);
        }

        /// <summary>
        /// Performs an action against any runs that occur within the given span.
        /// </summary>
        private void AlterChildRuns(Span parentSpan, Action<Span, Run> action)
        {
            foreach (var inlineElement in parentSpan.Inlines)
            {
                var span = inlineElement as Span;
                if (span != null)
                {
                    AlterChildRuns(span, action);
                }
                else if (inlineElement is Run)
                {
                    action(parentSpan, (Run)inlineElement);
                }
            }
        }

        /// <summary>
        /// Removes leading whitespace, but only if this is the first run in the block.
        /// </summary>
        /// <returns>The corrected string.</returns>
        private string CollapseWhitespace(RenderContext context, string text)
        {
            bool dontOutputWhitespace = context.TrimLeadingWhitespace;
            StringBuilder result = null;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == ' ' || c == '\t')
                {
                    if (dontOutputWhitespace)
                    {
                        if (result == null)
                        {
                            result = new StringBuilder(text.Substring(0, i), text.Length);
                        }
                    }
                    else
                    {
                        result?.Append(c);

                        dontOutputWhitespace = true;
                    }
                }
                else
                {
                    result?.Append(c);

                    dontOutputWhitespace = false;
                }
            }

            context.TrimLeadingWhitespace = false;
            return result == null ? text : result.ToString();
        }

        /// <summary>
        /// Creates a new RichTextBlock, if the last element of the provided collection isn't already a RichTextBlock.
        /// </summary>
        /// <returns>The rich text block.</returns>
        private RichTextBlock CreateOrReuseRichTextBlock(UIElementCollection blockUIElementCollection, RenderContext context)
        {
            // Reuse the last RichTextBlock, if possible.
            if (blockUIElementCollection != null && blockUIElementCollection.Count > 0 && blockUIElementCollection[blockUIElementCollection.Count - 1] is RichTextBlock)
            {
                return (RichTextBlock)blockUIElementCollection[blockUIElementCollection.Count - 1];
            }

            var result = new RichTextBlock
            {
                CharacterSpacing = CharacterSpacing,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                LineHeight = LineHeight,
                Foreground = context.Foreground,
                IsTextSelectionEnabled = IsTextSelectionEnabled,
                TextWrapping = TextWrapping,
            };

            if (IsRightTapHandled)
            {
                result.ContextMenuOpening += (sender, e) =>
                {
                    e.Handled = true;
                };
            }

            blockUIElementCollection?.Add(result);

            return result;
        }

        // Helper class for holding persistent state.
        private class RenderContext
        {
            public Brush Foreground { get; set; }

            public bool TrimLeadingWhitespace { get; set; }

            public bool WithinHyperlink { get; set; }

            public RenderContext Clone()
            {
                return (RenderContext)MemberwiseClone();
            }
        }
    }
}
