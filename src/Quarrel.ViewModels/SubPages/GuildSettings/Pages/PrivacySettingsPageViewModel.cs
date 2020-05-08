// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Privacy Setting page data.
    /// </summary>
    public class PrivacySettingsPageViewModel : ViewModelBase
    {
        private BindableGuild _guild;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivacySettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to modify.</param>
        public PrivacySettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        /// <summary>
        /// Gets or sets the guild being modified.
        /// </summary>
        public BindableGuild Guild
        {
            get => _guild;
            set => Set(ref _guild, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the user allows DMs from members of this server.
        /// </summary>
        public bool AllowDMs
        {
            get => !CurrentUserService.CurrentUserSettings.RestrictedGuilds.Contains(Guild.Model.Id);
            set
            {
                List<string> restrictedGuilds = CurrentUserService.CurrentUserSettings.RestrictedGuilds.ToList();

                if (restrictedGuilds.Contains(Guild.Model.Id))
                {
                    restrictedGuilds.Remove(Guild.Model.Id);
                }
                else
                {
                    restrictedGuilds.Add(Guild.Model.Id);
                }

                ModifyUserSettings modify = new ModifyUserSettings(CurrentUserService.CurrentUserSettings)
                {
                    RestrictedGuilds = restrictedGuilds.ToArray(),
                };

                ModifySettings(modify);
            }
        }

        private ICurrentUserService CurrentUserService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        /// <summary>
        /// Finalizes modifications.
        /// </summary>
        /// <param name="modify">The user settings progress to apply.</param>
        public async void ModifySettings(ModifyUserSettings modify)
        {
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings(modify);
        }
    }
}