// Quarrel © 2022

using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.Settings
{
    /// <summary>
    /// A header for a category of settings menu items.
    /// </summary>
    public class SettingsCategoryHeader : ISettingsMenuItem
    {
        internal SettingsCategoryHeader(ILocalizationService localizationService, string resource)
        {
            Title = localizationService[resource];
        }

        /// <inheritdoc/>
        public string Title { get; }
    }
}
