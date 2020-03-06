// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Windows.UI.Xaml.Documents;

namespace Quarrel.Controls.Markdown.Display
{
    /// <summary>
    /// An instance of a code block.
    /// </summary>
    public interface ICodeBlockResolver
    {
        /// <summary>
        /// Parses Code Block text into Rich text.
        /// </summary>
        /// <param name="inlineCollection">Block to add formatted Text to.</param>
        /// <param name="text">The raw code block text.</param>
        /// <param name="codeLanguage">The language of the Code Block.</param>
        /// <returns>Parsing was handled Successfully.</returns>
        bool ParseSyntax(InlineCollection inlineCollection, string text, string codeLanguage);
    }
}
