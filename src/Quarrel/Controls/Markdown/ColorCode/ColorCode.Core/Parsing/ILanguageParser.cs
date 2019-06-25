// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing
{
    public interface ILanguageParser
    {
        void Parse(string sourceCode,
                   ILanguage language,
                   Action<string, IList<Scope>> parseHandler);
    }
}