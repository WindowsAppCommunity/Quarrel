// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace ColorSyntax.Compilation
{
    public interface ILanguageCompiler
    {
        CompiledLanguage Compile(ILanguage language);
    }
}