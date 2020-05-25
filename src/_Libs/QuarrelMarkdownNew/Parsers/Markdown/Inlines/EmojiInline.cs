// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Parsers.Core;
using Microsoft.Toolkit.Parsers.Markdown.Helpers;

namespace Microsoft.Toolkit.Parsers.Markdown.Inlines
{
    /// <summary>
    /// Represents a span containing emoji symbol.
    /// </summary>
    public partial class EmojiInline : MarkdownInline, IInlineLeaf
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmojiInline"/> class.
        /// </summary>
        public EmojiInline()
            : base(MarkdownInlineType.Emoji)
        {
        }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = ':', Method = InlineParseMethod.Emoji });
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '<', Method = InlineParseMethod.Emoji });
        }

        internal static InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            if (start >= maxEnd - 1)
            {
                return null;
            }

            // Check the start sequence.
            string startSequence = markdown.Substring(start, 1);
            if (startSequence == ":")
            {

                // Find the end of the span.
                var innerStart = start + 1;
                int innerEnd = Common.IndexOf(markdown, startSequence, innerStart, maxEnd);
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
                if (ParseHelpers.IsMarkdownWhiteSpace(markdown[innerStart]))
                {
                    return null;
                }

                // The last character inside the span must NOT be a space.
                if (ParseHelpers.IsMarkdownWhiteSpace(markdown[innerEnd - 1]))
                {
                    return null;
                }

                var emojiName = markdown.Substring(innerStart, innerEnd - innerStart);

                if (_emojiCodesDictionary.TryGetValue(emojiName, out var emojiCode))
                {
                    var result = new EmojiInline
                        { Name = char.ConvertFromUtf32(emojiCode), Type = MarkdownInlineType.Emoji };
                    return new InlineParseResult(result, start, innerEnd + 1);
                }

            }
            else if (startSequence == "<" && maxEnd - start >= 3)
            {
                int innerStart = start + 1;

                int pos = -1;
                bool animated = false;
                if (string.Equals(markdown.Substring(innerStart, 1), ":", StringComparison.OrdinalIgnoreCase))
                {
                    // Emoji scheme found.
                    pos = innerStart + 1;
                }
                else if (string.Equals(markdown.Substring(innerStart, 2), "a:", StringComparison.OrdinalIgnoreCase))
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


                var result = new EmojiInline { Name = name, Id = id, IsAnimated = animated, Type = MarkdownInlineType.Emoji };
                return new InlineParseResult(result, start, innerEnd + 1);
            }
            return null;
        }

        /// <inheritdoc/>
        public string Text
        {
            get => Name; 
            set {}
        }


        /// <summary>
        /// Gets or sets the name of the emoji (:emoji:).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id of the emoji (:emoji:).
        /// </summary>
        public string Id { get; set; }

        public bool IsAnimated { get; set; }
    }
}