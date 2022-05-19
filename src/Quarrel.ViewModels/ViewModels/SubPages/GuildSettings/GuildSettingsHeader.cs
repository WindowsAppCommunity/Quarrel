// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.GuildSettings
{
    /// <summary>
    /// A header for a category of guild settings menu items.
    /// </summary>
    public class GuildSettingsHeader : ObservableObject, IGuildSettingsMenuItem
    {
        internal GuildSettingsHeader(ILocalizationService localizationService, string resource)
        {
            Title = localizationService[resource];
        }

        /// <inheritdoc/>
        public string Title { get; }
    }
}
