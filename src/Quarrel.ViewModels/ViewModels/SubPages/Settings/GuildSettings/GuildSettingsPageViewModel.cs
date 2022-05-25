// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.Abstract;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings
{
    /// <summary>
    /// A view model for the guild settings page.
    /// </summary>
    public class GuildSettingsPageViewModel : SettingsPageViewModel
    {
        private const string PersonalSettingsResource = "GuildSettings/PersonalSettings";
        private const string ServerSettingsResource = "GuildSettings/ServerSettings";
        private const string UserManagementResource = "GuildSettings/UserManagement";

        private SettingsSubPageViewModel? _selectedSubPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildSettingsPageViewModel"/>.
        /// </summary>
        public GuildSettingsPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(new ISettingsMenuItem[]
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
            })
        {
            SelectedSubPage = (SettingsSubPageViewModel)Pages.FirstOrDefault(x => x is SettingsSubPageViewModel { IsActive: true });
        }
    }
}
