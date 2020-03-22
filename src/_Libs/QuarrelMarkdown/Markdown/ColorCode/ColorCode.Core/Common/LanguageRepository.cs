// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// Manages the collection of languages.
    /// </summary>
    public class LanguageRepository : ILanguageRepository
    {
        private readonly Dictionary<string, ILanguage> loadedLanguages;
        private readonly ReaderWriterLockSlim loadLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageRepository"/> class.
        /// </summary>
        /// <param name="loadedLanguages">Set of languages to load.</param>
        public LanguageRepository(Dictionary<string, ILanguage> loadedLanguages)
        {
            this.loadedLanguages = loadedLanguages;
            loadLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Gets all loaded <see cref="ILanguage"/>s.
        /// </summary>
        public IEnumerable<ILanguage> All
        {
            get { return loadedLanguages.Values; }
        }

        /// <summary>
        /// Gets an <see cref="ILanguage"/> by Id.
        /// </summary>
        /// <param name="languageId">The Id of the <see cref="ILanguage"/> to load.</param>
        /// <returns>The <see cref="ILanguage"/> with Id <paramref name="languageId"/>.</returns>
        public ILanguage FindById(string languageId)
        {
            Guard.ArgNotNullAndNotEmpty(languageId, "languageId");
            ILanguage language = null;
            loadLock.EnterReadLock();

            try
            {
                // If we have a matching name for the language then use it
                // otherwise check if any languages have that string as an
                // alias. For example: "js" is an alias for Javascript.
                language = loadedLanguages.FirstOrDefault(x => (x.Key.ToLower() == languageId.ToLower()) ||
                                                               x.Value.HasAlias(languageId)).Value;
            }
            finally
            {
                loadLock.ExitReadLock();
            }

            return language;
        }

        /// <summary>
        /// Adds an <see cref="ILanguage"/> to <see cref="loadedLanguages"/>.
        /// </summary>
        /// <param name="language">The language to add.</param>
        public void Load(ILanguage language)
        {
            Guard.ArgNotNull(language, "language");

            if (string.IsNullOrEmpty(language.Id))
            {
                throw new ArgumentException("The language identifier must not be null or empty.", "language");
            }

            loadLock.EnterWriteLock();

            try
            {
                loadedLanguages[language.Id] = language;
            }
            finally
            {
                loadLock.ExitWriteLock();
            }
        }
    }
}