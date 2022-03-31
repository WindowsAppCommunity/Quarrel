// Adam Dernis © 2022

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
        /// Gets a value indicating whether or not the current language is written right to left.
        /// </summary>
        bool IsRightToLeftLanguage { get; }
    }
}
