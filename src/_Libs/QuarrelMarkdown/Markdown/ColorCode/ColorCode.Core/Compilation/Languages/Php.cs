// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// PHP language rules.
    /// </summary>
    public class Php : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "php" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Php; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "PHP"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "php"; }
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
                                   @"(#.*?)\r?$",
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
                                   @"""[^\n]*?(?<!\\)""",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),
                               new LanguageRule(
                                   @"\b(abstract|and|array|as|break|case|catch|cfunction|class|clone|const|continue|declare|default|do|else|elseif|enddeclare|endfor|endforeach|endif|endswitch|endwhile|exception|extends|fclose|file|final|for|foreach|function|global|goto|if|implements|interface|instanceof|mysqli_fetch_object|namespace|new|old_function|or|php_user_filter|private|protected|public|static|switch|throw|try|use|var|while|xor|__CLASS__|__DIR__|__FILE__|__FUNCTION__|__LINE__|__METHOD__|__NAMESPACE__|die|echo|empty|exit|eval|include|include_once|isset|list|require|require_once|return|print|unset)\b",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.Keyword },
                                   }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "php3":
                case "php4":
                case "php5":
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
