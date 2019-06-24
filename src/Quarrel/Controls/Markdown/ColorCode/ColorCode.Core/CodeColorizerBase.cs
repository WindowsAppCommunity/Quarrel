using ColorSyntax.Compilation;
using ColorSyntax.Parsing;
using ColorSyntax.Styling;
using System.Collections.Generic;

namespace ColorSyntax
{
    /// <summary>
    /// Creates a <see cref="CodeColorizerBase"/>, for creating Syntax Highlighted code.
    /// </summary>
    /// <param name="Style">The Custom styles to Apply to the formatted Code.</param>
    /// <param name="languageParser">The language parser that the <see cref="CodeColorizerBase"/> instance will use for its lifetime.</param>
    public abstract class CodeColorizerBase
    {
        public CodeColorizerBase(StyleDictionary Styles, ILanguageParser languageParser)
        {
            this.languageParser = languageParser
                ?? new LanguageParser(new LanguageCompiler(Languages.CompiledLanguages), Languages.LanguageRepository);

            this.Styles = Styles ?? StyleDictionary.DefaultLight;
        }

        /// <summary>
        /// Writes the parsed source code to the ouput using the specified style sheet.
        /// </summary>
        /// <param name="parsedSourceCode">The parsed source code to format and write to the output.</param>
        /// <param name="scopes">The captured scopes for the parsed source code.</param>
        protected abstract void Write(string parsedSourceCode, IList<Scope> scopes);

        /// <summary>
        /// The language parser that the <see cref="CodeColorizerBase"/> instance will use for its lifetime.
        /// </summary>
        protected readonly ILanguageParser languageParser;

        /// <summary>
        /// The styles to Apply to the formatted Code.
        /// </summary>
        public readonly StyleDictionary Styles;
    }
}