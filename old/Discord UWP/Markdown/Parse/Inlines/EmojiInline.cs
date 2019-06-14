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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Quarrel.MarkdownTextBlock.Parse.Inlines
{


    /// <summary>
    /// Represents a type of hyperlink where the text and the target URL cannot be controlled
    /// independently.
    /// </summary>
    internal class EmojiInline : MarkdownInline, IInlineLeaf
    {

        /// <summary>
        /// Gets or sets the name of the emoji (:emoji:).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the emoji (:emoji:).
        /// </summary>
        public string Id { get; set; }

        public bool IsAnimated { get; set; }

        string IInlineLeaf.Text
        {
            get => GetText();
            set { }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiInline"/> class.
        /// </summary>
        public EmojiInline()
            : base(MarkdownInlineType.Emoji)
        {
        }

        /// <summary>
        /// Attempts to parse a URL within angle brackets e.g. "http://www.reddit.com".
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A parsed URL, or <c>null</c> if this is not a URL. </returns>
        internal static Helpers.Common.InlineParseResult ParseAngleBracketLink(string markdown, int start, int maxEnd)
        {
            int innerStart = start + 1;

            int pos = -1;
            bool animated = false;
            if (maxEnd - innerStart >= 1 && string.Equals(markdown.Substring(innerStart, 1), ":", StringComparison.OrdinalIgnoreCase))
            {
                // Emoji scheme found.
                pos = innerStart + 1;
            }
            else if (maxEnd - innerStart >= 1 && markdown.Length<2 && string.Equals(markdown.Substring(innerStart, 2), "a:", StringComparison.OrdinalIgnoreCase))
            {
                // Animated emoji scheme found.
                pos = innerStart + 1;
                animated = true;
            }
            if (pos == -1)
            {
                return null;
            }

            // Angle bracket links should not have any whitespace.
            int innerEnd = markdown.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '>' }, pos, maxEnd - pos);
            if (innerEnd == -1 || markdown[innerEnd] != '>')
            {
                return null;
            }

            // There should be at least one character after the http://.
            if (innerEnd == pos)
            {
                return null;
            }

            var substr = markdown.Substring(innerStart, innerEnd - innerStart);
            //Emoji markdown must have at least two colons, as it is <:name:id>
            int dotcnt = 0;
            foreach (char c in substr) { if (c == ':') dotcnt++; }
            if (dotcnt < 2)
                return null;

            string name = "";
            if (animated)
                substr = substr.Remove(0, 1);

            name = substr.Substring(0, substr.IndexOf(":", 1) + 1);
            var id = substr.Substring(name.Length, substr.Length - name.Length);


            return new Helpers.Common.InlineParseResult(new EmojiInline { Name = name, Id = id, IsAnimated = animated }, start, innerEnd + 1);
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        private string GetText()
        {
            if (Id != null) return Name;
            return "";
        }

    }
}
