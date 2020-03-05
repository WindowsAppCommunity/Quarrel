// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core
{
    /// <summary>
    /// Defines how ColorSyntax will parse the source code of a given language.
    /// </summary>
    public interface ILanguage
    {
        /// <summary>
        /// Gets the identifier of the language (e.g., csharp).
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the first line pattern (regex) to use when determining if the language matches a source text.
        /// </summary>
        string FirstLinePattern { get; }

        /// <summary>
        /// Gets the "friendly" name of the language (e.g., C#).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the possible names for the language as after the ```.
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// Gets the collection of language rules in the language.
        /// </summary>
        IList<LanguageRule> Rules { get; }

        /// <summary>
        /// Gets the CSS class name to use for a language.
        /// </summary>
        string CssClassName { get; }

        /// <summary>
        /// Returns true if the specified string is an alias for the language.
        /// </summary>
        /// <param name="lang">The alias to check.</param>
        /// <returns>Whether or not the language goes by that alias.</returns>
        bool HasAlias(string lang);
    }
}