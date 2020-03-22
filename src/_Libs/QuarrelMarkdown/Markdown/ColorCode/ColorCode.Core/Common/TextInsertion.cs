// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// A section of text.
    /// </summary>
    public class TextInsertion
    {
        /// <summary>
        /// Gets or sets index the text begins.
        /// </summary>
        public virtual int Index { get; set; }

        /// <summary>
        /// Gets or sets the text in the insertion.
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the scope the insertion is in.
        /// </summary>
        public virtual Scope Scope { get; set; }
    }
}