// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Powershell language rules.
    /// </summary>
    public class PowerShell : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "powershell", "posh", "ps1" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.PowerShell; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "PowerShell"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "powershell"; }
        }

        /// <inheritdoc/>
        public string FirstLinePattern
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public IList<LanguageRule> Rules
        {
            get
            {
                return new List<LanguageRule>
                           {
                               new LanguageRule(
                                   @"(?s)(<\#.*?\#>)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Comment },
                                   }),
                               new LanguageRule(
                                   @"(\#.*?)\r?$",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Comment },
                                   }),
                               new LanguageRule(
                                   @"'[^\n]*?(?<!\\)'",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),
                               new LanguageRule(
                                   @"(?s)@"".*?""@",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.StringCSharpVerbatim },
                                   }),
                               new LanguageRule(
                                   @"(?s)(""[^\n]*?(?<!`)"")",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),
                               new LanguageRule(
                                   @"\$(?:[\d\w\-]+(?::[\d\w\-]+)?|\$|\?|\^)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellVariable },
                                   }),
                               new LanguageRule(
                                   @"\${[^}]+}",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellVariable },
                                   }),
                               new LanguageRule(
                                   @"\b(begin|break|catch|continue|data|do|dynamicparam|elseif|else|end|exit|filter|finally|foreach|for|from|function|if|in|param|process|return|switch|throw|trap|try|until|while)\b",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Keyword },
                                   }),
                               new LanguageRule(
                                   @"-(?:c|i)?(?:eq|ne|gt|ge|lt|le|notlike|like|notmatch|match|notcontains|contains|replace)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellOperator },
                                   }),
                               new LanguageRule(
                                   @"-(?:band|and|as|join|not|bxor|xor|bor|or|isnot|is|split)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellOperator },
                                   }),
                               new LanguageRule(
                                   @"(?:\+=|-=|\*=|/=|%=|=|\+\+|--|\+|-|\*|/|%)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellOperator },
                                   }),
                               new LanguageRule(
                                   @"(?:\>\>|2\>&1|\>|2\>\>|2\>)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.PowerShellOperator },
                                   }),
                               new LanguageRule(
                                   @"(?s)\[(CmdletBinding)[^\]]+\]",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.PowerShellAttribute },
                                   }),
                               new LanguageRule(
                                   @"(\[)([^\]]+)(\])(::)?",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.PowerShellOperator },
                                       { 2, ScopeName.PowerShellType },
                                       { 3, ScopeName.PowerShellOperator },
                                       { 4, ScopeName.PowerShellOperator },
                                   }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "posh":
                case "ps1":
                    return true;

                default:
                    return false;
            }
        }
    }
}