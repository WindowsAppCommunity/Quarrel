// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages
{
    /// <summary>
    /// A view model for the invites page in guild settings.
    /// </summary>
    public class InvitesPageViewModel : GuildSettingsSubPageViewModel
    {
        private const string InvitesResource = "GuildSettings/Invites";

        internal InvitesPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(localizationService, discordService, guild)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[InvitesResource];

        /// <inheritdoc/>
        protected override void ApplyChanges()
        {
        }
    }
}
