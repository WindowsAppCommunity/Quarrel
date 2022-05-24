// Quarrel © 2022

using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.Settings
{
    /// <summary>
    /// A header for a category of settings menu items.
    /// </summary>
    public class SettingsCategoryHeader : ISettingsMenuItem
    {
        internal SettingsCategoryHeader(string title)
        {
            Title = title;
        }

        internal SettingsCategoryHeader(string resource, ILocalizationService localizationService) :
            this(localizationService[resource])
        {
        }

        /// <inheritdoc/>
        public string Title { get; }
    }
}
