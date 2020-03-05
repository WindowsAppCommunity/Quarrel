// Copyright (c) Quarrel. All rights reserved.

using System;
using Windows.UI.Xaml.Documents;

namespace Quarrel.Controls.Markdown
{
    /// <summary>
    /// Arguments for the CodeBlockResolving event.
    /// </summary>
    public class CodeBlockResolvingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeBlockResolvingEventArgs"/> class.
        /// </summary>
        /// <param name="inlineCollection">The inlines in the codeblock.</param>
        /// <param name="text">The code text.</param>
        /// <param name="codeLanguage">The code language.</param>
        internal CodeBlockResolvingEventArgs(InlineCollection inlineCollection, string text, string codeLanguage)
        {
            InlineCollection = inlineCollection;
            Text = text;
            CodeLanguage = codeLanguage;
        }

        /// <summary>
        /// Gets the language of the Code Block, as specified by ```{Language} on the first line of the block.
        /// </summary>
        public string CodeLanguage { get; }

        /// <summary>
        /// Gets the raw code block text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets Collection to add formatted Text to.
        /// </summary>
        public InlineCollection InlineCollection { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this event was handled successfully.
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}
