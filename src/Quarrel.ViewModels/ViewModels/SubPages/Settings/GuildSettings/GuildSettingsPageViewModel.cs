// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.Services.Storage;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages;
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
        public GuildSettingsPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild)
        {
            Pages = new ObservableCollection<ISettingsMenuItem>
            {
                new OverviewPageViewModel(localizationService, discordService, guild),

                // Personal Settings
                new SettingsCategoryHeader(localizationService[PersonalSettingsResource]),
                new PrivacyPageViewModel(localizationService, discordService, guild),
                new NotificationsPageViewModel(localizationService, discordService, guild),

                // Server Settings
                new SettingsCategoryHeader(localizationService[ServerSettingsResource]),
                new RolesPageViewModel(localizationService, discordService, guild),
                new EmojisPageViewModel(localizationService, discordService, guild),
                new ModerationPageViewModel(localizationService, discordService, guild),
                new AuditLogPageViewModel(localizationService, discordService, guild),

                // User management
                new SettingsCategoryHeader(localizationService[UserManagementResource]),
                new MembersPageViewModel(localizationService, discordService, guild),
                new InvitesPageViewModel(localizationService, discordService, guild),
                new BansPageViewModel(localizationService, discordService, guild)
            };
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
