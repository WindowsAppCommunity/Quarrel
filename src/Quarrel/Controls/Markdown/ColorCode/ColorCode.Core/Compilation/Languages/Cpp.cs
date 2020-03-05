// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// C++ language rules.
    /// </summary>
    public class Cpp : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "c++", "cpp", "c", "h", "h++", "hpp", "cc" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Cpp; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "C++"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "cplusplus"; }
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
                                   @"(?s)(""[^\n]*?(?<!\\)"")",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.String },
                                       }),
                               new LanguageRule(
                                   @"\b((0b[01\']+)|(0(x|X)[0123456789aAbBcCdDeEfF]+)|([\d']*\.[\d'])|([\d'])+)(u|U|l|L|ul|UL|f|F|b|B|ll|LL|e)?\b",
                                   new Dictionary<int, string>
                                   {
                                       { 0, ScopeName.Number },
                                   }),
                               new LanguageRule(
                                   @"^\s*(\#if|\#else|\#elif|\#endif|\#define|\#undef|\#warning|\#error|\#line|\#pragme|\#ifdef|\#ifnder|\#include|\#region|\#endregion).*?$",
                                   new Dictionary<int, string>
                                   {
                                       { 1, ScopeName.PreprocessorKeyword },
                                   }),
                               new LanguageRule(
                                   @"\b(abstract|array|auto|bool|break|case|catch|char|ref class|class|const|const_cast|continue|default|delegate|delete|deprecated|dllexport|dllimport|do|double|dynamic_cast|each|else|enum|event|explicit|export|extern|false|float|for|friend|friend_as|gcnew|generic|goto|if|in|initonly|inline|int|interface|literal|long|mutable|naked|namespace|new|noinline|noreturn|nothrow|novtable|nullptr|operator|private|property|protected|public|register|reinterpret_cast|return|safecast|sealed|selectany|short|signed|sizeof|static|static_cast|ref struct|struct|switch|template|this|thread|throw|true|try|typedef|typeid|typename|union|unsigned|using|uuid|value|virtual|void|volatile|wchar_t|while)\b",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.Keyword },
                                       }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "c++":
                case "c":
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
