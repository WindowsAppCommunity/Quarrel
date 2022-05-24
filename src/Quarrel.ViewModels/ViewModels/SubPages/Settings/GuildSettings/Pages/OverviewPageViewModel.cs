// Quarrel © 2022

using Quarrel.Bindables.Guilds;
using Quarrel.Services.Discord;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages.Abstract;

namespace Quarrel.ViewModels.SubPages.Settings.GuildSettings.Pages
{
    /// <summary>
    /// A view model for the overview page in guild settings.
    /// </summary>
    public class OverviewPageViewModel : GuildSettingsSubPageViewModel
    {
        private const string OverviewResource = "GuildSettings/Overview";

        private string _name;

        internal OverviewPageViewModel(ILocalizationService localizationService, IDiscordService discordService, BindableGuild guild) :
            base(localizationService, discordService, guild)
        {
            ResetValues(guild);
        }

        /// <inheritdoc/>
        public override string Glyph => "";

        /// <inheritdoc/>
        public override string Title => _localizationService[OverviewResource];

        /// <inheritdoc/>
        public override bool IsActive => true;

        /// <summary>
        /// Gets or sets the guild's name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private void ResetValues(BindableGuild guild)
        {
            Name = guild.Guild.Name;
        }
    }
}
