// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Guilds;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings
{
    /// <summary>
    /// A view model for the guild settings page.
    /// </summary>
    public class GuildSettingsPageViewModel : ObservableObject
    {
        private const string PersonalSettingsResource = "GuildSettings/PersonalSettings";
        private const string ServerSettingsResource = "GuildSettings/ServerSettings";
        private const string UserManagementResource = "GuildSettings/UserManagement";

        private SettingsSubPageViewModel? _selectedSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildSettingsPageViewModel"/>.
        /// </summary>
        public GuildSettingsPageViewModel(ILocalizationService localizationService, BindableGuild guild)
        {
            Pages = new ObservableCollection<ISettingsMenuItem>();

            // Personal Settings
            Pages.Add(new SettingsCategoryHeader(localizationService, PersonalSettingsResource));

            // Server Settings
            Pages.Add(new SettingsCategoryHeader(localizationService, ServerSettingsResource));

            // User management
            Pages.Add(new SettingsCategoryHeader(localizationService, UserManagementResource));
        }

        /// <summary>
        /// Gets the view model of the selected sub page.
        /// </summary>
        public SettingsSubPageViewModel? SelectedSubPage
        {
            get => _selectedSubPage;
            set => SetProperty(ref _selectedSubPage, value);
        }

        /// <summary>
        /// Gets the view models of all sub page options.
        /// </summary>
        public ObservableCollection<ISettingsMenuItem> Pages { get; }
    }
}
