// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Collections.Generic;
using ColorSyntax.Common;

namespace ColorSyntax.Compilation.Languages
{
    public class Typescript : ILanguage
    {
        public string Id
        {
            get { return LanguageId.TypeScript; }
        }

        public string Name
        {
            get { return "Typescript"; }
        }

        public string CssClassName
        {
            get { return "typescript"; }
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
                                   @"/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.Comment },
                                       }),
                               new LanguageRule(
                                   @"(//.*?)\r?$",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.Comment },
                                       }),
                               new LanguageRule(
                                   @"'[^\n]*?'",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.String },
                                       }),
                               new LanguageRule(
                                   @"""[^\n]*?""",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.String },
                                       }),
                               new LanguageRule(
                                   @"\b(abstract|any|bool|boolean|break|byte|case|catch|char|class|const|constructor|continue|debugger|declare|default|delete|do|double|else|enum|export|extends|false|final|finally|float|for|function|goto|if|implements|import|in|instanceof|int|interface|long|module|native|new|number|null|package|private|protected|public|return|short|static|string|super|switch|synchronized|this|throw|throws|transient|true|try|typeof|var|void|volatile|while|with)\b",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.Keyword },
                                       }),
                           };
            }
        }

        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "ts":
                    return true;

                default:
                    return false;
            }
        }
        string[] ILanguage.Aliases => new string[] { "typescript", "ts" };

        public override string ToString()
        {
            return Name;
        }
    }
}