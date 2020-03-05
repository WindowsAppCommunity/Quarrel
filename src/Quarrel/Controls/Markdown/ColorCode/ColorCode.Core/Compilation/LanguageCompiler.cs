// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation
{
    /// <summary>
    /// Compiles a language into a <see cref="CompiledLanguage"/> for language color coding.
    /// </summary>
    public class LanguageCompiler : ILanguageCompiler
    {
        private static readonly System.Text.RegularExpressions.Regex NumberOfCapturesRegex = new System.Text.RegularExpressions.Regex(@"(?x)(?<!(\\|(?!\\)\(\?))\((?!\?)", RegexOptions.Compiled);
        private readonly Dictionary<string, CompiledLanguage> compiledLanguages;
        private readonly ReaderWriterLockSlim compileLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageCompiler"/> class.
        /// </summary>
        /// <param name="compiledLanguages">Pre compiled languages.</param>
        public LanguageCompiler(Dictionary<string, CompiledLanguage> compiledLanguages)
        {
            this.compiledLanguages = compiledLanguages;

            compileLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Creates a new <see cref="CompiledLanguage"/> from a simple <see cref="ILanguage"/>.
        /// </summary>
        /// <param name="language">A languages color style.</param>
        /// <returns>A <see cref="CompiledLanguage"/> for the <see cref="ILanguage"/>.</returns>
        public CompiledLanguage Compile(ILanguage language)
        {
            Guard.ArgNotNull(language, "language");

            if (string.IsNullOrEmpty(language.Id))
            {
                throw new ArgumentException("The language identifier must not be null.", "language");
            }

            CompiledLanguage compiledLanguage;

            compileLock.EnterReadLock();
            try
            {
                // for performance reasons we should first try with
                // only a read lock since the majority of the time
                // it'll be created already and upgradeable lock blocks
                if (compiledLanguages.ContainsKey(language.Id))
                {
                    return compiledLanguages[language.Id];
                }
            }
            finally
            {
                compileLock.ExitReadLock();
            }

            compileLock.EnterUpgradeableReadLock();
            try
            {
                if (compiledLanguages.ContainsKey(language.Id))
                {
                    compiledLanguage = compiledLanguages[language.Id];
                }
                else
                {
                    compileLock.EnterWriteLock();

                    try
                    {
                        if (string.IsNullOrEmpty(language.Name))
                        {
                            throw new ArgumentException("The language name must not be null or empty.", "language");
                        }

                        if (language.Rules == null || language.Rules.Count == 0)
                        {
                            throw new ArgumentException("The language rules collection must not be empty.", "language");
                        }

                        compiledLanguage = CompileLanguage(language);

                        compiledLanguages.Add(compiledLanguage.Id, compiledLanguage);
                    }
                    finally
                    {
                        compileLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                compileLock.ExitUpgradeableReadLock();
            }

            return compiledLanguage;
        }

        private static CompiledLanguage CompileLanguage(ILanguage language)
        {
            string id = language.Id;
            string name = language.Name;

            CompileRules(language.Rules, out System.Text.RegularExpressions.Regex regex, out IList<string> captures);

            return new CompiledLanguage(id, name, regex, captures);
        }

        private static void CompileRules(IList<LanguageRule> rules, out System.Text.RegularExpressions.Regex regex, out IList<string> captures)
        {
            StringBuilder regexBuilder = new StringBuilder();
            captures = new List<string>();

            regexBuilder.AppendLine("(?x)");
            captures.Add(null);

            CompileRule(rules[0], regexBuilder, captures, true);

            for (int i = 1; i < rules.Count; i++)
            {
                CompileRule(rules[i], regexBuilder, captures, false);
            }

            regex = new System.Text.RegularExpressions.Regex(regexBuilder.ToString());
        }

        private static void CompileRule(LanguageRule languageRule, StringBuilder regex, ICollection<string> captures, bool isFirstRule)
        {
            if (!isFirstRule)
            {
                regex.AppendLine();
                regex.AppendLine();
                regex.AppendLine("|");
                regex.AppendLine();
            }

            regex.AppendFormat("(?-xis)(?m)({0})(?x)", languageRule.Regex);

            int numberOfCaptures = GetNumberOfCaptures(languageRule.Regex);

            for (int i = 0; i <= numberOfCaptures; i++)
            {
                string scope = null;

                foreach (int captureIndex in languageRule.Captures.Keys)
                {
                    if (i == captureIndex)
                    {
                        scope = languageRule.Captures[captureIndex];
                        break;
                    }
                }

                captures.Add(scope);
            }
        }

        private static int GetNumberOfCaptures(string regex)
        {
            return NumberOfCapturesRegex.Matches(regex).Count;
        }
    }
}