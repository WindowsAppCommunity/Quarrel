// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages
{
    /// <summary>
    /// A view model for the emojis page in guild settings.
    /// </summary>
    public class EmojisPageViewModel : GuildSettingsSubPageViewModel
    {
        private const string EmojisResource = "GuildSettings/Emojis";

        internal EmojisPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(localizationService, discordService, guild)
        {
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[EmojisResource];

        /// <inheritdoc/>
        protected override void ApplyChanges()
        {
        }
    }
}
