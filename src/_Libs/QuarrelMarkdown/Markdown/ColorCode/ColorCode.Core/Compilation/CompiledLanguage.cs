// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation
{
    /// <summary>
    /// Language color coding parser.
    /// </summary>
    public class CompiledLanguage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledLanguage"/> class with the appropiate <paramref name="id"/>, regex and keywords.
        /// </summary>
        /// <param name="id">Language ID.</param>
        /// <param name="name">Langauge name.</param>
        /// <param name="regex">Regex for language colors.</param>
        /// <param name="captures">Language keywords.</param>
        public CompiledLanguage(
            string id,
            string name,
            System.Text.RegularExpressions.Regex regex,
            IList<string> captures)
        {
            Guard.ArgNotNullAndNotEmpty(id, "id");
            Guard.ArgNotNullAndNotEmpty(name, "name");
            Guard.ArgNotNull(regex, "regex");
            Guard.ArgNotNullAndNotEmpty(captures, "captures");

            Id = id;
            Name = name;
            Regex = regex;
            Captures = captures;
        }

        /// <summary>
        /// Gets or sets captures in the language.
        /// </summary>
        public IList<string> Captures { get; set; }

        /// <summary>
        /// Gets or sets the language ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the language name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Regex.
        /// </summary>
        public System.Text.RegularExpressions.Regex Regex { get; set; }

        /// <summary>
        /// Gets the language name when called as a string.
        /// </summary>
        /// <returns>Teh language name.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}