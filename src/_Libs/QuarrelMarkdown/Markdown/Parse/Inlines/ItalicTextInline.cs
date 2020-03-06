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

using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.Parse.Inlines
{
    /// <summary>
    /// Represents a span containing italic text.
    /// </summary>
    internal class ItalicTextInline : MarkdownInline, IInlineContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItalicTextInline"/> class.
        /// </summary>
        public ItalicTextInline()
            : base(MarkdownInlineType.Italic)
        {
        }

        /// <summary>
        /// Gets or sets the contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
            {
                return base.ToString();
            }

            return "*" + string.Join(string.Empty, Inlines) + "*";
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        /// <param name="tripCharHelpers">List of characters that begin a markdown section.</param>
        internal static void AddTripChars(List<Helpers.Common.InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new Helpers.Common.InlineTripCharHelper() { FirstChar = '*', Method = Helpers.Common.InlineParseMethod.Italic });
            tripCharHelpers.Add(new Helpers.Common.InlineTripCharHelper() { FirstChar = '_', Method = Helpers.Common.InlineParseMethod.Italic });
        }

        /// <summary>
        /// Attempts to parse a italic text span.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed italic text span, or <c>null</c> if this is not a italic text span. </returns>
        internal static Helpers.Common.InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            // Check the first char.
            char startChar = markdown[start];
            if (start == maxEnd || (startChar != '*' && startChar != '_'))
            {
                return null;
            }

            // Find the end of the span.  The end character (either '*' or '_') must be the same as
            // the start character.
            var innerStart = start + 1;
            int innerEnd = Helpers.Common.IndexOf(markdown, startChar, start + 1, maxEnd);
            if (innerEnd == -1)
            {
                return null;
            }

            // The span must contain at least one character.
            if (innerStart == innerEnd)
            {
                return null;
            }

            // The first character inside the span must NOT be a space.
            if (Helpers.Common.IsWhiteSpace(markdown[innerStart]))
            {
                return null;
            }

            // The last character inside the span must NOT be a space.
            if (Helpers.Common.IsWhiteSpace(markdown[innerEnd - 1]))
            {
                return null;
            }

            // We found something!
            var result = new ItalicTextInline();
            result.Inlines = Helpers.Common.ParseInlineChildren(markdown, innerStart, innerEnd);
            return new Helpers.Common.InlineParseResult(result, start, innerEnd + 1);
        }
    }
}
