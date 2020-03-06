// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Python language rules.
    /// </summary>
    public class Python : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "python", "py", "gyp" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Python; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "Python"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "python"; }
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
                                   @"(?s)^(>>>|\.\.\.)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Brackets },
                                   }),
                               new LanguageRule(
                                   @"(# .*?)\r?$",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Comment },
                                   }),
                               new LanguageRule(
                                   @"((""""""|''')((\n(>>>|\.\.\.))|.)*?(?<!\\)(""""""|'''))",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.Comment },
                                           { 2, ScopeName.Comment },
                                           { 3, ScopeName.Comment },
                                           { 4, ScopeName.Brackets },
                                           { 6, ScopeName.Comment },
                                       }),
                               new LanguageRule(
                                   @"(?m)(^@.*)$",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.PreprocessorKeyword },
                                   }),
                               new LanguageRule(
                                   @"(?s)('[^\n]*?(?<!\\)')",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),
                               new LanguageRule(
                                   @"(?s)(""[^\n]*?(?<!\\)"")",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),
                               new LanguageRule(
                                   @"\b((0b[01]+)[lLjJ]|\d+(.\d+)?)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.Number },
                                   }),
                               new LanguageRule(
                                   @"\b(and|elif|is|global|as|in|if|from|raise|for|except|finally|print|import|pass|return|exec|else|break|not|with|class|assert|yield|try|while|continue|del|or|def|lambda|async|await|nonlocal|None|True|False)\b",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.Keyword },
                                       }),
                               new LanguageRule(
                                   @"\b(Ellipsis|NotImplemented)\b",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.BuiltinFunction },
                                   }),
                               new LanguageRule(
                                   @"(?s)(?<=def )[A-Za-z0-9_]+",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.BuiltinFunction },
                                   }),
                               new LanguageRule(
                                   @"(?s)(?<=class )[A-Za-z0-9_]+",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.Attribute },
                                   }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "python":
                    return true;
                case "py":
                    return true;
                case "gyp":
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }
    }
}
