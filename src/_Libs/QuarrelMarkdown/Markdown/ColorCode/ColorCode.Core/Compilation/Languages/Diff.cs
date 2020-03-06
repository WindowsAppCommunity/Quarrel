// Copyright (c) 2015 Christopher Pardi.
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common;
using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Compilation.Languages
{
    /// <summary>
    /// Diff language rules.
    /// </summary>
    public class Diff : ILanguage
    {
        /// <inheritdoc/>
        string[] ILanguage.Aliases => new string[] { "diff", "patch" };

        /// <inheritdoc/>
        public string Id
        {
            get { return LanguageId.Diff; }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return "Diff"; }
        }

        /// <inheritdoc/>
        public string CssClassName
        {
            get { return "diff"; }
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
                                   @"^(((---)|(\*\*\*)) +\d+,\d+ +((----)|(\*\*\*\*))|@@ +\-\d+,\d+ \+\d+,\d+ +@@)(\r?\n?)",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.DiffMeta },
                                       }),
                    new LanguageRule(
                                   @"^(\*{5}).*(\*{5})(\r?\n?)",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.Brackets },
                                       }),
                    new LanguageRule(
                                   @"^((-{3,})|(\*{3,})|(\+{3,})|(Index:)|(={3,})).*(\r?\n?)",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.Brackets },
                                       }),
                    new LanguageRule(
                                   @"(^(\+|!).*(\r?\n?))",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.DiffAddition },
                                       }),
                    new LanguageRule(
                                   @"^\-.*(\r?\n?)",
                                   new Dictionary<int, string>
                                       {
                                           { 0, ScopeName.DiffDeletion },
                                       }),
                           };
            }
        }

        /// <inheritdoc/>
        public bool HasAlias(string lang)
        {
            switch (lang.ToLower())
            {
                case "diff":
                case "patch":
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