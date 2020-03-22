// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// XML language rules.
    /// </summary>
    public class Xml : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "xml", "xaml", "xhtml", "rss", "atom", "xjb", "xsd", "xsl", "plist" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Xml; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "XML"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "xml"; }
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
                                   @"\<![ \r\n\t]*(--([^\-]|[\r\n]|-[^\-])*--[ \r\n\t]*)\>",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.HtmlComment },
                                   }),
                               new LanguageRule(
                                   @"(?i)(<!)(doctype)(?:\s+([a-z0-9]+))*(?:\s+("")([^\n]*?)(""))*(>)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.XmlDelimiter },
                                       { 2, ScopeName.XmlName },
                                       { 3, ScopeName.XmlAttribute },
                                       { 4, ScopeName.XmlAttributeQuotes },
                                       { 5, ScopeName.XmlAttributeValue },
                                       { 6, ScopeName.XmlAttributeQuotes },
                                       { 7, ScopeName.XmlDelimiter },
                                   }),
                               new LanguageRule(
                                   @"(?i)(<\?)(xml-stylesheet)((?:\s+[a-z0-9]+=""[^\n""]*"")*(?:\s+[a-z0-9]+=\'[^\n\']*\')*\s*?)(\?>)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.XmlDelimiter },
                                       { 2, ScopeName.XmlName },
                                       { 3, ScopeName.XmlDocTag },
                                       { 4, ScopeName.XmlDelimiter },
                                   }),
                               new LanguageRule(
                                   @"(?i)(<\?)([a-z][a-z0-9-]*)(?:\s+([a-z0-9]+)(=)("")([^\n]*?)(""))*(?:\s+([a-z0-9]+)(=)(\')([^\n]*?)(\'))*\s*?(\?>)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.XmlDelimiter },
                                           { 2, ScopeName.XmlName },
                                           { 3, ScopeName.XmlAttribute },
                                           { 4, ScopeName.XmlDelimiter },
                                           { 5, ScopeName.XmlAttributeQuotes },
                                           { 6, ScopeName.XmlAttributeValue },
                                           { 7, ScopeName.XmlAttributeQuotes },
                                           { 8, ScopeName.XmlAttribute },
                                           { 9, ScopeName.XmlDelimiter },
                                           { 10, ScopeName.XmlAttributeQuotes },
                                           { 11, ScopeName.XmlAttributeValue },
                                           { 12, ScopeName.XmlAttributeQuotes },
                                           { 13, ScopeName.XmlDelimiter },
                                       }),
                               new LanguageRule(
                                   @"(?xi)(</?)
                                          (?: ([a-z][a-z0-9-]*)(:) )*
                                          ([a-z][a-z0-9-_\.]*)
                                          (?:
                                            |[\s\n]+([a-z0-9-_\.:]+)[\s\n]*(=)[\s\n]*("")([^\n]+?)("")
                                            |[\s\n]+([a-z0-9-_\.:]+)[\s\n]*(=)[\s\n]*(')([^\n]+?)(')
                                            |[\s\n]+([a-z0-9-_\.:]+) )*
                                          [\s\n]*
                                          (/?>)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.XmlDelimiter },
                                           { 2, ScopeName.XmlName },
                                           { 3, ScopeName.XmlDelimiter },
                                           { 4, ScopeName.XmlName },
                                           { 5, ScopeName.XmlAttribute },
                                           { 6, ScopeName.XmlDelimiter },
                                           { 7, ScopeName.XmlAttributeQuotes },
                                           { 8, ScopeName.XmlAttributeValue },
                                           { 9, ScopeName.XmlAttributeQuotes },
                                           { 10, ScopeName.XmlAttribute },
                                           { 11, ScopeName.XmlDelimiter },
                                           { 12, ScopeName.XmlAttributeQuotes },
                                           { 13, ScopeName.XmlAttributeValue },
                                           { 14, ScopeName.XmlAttributeQuotes },
                                           { 15, ScopeName.XmlAttribute },
                                           { 16, ScopeName.XmlDelimiter },
                                       }),
                               new LanguageRule(
                                   @"(?i)&[a-z0-9]+?;",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.XmlAttribute },
                                   }),
                               new LanguageRule(
                                   @"(?s)(<!\[CDATA\[)(.*?)(\]\]>)",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.XmlDelimiter },
                                       { 2, ScopeName.XmlCDataSection },
                                       { 3, ScopeName.XmlDelimiter },
                                   }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "xaml":
                case "xhtml":
                case "rss":
                case "atom":
                case "xjb":
                case "xsd":
                case "xsl":
                case "plist":
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