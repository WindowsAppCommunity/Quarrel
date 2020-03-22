// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Haskell language rules.
    /// </summary>
    public class Haskell : ILanguage
    {
        private const string NonnestComment = @"((?:--.*\r?\n|{-(?:[^-]|-(?!})|[\r\n])*-}))";

        private const string Incomment = @"([^-{}]|{(?!-)|-(?!})|(?<!-)})*";
        private const string Keywords = @"case|class|data|default|deriving|do|else|foreign|if|import|in|infix|infixl|infixr|instance|let|module|newtype|of|then|type|where";
        private const string OpKeywords = @"\.\.|:|::|=|\\|\||<-|->|@|~|=>";

        private const string Vsymbol = @"[\!\#\$%\&\⋆\+\./<=>\?@\\\^\-~\|]";
        private const string Symbol = @"(?:" + Vsymbol + "|:)";

        private const string Varop = Vsymbol + "(?:" + Symbol + @")*";
        private const string Conop = ":(?:" + Symbol + @")*";

        private const string Conid = @"(?:[A-Z][\w']*|\(" + Conop + @"\))";
        private const string Varid = @"(?:[a-z][\w']*|\(" + Varop + @"\))";

        private const string Qconid = @"((?:[A-Z][\w']*\.)*)" + Conid;
        private const string Qvarid = @"((?:[A-Z][\w']*\.)*)" + Varid;
        private const string Qconidop = @"((?:[A-Z][\w']*\.)*)(?:" + Conid + "|" + Conop + ")";

        private const string InType = @"(\bforall\b|=>)|" + Qconidop + @"|(?!deriving|where|data|type|newtype|instance|class)([a-z][\w']*)|\->|[ \t!\#]|\r?\n[ \t]+(?=[\(\)\[\]]|->|=>|[A-Z])";
        private const string TopType = "(?:" + InType + "|::)";
        private const string NestedType = @"(?:" + InType + ")";

        private const string DataType = "(?:" + InType + @"|[,]|\r?\n[ \t]+|::|(?<!" + Symbol + @"|^)([=\|])\s*(" + Conid + ")|" + NonnestComment + ")";

        private const string InExports = @"(?:[\[\],\s]|(" + Conid + ")|" + Varid
                                          + "|" + NonnestComment
                                          + @"|\((?:[,\.\s]|(" + Conid + ")|" + Varid + @")*\)"
                                          + ")*";

        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "haskell", "hs" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Haskell; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "Haskell"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "haskell"; }
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
        public IList<LanguageRule> Rules => new List<LanguageRule>
        {
                    // Nested block comments: note does not match no unclosed block comments.
                    new LanguageRule(
                        @"{-+" + Incomment +
                        @"(?>" +
                        @"(?>(?<comment>{-)" + Incomment + ")+" +
                        @"(?>(?<-comment>-})" + Incomment + ")+" +
                        @")*" +
                        @"(-+})",
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Comment },
                            }),

                    // Line comments
                    new LanguageRule(
                        @"(--.*?)\r?$",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Comment },
                            }),

                    // Types
                    new LanguageRule(
                        @"(?:" + @"\b(class|instance|deriving)\b"
                        + @"|::(?!" + Symbol + ")"
                        + @"|\b(type)\s+" + TopType + @"*\s*(=)"
                        + @"|\b(data|newtype)\s+" + TopType + @"*\s*(=)\s*(" + Conid + ")"
                        + @"|\s+(\|)\s*(" + Conid + ")"
                        + ")" + TopType + "*" +
                        @"(?:" +
                        @"(?:(?<type>[\(\[<])(?:" + NestedType + @"|[,]" + @")*)+" +
                        @"(?:(?<-type>[\)\]>])(?:" + NestedType + @"|(?(type)[,])" + @")*)+" +
                        @")*",
                        new Dictionary<int, string>
                        {
                            { 0, ScopeName.Type },
                            { 1, ScopeName.Keyword },   // class instance etc
                            { 2, ScopeName.Keyword },        // type
                            { 3, ScopeName.Keyword },
                            { 4, ScopeName.NameSpace },
                            { 5, ScopeName.TypeVariable },
                            { 6, ScopeName.Keyword },
                            { 7, ScopeName.Keyword },        // data , newtype
                            { 8, ScopeName.Keyword },
                            { 9, ScopeName.NameSpace },
                            { 10, ScopeName.TypeVariable },
                            { 11, ScopeName.Keyword },       // = conid
                            { 12, ScopeName.Constructor },
                            { 13, ScopeName.Keyword },       // | conid
                            { 14, ScopeName.Constructor },
                            { 15, ScopeName.Keyword },
                            { 16, ScopeName.NameSpace },
                            { 17, ScopeName.TypeVariable },
                            { 18, ScopeName.Keyword },
                            { 19, ScopeName.NameSpace },
                            { 20, ScopeName.TypeVariable },
                            { 21, ScopeName.Keyword },
                            { 22, ScopeName.NameSpace },
                            { 23, ScopeName.TypeVariable },
                        }),

                    // Special sequences
                    new LanguageRule(
                        @"\b(module)\s+(" + Qconid + @")(?:\s*\(" + InExports + @"\))?",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                                { 2, ScopeName.NameSpace },
                                { 4, ScopeName.Type },
                                { 5, ScopeName.Comment },
                                { 6, ScopeName.Constructor },
                            }),
                    new LanguageRule(
                        @"\b(module|as)\s+(" + Qconid + ")",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                                { 2, ScopeName.NameSpace },
                            }),

                    new LanguageRule(
                        @"\b(import)\s+(qualified\s+)?(" + Qconid + @")\s*"
                            + @"(?:\(" + InExports + @"\))?"
                            + @"(?:(hiding)(?:\s*\(" + InExports + @"\)))?",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                                { 2, ScopeName.Keyword },
                                { 3, ScopeName.NameSpace },
                                { 5, ScopeName.Type },
                                { 6, ScopeName.Comment },
                                { 7, ScopeName.Constructor },
                                { 8, ScopeName.Keyword },
                                { 9, ScopeName.Type },
                                { 10, ScopeName.Comment },
                                { 11, ScopeName.Constructor },
                            }),

                    // Keywords
                    new LanguageRule(
                        @"\b(" + Keywords + @")\b",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                            }),
                    new LanguageRule(
                        @"(?<!" + Symbol + ")(" + OpKeywords + ")(?!" + Symbol + ")",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                            }),

                    // Names
                    new LanguageRule(
                        Qvarid,
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.NameSpace },
                            }),
                    new LanguageRule(
                        Qconid,
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Constructor },
                                { 1, ScopeName.NameSpace },
                            }),

                    // Operators and punctuation
                    new LanguageRule(
                        Varop,
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Operator },
                            }),
                    new LanguageRule(
                        Conop,
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Constructor },
                            }),

                    new LanguageRule(
                        @"[{}\(\)\[\];,]",
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Delimiter },
                            }),

                    // Literals
                    new LanguageRule(
                        @"0[xX][\da-fA-F]+|\d+(\.\d+([eE][\-+]?\d+)?)?",
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Number },
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
        };

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "hs":
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