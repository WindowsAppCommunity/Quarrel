// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation
{
    /// <summary>
    /// Gets capute rules.
    /// </summary>
    public static class RuleCaptures
    {
        static RuleCaptures()
        {
            JavaScript = BuildCaptures(LanguageId.JavaScript);
            CSharpScript = BuildCaptures(LanguageId.CSharp);
            VbDotNetScript = BuildCaptures(LanguageId.VbDotNet);
        }

        /// <summary>
        /// Gets the JavaScript like capture rules.
        /// </summary>
        public static IDictionary<int, string> JavaScript { get; }

        /// <summary>
        /// Gets the C# like capture rules.
        /// </summary>
        public static IDictionary<int, string> CSharpScript { get; }

        /// <summary>
        /// Gets the Visual Basic .Net like capture rules.
        /// </summary>
        public static IDictionary<int, string> VbDotNetScript { get; }

        private static IDictionary<int, string> BuildCaptures(string languageId)
        {
            return new Dictionary<int, string>
                       {
                           { 1, ScopeName.HtmlTagDelimiter },
                           { 2, ScopeName.HtmlElementName },
                           { 3, ScopeName.HtmlAttributeName },
                           { 4, ScopeName.HtmlOperator },
                           { 5, ScopeName.HtmlAttributeValue },
                           { 6, ScopeName.HtmlAttributeName },
                           { 7, ScopeName.HtmlOperator },
                           { 8, ScopeName.HtmlAttributeValue },
                           { 9, ScopeName.HtmlAttributeName },
                           { 10, ScopeName.HtmlOperator },
                           { 11, ScopeName.HtmlAttributeValue },
                           { 12, ScopeName.HtmlAttributeName },
                           { 13, ScopeName.HtmlAttributeName },
                           { 14, ScopeName.HtmlOperator },
                           { 15, ScopeName.HtmlAttributeValue },
                           { 16, ScopeName.HtmlAttributeName },
                           { 17, ScopeName.HtmlOperator },
                           { 18, ScopeName.HtmlAttributeValue },
                           { 19, ScopeName.HtmlAttributeName },
                           { 20, ScopeName.HtmlOperator },
                           { 21, ScopeName.HtmlAttributeValue },
                           { 22, ScopeName.HtmlAttributeName },
                           { 23, ScopeName.HtmlOperator },
                           { 24, ScopeName.HtmlAttributeValue },
                           { 25, ScopeName.HtmlAttributeName },
                           { 26, ScopeName.HtmlTagDelimiter },
                           { 27, string.Format("{0}{1}", ScopeName.LanguagePrefix, languageId) },
                           { 28, ScopeName.HtmlTagDelimiter },
                           { 29, ScopeName.HtmlElementName },
                           { 30, ScopeName.HtmlTagDelimiter },
                       };
        }
    }
}