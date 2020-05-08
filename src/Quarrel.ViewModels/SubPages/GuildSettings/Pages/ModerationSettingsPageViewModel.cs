// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Moderation Setting page data.
    /// </summary>
    public class ModerationSettingsPageViewModel : ViewModelBase
    {
        private BindableGuild _guild;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerationSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to modify.</param>
        public ModerationSettingsPageViewModel(BindableGuild guild)
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
        /// Gets or sets a value indicating whether or not the Verification level is 0.
        /// </summary>
        public bool VerficationLevel0
        {
            get => Guild?.Model?.VerificationLevel == 0;
            set
            {
                SetVerifcationLevel(0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Verification level is 1.
        /// </summary>
        public bool VerficationLevel1
        {
            get => Guild?.Model?.VerificationLevel == 1;
            set
            {
                SetVerifcationLevel(1);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Verification level is 2.
        /// </summary>
        public bool VerficationLevel2
        {
            get => Guild?.Model?.VerificationLevel == 2;
            set
            {
                SetVerifcationLevel(2);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Verification level is 3.
        /// </summary>
        public bool VerficationLevel3
        {
            get => Guild?.Model?.VerificationLevel == 3;
            set
            {
                SetVerifcationLevel(3);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Verification level is 4.
        /// </summary>
        public bool VerficationLevel4
        {
            get => Guild?.Model?.VerificationLevel == 4;
            set
            {
                SetVerifcationLevel(4);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter level is 0.
        /// </summary>
        public bool FilterLevel0
        {
            get => Guild?.Model?.ExplicitContentFilter == 0;
            set
            {
                SetFilterLevel(0);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter level is 1.
        /// </summary>
        public bool FilterLevel1
        {
            get => Guild?.Model?.ExplicitContentFilter == 1;
            set
            {
                SetFilterLevel(1);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter level is 2.
        /// </summary>
        public bool FilterLevel2
        {
            get => Guild?.Model?.ExplicitContentFilter == 2;
            set
            {
                SetFilterLevel(2);
            }
        }

        private async void SetVerifcationLevel(int level)
        {
            ModifyGuild modify = new ModifyGuild(Guild.Model)
            {
                VerificationLevel = level,
            };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);
            Guild.Model.VerificationLevel = level;
        }

        private async void SetFilterLevel(int level)
        {
            ModifyGuild modify = new ModifyGuild(Guild.Model)
            {
                ExplicitContentFilter = level,
            };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);
            Guild.Model.ExplicitContentFilter = level;
        }
    }
}
