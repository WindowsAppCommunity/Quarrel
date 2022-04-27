// Quarrel © 2022

using System.Collections.Generic;

namespace Quarrel.Services.Localization
{
    /// <summary>
    /// An interface for a localization service used in the app.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the localized <see langword="string"/> for a given resource.
        /// </summary>
        /// <param name="key">The key of the resource.</param>
        /// <returns>Localized <see langword="string"/> if valid, otherwise returns an empty <see langword="string"/>.</returns>
        string this[string key] { get; }

        /// <summary>
        /// Gets the localized <see langword="string"/> for a given resource.
        /// </summary>
        /// <param name="key">The key of the resource.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>Localized <see langword="string"/> if valid, otherwise returns an empty <see langword="string"/>.</returns>
        string this[string key, params object[] args] { get; }

        /// <summary>
        /// Gets a list of items as as a string with and.
        /// </summary>
        string CommaList(params string[] args);

        public string LanguageOverride { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the current language is written right to left.
        /// </summary>
        bool IsRightToLeftLanguage { get; }

        /// <summary>
        /// Gets a value indicating whether or not the current language is the app's neutral language.
        /// </summary>
        bool IsNeutralLanguage { get; }

        /// <summary>
        /// Gets the list of available languages.
        /// </summary>
        public IReadOnlyList<string> AvailableLanguages { get; }
    }
}
