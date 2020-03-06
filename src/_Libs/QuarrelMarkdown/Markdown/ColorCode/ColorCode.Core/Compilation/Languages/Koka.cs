// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Koka language rules.
    /// </summary>
    public class Koka : ILanguage
    {
        private const string InComment = @"([^*/]|/(?!\*)|\*(?!/))*";

        private const string PlainKeywords = @"infix|infixr|infixl|type|cotype|rectype|struct|alias|interface|instance|external|fun|function|val|var|con|module|import|as|public|private|abstract|yield";
        private const string ControlKeywords = @"if|then|else|elif|match|return";
        private const string TypeKeywords = @"forall|exists|some|with";
        private const string PseudoKeywords = @"include|inline|cs|js|file";
        private const string OpKeywords = @"[=\.\|]|\->|\:=";

        private const string InType = @"\b(" + TypeKeywords + @")\b|(?:[a-z]\w*/)*[a-z]\w+\b|(?:[a-z]\w*/)*[A-Z]\w*\b|([a-z][0-9]*\b|_\w*\b)|\->|[\s\|]";
        private const string TopType = "(?:" + InType + "|::)";
        private const string NestedType = @"(?:([a-z]\w*)\s*[:]|" + InType + ")";

        private const string Symbol = @"[$%&\*\+@!\\\^~=\.:\-\?\|<>/]";
        private const string Symbols = @"(?:" + Symbol + @")+";

        private const string Escape = @"\\(?:[nrt\\""']|x[\da-fA-F]{2}|u[\da-fA-F]{4}|U[\da-fA-F]{6})";

        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "koka", "kk", "kki" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Koka; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "Koka"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "koka"; }
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
                    // Nested block comments. note: does not match on unclosed comments.
                    new LanguageRule(
                      @"/\*" + InComment +
                      @"(" +
                       @"((?<comment>/\*)" + InComment + ")+" +
                       @"((?<-comment>\*/)" + InComment + ")+" +
                      @")*" +
                      @"(\*+/)",
                      new Dictionary<int, string>
                          {
                              { 0, ScopeName.Comment },
                          }),

                   // Line comments
                    new LanguageRule(
                        @"(//.*?)\r?$",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Comment },
                            }),

                    // operator keywords
                    new LanguageRule(
                        @"(?<!" + Symbol + ")(" + OpKeywords + @")(?!" + Symbol + @")",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                            }),

                    // Types
                    new LanguageRule(
                        @"(?:" + @"\b(type|struct|cotype|rectype)\b|"
                        + @"::?(?!" + Symbol + ")|"
                        + @"\b(alias)\s+[a-z]\w+\s*(?:<[^>]*>\s*)?(=)" + ")"
                        + TopType + "*" +
                        @"(?:" +
                        @"(?:(?<type>[\(\[<])(?:" + NestedType + @"|[,]" + @")*)+" +
                        @"(?:(?<-type>[\)\]>])(?:" + NestedType + @"|(?(type)[,])" + @")*)+" +
                        @")*",
                        new Dictionary<int, string>
                        {
                            { 0, ScopeName.Type },
                            { 1, ScopeName.Keyword },   // type struct etc
                            { 2, ScopeName.Keyword },   // alias
                            { 3, ScopeName.Keyword },   //  =
                            { 4, ScopeName.Keyword },
                            { 5, ScopeName.TypeVariable },
                            { 6, ScopeName.PlainText },
                            { 7, ScopeName.Keyword },
                            { 8, ScopeName.TypeVariable },
                            { 9, ScopeName.PlainText },
                            { 10, ScopeName.Keyword },
                            { 11, ScopeName.TypeVariable },
                        }),

                    // module and imports
                    new LanguageRule(
                        @"\b(module)\s+(interface\s+)?((?:[a-z]\w*/)*[a-z]\w*)",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                                { 2, ScopeName.Keyword },
                                { 3, ScopeName.NameSpace },
                            }),

                    new LanguageRule(
                        @"\b(import)\s+((?:[a-z]\w*/)*[a-z]\w*)\s*(?:(=)\s*(qualified\s+)?((?:[a-z]\w*/)*[a-z]\w*))?",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                                { 2, ScopeName.NameSpace },
                                { 3, ScopeName.Keyword },
                                { 4, ScopeName.Keyword },
                                { 5, ScopeName.NameSpace },
                            }),

                    // keywords
                    new LanguageRule(
                        @"\b(" + PlainKeywords + "|" + TypeKeywords + @")\b",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.Keyword },
                            }),
                    new LanguageRule(
                        @"\b(" + ControlKeywords + @")\b",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.ControlKeyword },
                            }),
                    new LanguageRule(
                        @"\b(" + PseudoKeywords + @")\b",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.PseudoKeyword },
                            }),

                    // Names
                    new LanguageRule(
                        @"([a-z]\w*/)*([a-z]\w*|\(" + Symbols + @"\))",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.NameSpace },
                            }),
                    new LanguageRule(
                        @"([a-z]\w*/)*([A-Z]\w*)",
                        new Dictionary<int, string>
                            {
                                { 1, ScopeName.NameSpace },
                                { 2, ScopeName.Constructor },
                            }),

                    // Operators and punctuation
                    new LanguageRule(
                        Symbols,
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.Operator },
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
                        @"(?s)'(?:[^\t\n\\']+|(" + Escape + @")|\\)*'",
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.String },
                                { 1, ScopeName.StringEscape },
                            }),
                    new LanguageRule(
                        @"(?s)@""(?:("""")|[^""]+)*""(?!"")",
                        new Dictionary<int, string>
                            {
                                { 0, ScopeName.StringCSharpVerbatim },
                                { 1, ScopeName.StringEscape },
                            }),
                    new LanguageRule(
                                @"(?s)""(?:[^\t\n\\""]+|(" + Escape + @")|\\)*""",
                                new Dictionary<int, string>
                                    {
                                        { 0, ScopeName.String },
                                        { 1, ScopeName.StringEscape },
                                    }),
                    new LanguageRule(
                        @"^\s*(\#error|\#line|\#pragma|\#warning|\#!/usr/bin/env\s+koka|\#).*?$",
                        new Dictionary<int, string>
                        {
                            { 1, ScopeName.PreprocessorKeyword },
                        }),
                };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "kk":
                case "kki":
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
