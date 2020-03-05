// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Styling;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.UWP.Common;
using System.Collections.Generic;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Style = Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Styling.Style;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.UWP
{
    /// <summary>
    /// Creates a <see cref="RichTextBlockFormatter"/>, for rendering Syntax Highlighted code to a RichTextBlock.
    /// </summary>
    public class RichTextBlockFormatter : CodeColorizerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBlockFormatter"/> class for rendering Syntax Highlighted code to a RichTextBlock.
        /// </summary>
        /// <param name="theme">The Theme to use, determines whether to use Default Light or Default Dark.</param>
        /// <param name="languageParser">The language parser that the <see cref="RichTextBlockFormatter"/> instance will use for its lifetime.</param>
        public RichTextBlockFormatter(ElementTheme theme, ILanguageParser languageParser = null) : this(theme == ElementTheme.Dark ? StyleDictionary.DefaultDark : StyleDictionary.DefaultLight, languageParser)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBlockFormatter"/> class for rendering Syntax Highlighted code to a RichTextBlock.
        /// </summary>
        /// <param name="style">The Custom styles to Apply to the formatted Code.</param>
        /// <param name="languageParser">The language parser that the <see cref="RichTextBlockFormatter"/> instance will use for its lifetime.</param>
        public RichTextBlockFormatter(StyleDictionary style = null, ILanguageParser languageParser = null) : base(style, languageParser)
        {
        }

        private InlineCollection InlineCollection { get; set; }

        /// <summary>
        /// Adds Syntax Highlighted Source Code to the provided RichTextBlock.
        /// </summary>
        /// <param name="sourceCode">The source code to colorize.</param>
        /// <param name="language">The language to use to colorize the source code.</param>
        /// <param name="richText">The Control to add the Text to.</param>
        public void FormatRichTextBlock(string sourceCode, ILanguage language, RichTextBlock richText)
        {
            var paragraph = new Paragraph();
            richText.Blocks.Add(paragraph);
            FormatInlines(sourceCode, language, paragraph.Inlines);
        }

        /// <summary>
        /// Adds Syntax Highlighted Source Code to the provided InlineCollection.
        /// </summary>
        /// <param name="sourceCode">The source code to colorize.</param>
        /// <param name="language">The language to use to colorize the source code.</param>
        /// <param name="inlineCollection">InlineCollection to add the Text to.</param>
        public void FormatInlines(string sourceCode, ILanguage language, InlineCollection inlineCollection)
        {
            this.InlineCollection = inlineCollection;
            LanguageParser.Parse(sourceCode, language, (parsedSourceCode, captures) => Write(parsedSourceCode, captures));
        }

        /// <summary>
        /// Remakes <paramref name="parsedSourceCode"/> with color in a <see cref="RichTextBlock"/>.
        /// </summary>
        /// <param name="parsedSourceCode">Raw code.</param>
        /// <param name="scopes">Scope in the code.</param>
        protected override void Write(string parsedSourceCode, IList<Scope> scopes)
        {
            var styleInsertions = new List<TextInsertion>();

            foreach (Scope scope in scopes)
            {
                GetStyleInsertionsForCapturedStyle(scope, styleInsertions);
            }

            styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

            int offset = 0;

            Scope previousScope = null;

            foreach (var styleinsertion in styleInsertions)
            {
                var text = parsedSourceCode.Substring(offset, styleinsertion.Index - offset);
                CreateSpan(text, previousScope);
                if (!string.IsNullOrWhiteSpace(styleinsertion.Text))
                {
                    CreateSpan(text, previousScope);
                }

                offset = styleinsertion.Index;
                previousScope = styleinsertion.Scope;
            }

            var remaining = parsedSourceCode.Substring(offset);

            // Ensures that those loose carriages don't run away!
            if (remaining != "\r")
            {
                CreateSpan(remaining, null);
            }
        }

        private void CreateSpan(string text, Scope scope)
        {
            var span = new Span();
            var run = new Run
            {
                Text = text,
            };

            // Styles and writes the text to the span.
            if (scope != null)
            {
                StyleRun(run, scope);
            }

            span.Inlines.Add(run);

            InlineCollection.Add(span);
        }

        private void StyleRun(Run run, Scope scope)
        {
            string foreground = null;
            string background = null;
            bool italic = false;
            bool bold = false;

            if (Styles.Contains(scope.Name))
            {
                Style style = Styles[scope.Name];

                foreground = style.Foreground;
                background = style.Background;
                italic = style.Italic;
                bold = style.Bold;
            }

            if (!string.IsNullOrWhiteSpace(foreground))
            {
                run.Foreground = foreground.GetSolidColorBrush();
            }

            // Background isn't supported, but a workaround could be created.
            if (italic)
            {
                run.FontStyle = FontStyle.Italic;
            }

            if (bold)
            {
                run.FontWeight = FontWeights.Bold;
            }
        }

        private void GetStyleInsertionsForCapturedStyle(Scope scope, ICollection<TextInsertion> styleInsertions)
        {
            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index,
                Scope = scope,
            });

            foreach (Scope childScope in scope.Children)
            {
                GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);
            }

            styleInsertions.Add(new TextInsertion
            {
                Index = scope.Index + scope.Length,
            });
        }
    }
}