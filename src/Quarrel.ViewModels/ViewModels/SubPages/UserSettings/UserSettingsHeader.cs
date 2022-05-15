// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.UserSettings
{
    /// <summary>
    /// A header for a category of user settings menu items.
    /// </summary>
    public class UserSettingsHeader : ObservableObject, IUserSettingsMenuItem
    {
        internal UserSettingsHeader(ILocalizationService localizationService, string resource)
        {
            Title = localizationService[resource];
        }

        /// <inheritdoc/>
        public string Title { get; }
    }
}
