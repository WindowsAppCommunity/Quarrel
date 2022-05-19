// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.GuildSettings.Pages.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.GuildSettings
{
    /// <summary>
    /// A view model for the guild settings page.
    /// </summary>
    public class GuildSettingsPageViewModel : ObservableObject
    {
        private const string PersonalSettingsResource = "GuildSettings/PersonalSettings";
        private const string ServerSettingsResource = "GuildSettings/ServerSettings";
        private const string UserManagementResource = "GuildSettings/UserManagement";

        private GuildSettingsSubPageViewModel? _selectedSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildSettingsPageViewModel"/>.
        /// </summary>
        public GuildSettingsPageViewModel(ILocalizationService localizationService)
        {
            Pages = new ObservableCollection<IGuildSettingsMenuItem>();

            // Personal Settings
            Pages.Add(new GuildSettingsHeader(localizationService, PersonalSettingsResource));

            // Server Settings
            Pages.Add(new GuildSettingsHeader(localizationService, ServerSettingsResource));

            // User management
            Pages.Add(new GuildSettingsHeader(localizationService, UserManagementResource));
        }

        /// <summary>
        /// Gets the view model of the selected sub page.
        /// </summary>
        public GuildSettingsSubPageViewModel? SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        /// <summary>
        /// Gets the view models of all sub page options.
        /// </summary>
        public ObservableCollection<IGuildSettingsMenuItem> Pages { get; }
    }
}
