// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// CSS language rules.
    /// </summary>
    public class Css : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "css" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Css; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "CSS"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "css"; }
        }

        /// <inheritdoc/>
        public string FirstLinePattern
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule>
                           {
                               new LanguageRule(
                                  @"(?msi)(?:(\s*/\*.*?\*/)|(([a-z0-9#. \[\]=\"":_-]+)\s*(?:,\s*|{))+(?:(\s*/\*.*?\*/)|(?:\s*([a-z0-9 -]+\s*):\s*([a-z0-9#,<>\?%. \(\)\\\/\*\{\}:'\""!_=-]+?)(!important(;)|(;))?))*\s*})",
                                  new Dictionary<int, string>
                                       {
                                           { 3, ScopeName.CssSelector },
                                           { 5, ScopeName.CssPropertyName },
                                           { 6, ScopeName.CssPropertyValue },
                                           { 7, ScopeName.Warning },
                                           { 8, ScopeName.CssPropertyValue },
                                           { 9, ScopeName.CssPropertyValue },
                                           { 4, ScopeName.Comment },
                                           { 1, ScopeName.Comment },
                                       }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
