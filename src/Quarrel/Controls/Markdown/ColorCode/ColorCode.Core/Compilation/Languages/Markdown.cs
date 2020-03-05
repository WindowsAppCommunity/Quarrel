// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Markdown language rules.
    /// </summary>
    public class Markdown : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "markdown", "md" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Markdown; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "Markdown"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "markdown"; }
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
                               // Heading
                               new LanguageRule(
                                   @"^(\#.*)\r?|^.*\r?\n([-]+|[=]+)\r?$",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.MarkdownHeader },
                                   }),

                               // code block
                               new LanguageRule(
                                   @"^([ ]{4}(?![ ])(?:.|\r?\n[ ]{4})*)|^(```+[ \t]*\w*)((?:[ \t\r\n]|.)*?)(^```+)[ \t]*\r?$",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.MarkdownCode },
                                       { 2, ScopeName.XmlDocTag },
                                       { 3, ScopeName.MarkdownCode },
                                       { 4, ScopeName.XmlDocTag },
                                   }),

                               // Line
                               new LanguageRule(
                                   @"^\s*((-\s*){3}|(\*\s*){3}|(=\s*){3})[ \t\-\*=]*\r?$",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.MarkdownHeader },
                                   }),

                               // List
                               new LanguageRule(
                                   @"^[ \t]*([\*\+\-]|\d+\.)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.MarkdownListItem },
                                   }),

                               // escape
                               new LanguageRule(
                                   @"\\[\\`\*_{}\[\]\(\)\#\+\-\.\!]",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.StringEscape },
                                   }),

                               // link
                               new LanguageRule(
                                   Link(Link()) + "|" + Link(),  // support nested links (mostly for images)
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.MarkdownBold },
                                           { 2, ScopeName.XmlDocTag },
                                           { 3, ScopeName.XmlDocTag },
                                           { 4, ScopeName.MarkdownBold },
                                           { 5, ScopeName.XmlDocTag },
                                       }),
                               new LanguageRule(
                                   @"^[ ]{0,3}\[[^\]\n]+\]:(.|\r|\n[ \t]+(?![\r\n]))*$",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.XmlDocTag },    // nice gray
                                       }),

                               // bold
                               new LanguageRule(
                                   @"\*(?!\*)([^\*\n]|\*\w)+?\*(?!\w)|\*\*([^\*\n]|\*(?!\*))+?\*\*",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.MarkdownBold },
                                   }),

                               // emphasized
                               new LanguageRule(
                                   @"_([^_\n]|_\w)+?_(?!\w)|__([^_\n]|_(?=[\w_]))+?__(?!\w)",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.MarkdownEmph },
                                   }),

                               // inline code
                               new LanguageRule(
                                   @"`[^`\n]+?`|``([^`\n]|`(?!`))+?``",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.MarkdownCode },
                                   }),

                               // strings
                               new LanguageRule(
                                   @"""[^""\n]+?""|'[\w\-_]+'",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.String },
                                   }),

                               // html tag
                               new LanguageRule(
                                   @"</?\w.*?>",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.HtmlTagDelimiter },
                                   }),

                               // html entity
                               new LanguageRule(
                                   @"\&\#?\w+?;",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.HtmlEntity },
                                   }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "md":
                case "markdown":
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

        private string Link(string content = @"([^\]\n]+)")
        {
            return @"\!?\[" + content + @"\][ \t]*(\([^\)\n]*\)|\[[^\]\n]*\])";
        }
    }
}