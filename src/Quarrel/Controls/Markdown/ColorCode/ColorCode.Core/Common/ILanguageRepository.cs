// Copyright (c) Microsoft Corporation.  All rights reserved.
// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;

namespace Quarrel.Controls.Markdown.ColorCode.ColorCode.Core.Common
{
    /// <summary>
    /// Manages the collection <see cref="ILanguage"/>s.
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Gets all <see cref="ILanguage"/>s.
        /// </summary>
        IEnumerable<ILanguage> All { get; }

        /// <summary>
        /// Gets an <see cref="ILanguage"/> by Id.
        /// </summary>
        /// <param name="languageId">Id of the <see cref="ILanguage"/> to get.</param>
        /// <returns>The <see cref="ILanguage"/> with id <paramref name="languageId"/>.</returns>
        ILanguage FindById(string languageId);

        /// <summary>
        /// Loads an <see cref="ILanguage"/>.
        /// </summary>
        /// <param name="language"><see cref="ILanguage"/> to load.</param>
        void Load(ILanguage language);
    }
}