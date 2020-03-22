// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Parsing;
using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Styling;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core
{
    /// <summary>
    /// Contains color analysis databy a language.
    /// </summary>
    public abstract class CodeColorizerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeColorizerBase"/> class for creating Syntax Highlighted code.
        /// </summary>
        /// <param name="styles">The Custom styles to Apply to the formatted Code.</param>
        /// <param name="languageParser">The language parser that the <see cref="CodeColorizerBase"/> instance will use for its lifetime.</param>
        public CodeColorizerBase(StyleDictionary styles, ILanguageParser languageParser)
        {
            LanguageParser = languageParser
                ?? new LanguageParser(new LanguageCompiler(Languages.CompiledLanguages), Languages.LanguageRepository);

            Styles = styles ?? StyleDictionary.DefaultLight;
        }

        /// <summary>
        /// Gets the styles to Apply to the formatted Code.
        /// </summary>
        public StyleDictionary Styles { get; }

        /// <summary>
        /// Gets the language parser that the <see cref="CodeColorizerBase"/> instance will use for its lifetime.
        /// </summary>
        public ILanguageParser LanguageParser { get; }

        /// <summary>
        /// Writes the parsed source code to the ouput using the specified style sheet.
        /// </summary>
        /// <param name="parsedSourceCode">The parsed source code to format and write to the output.</param>
        /// <param name="scopes">The captured scopes for the parsed source code.</param>
        protected abstract void Write(string parsedSourceCode, IList<Scope> scopes);
    }
}