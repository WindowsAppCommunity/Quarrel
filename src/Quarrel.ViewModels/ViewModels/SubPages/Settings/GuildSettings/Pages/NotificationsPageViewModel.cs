// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages
{
    /// <summary>
    /// A view model for the notifications page in guild settings.
    /// </summary>
    public class NotificationsPageViewModel : GuildSettingsSubPageViewModel
    {
        private const string NotificationsResource = "GuildSettings/Notifications";

        internal NotificationsPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(localizationService, discordService, guild)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[NotificationsResource];

        /// <inheritdoc/>
        protected override void ApplyChanges()
        {
        }
    }
}
