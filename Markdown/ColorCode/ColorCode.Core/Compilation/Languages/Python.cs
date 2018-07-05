// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using ColorSyntax.Common;

namespace ColorSyntax.Compilation.Languages
{
    public class Python : ILanguage
    {
        public string Id
        {
            get { return LanguageId.Python; }
        }

        public string Name
        {
            get { return "Python"; }
        }

        public string CssClassName
        {
            get { return "python"; }
        }

        public string FirstLinePattern
        {
            get
            {
                return null;
            }
        }

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
                                           { 1, ScopeName.Comment }
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
                                       { 1, ScopeName.PreprocessorKeyword }
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
                                           {0, ScopeName.Keyword},
                                       }),
                               new LanguageRule(
                                   @"\b(Ellipsis|NotImplemented)\b",
                                   new Dictionary<int, string>
                                   {
                                       {0, ScopeName.BuiltinFunction},
                                   }),
                               new LanguageRule(
                                   @"(?s)(?<=def )[A-Za-z0-9_]+",
                                   new Dictionary<int, string>
                                   {
                                       {0, ScopeName.BuiltinFunction},
                                   }),
                               new LanguageRule(
                                   @"(?s)(?<=class )[A-Za-z0-9_]+",
                                   new Dictionary<int, string>
                                   {
                                       {0, ScopeName.Attribute},
                                   }),
                               
                           };
            }
        }

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
        string[] ILanguage.Aliases => new string[] { "python", "py", "gyp" };
        public override string ToString()
        {
            return Name;
        }
    }
}
