// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages.Abstract
{
    /// <summary>
    /// A base class for guild settings sub-page view models.
    /// </summary>
    public abstract class GuildSettingsSubPageViewModel : SettingsSubPageViewModel
    {
        /// <summary>
        /// The discord service.
        /// </summary>
        protected readonly IDiscordService _discordService;

        /// <summary>
        /// The guild being modified or viewed.
        /// </summary>
        protected readonly BindableGuild _guild;

        internal GuildSettingsSubPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(localizationService)
        {
            _discordService = discordService;
            _guild = guild;
        }
    }
}
