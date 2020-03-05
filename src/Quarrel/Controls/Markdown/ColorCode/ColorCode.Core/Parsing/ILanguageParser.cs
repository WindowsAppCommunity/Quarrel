// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing
{
    public interface ILanguageParser
    {
        /// <summary>
        /// Parses a language block.
        /// </summary>
        /// <param name="sourceCode">The raw code.</param>
        /// <param name="language">The language to parse as.</param>
        /// <param name="parseHandler">Scope types by source code.</param>
        void Parse(string sourceCode, ILanguage language, Action<string, IList<Scope>> parseHandler);
    }
}