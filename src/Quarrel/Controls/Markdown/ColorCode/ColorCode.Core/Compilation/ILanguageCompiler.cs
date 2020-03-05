// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation
{
    /// <summary>
    /// Makes <see cref="CompiledLanguage"/>s from <see cref="ILanguage"/>s.
    /// </summary>
    public interface ILanguageCompiler
    {
        /// <summary>
        /// Get a <see cref="CompiledLanguage"/> from <paramref name="language"/>.
        /// </summary>
        /// <param name="language">The language to compile</param>
        /// <returns>A <see cref="CompiledLanguage"/>.</returns>
        CompiledLanguage Compile(ILanguage language);
    }
}