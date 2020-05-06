// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Overview settings page data.
    /// </summary>
    public class OverviewSettingsPageViewModel : ViewModelBase
    {
        private BindableGuild _guild;
        private RelayCommand _deleteIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to modify.</param>
        public OverviewSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets a command that deletes the icon for the guild.
        /// </summary>
        public RelayCommand DeleteIcon => _deleteIcon = new RelayCommand(async () =>
        {
            ModifyGuildIcon modify = new ModifyGuildIcon(Guild.Model) { Icon = null };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);

            Guild.Model.Icon = null;
            Guild.RaisePropertyChanged(nameof(Guild.HasIcon));
            Guild.RaisePropertyChanged(nameof(Guild.IconUrl));

            // TODO: Handle Guild Updated
        });

        /// <summary>
        /// Gets or sets the guild being modified.
        /// </summary>
        public BindableGuild Guild
        {
            get => _guild;
            set => Set(ref _guild, value);
        }

        /// <summary>
        /// Gets or sets the name of the guild.
        /// </summary>
        public string Name
        {
            get => Guild.Model.Name;
            set
            {
                SimpleIoc.Default.GetInstance<IDiscordService>()
                    .GuildService.ModifyGuild(Guild.Model.Id, new ModifyGuild(Guild.Model) { Name = value });
                Guild.Model.Name = value;
                Guild.RaisePropertyChanged(nameof(Guild.Model));
                Guild.RaisePropertyChanged(nameof(Guild.DisplayText));

                // TODO: Handle Guild Updated
            }
        }

        /// <summary>
        /// Updates the icon for the guild.
        /// </summary>
        /// <param name="base64Icon">The new icon in base64 string format.</param>
        public async void UpdateIcon(string base64Icon)
        {
            ModifyGuildIcon modify = new ModifyGuildIcon(Guild.Model) { Icon = base64Icon };
            try
            {
                var guild = await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);
                Guild.Model.Icon = guild.Icon;
                Guild.RaisePropertyChanged(nameof(Guild.HasIcon));
                Guild.RaisePropertyChanged(nameof(Guild.IconUrl));
            }
            catch
            {
                // Mainly for Rate Limit
            }
        }
    }
}
