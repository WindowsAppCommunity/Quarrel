// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;

namespace ColorSyntax.Common
{
    public interface ILanguageRepository
    {
        IEnumerable<ILanguage> All { get; }
        ILanguage FindById(string languageId);
        void Load(ILanguage language);
    }
}