// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Ashx language rules.
    /// </summary>
    public class Ashx : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "ashx" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Ashx; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "ASHX"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "ashx"; }
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
                                   @"(<%)(--.*?--)(%>)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.HtmlServerSideScript },
                                           { 2, ScopeName.HtmlComment },
                                           { 3, ScopeName.HtmlServerSideScript },
                                       }),
                               new LanguageRule(
                                   @"(?is)(?<=<%@.+?language=""c\#"".*?%>)(.*)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, string.Format("{0}{1}", ScopeName.LanguagePrefix, LanguageId.CSharp) },
                                       }),
                               new LanguageRule(
                                   @"(?is)(?<=<%@.+?language=""vb"".*?%>)(.*)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, string.Format("{0}{1}", ScopeName.LanguagePrefix, LanguageId.VbDotNet) },
                                       }),
                               new LanguageRule(
                                   @"(<%)(@)(?:\s+([a-zA-Z0-9]+))*(?:\s+([a-zA-Z0-9]+)(=)(""[^\n]*?""))*\s*?(%>)",
                                   new Dictionary<int, string>
                                       {
                                           { 1, ScopeName.HtmlServerSideScript },
                                           { 2, ScopeName.HtmlTagDelimiter },
                                           { 3, ScopeName.HtmlElementName },
                                           { 4, ScopeName.HtmlAttributeName },
                                           { 5, ScopeName.HtmlOperator },
                                           { 6, ScopeName.HtmlAttributeValue },
                                           { 7, ScopeName.HtmlServerSideScript },
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