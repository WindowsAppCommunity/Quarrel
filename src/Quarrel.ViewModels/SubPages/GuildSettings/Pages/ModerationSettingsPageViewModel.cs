using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    public class ModerationSettingsPageViewModel : ViewModelBase
    {
        public ModerationSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        #region Methods

        public async void SetVerifcationLevel(int level)
        {
            ModifyGuild modify = new ModifyGuild(Guild.Model)
            {
                VerificationLevel = level
            };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);
            Guild.Model.VerificationLevel = level;
            // TODO: Handle Guild Updated
        }

        public async void SetFilterLevel(int level)
        {
            ModifyGuild modify = new ModifyGuild(Guild.Model)
            {
                ExplicitContentFilter = level
            };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);
            Guild.Model.ExplicitContentFilter = level;
            // TODO: Handle Guild Updated
        }

        #endregion

        #region Properties

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }
        private BindableGuild _Guild;

        #region Verfication Levels

        public bool VerficationLevel0
        {
            get => Guild?.Model?.VerificationLevel == 0;
            set
            {
                SetVerifcationLevel(0);
            }
        }
        public bool VerficationLevel1
        {
            get => Guild?.Model?.VerificationLevel == 1;
            set
            {
                SetVerifcationLevel(1);
            }
        }
        public bool VerficationLevel2
        {
            get => Guild?.Model?.VerificationLevel == 2;
            set
            {
                SetVerifcationLevel(2);
            }
        }
        public bool VerficationLevel3
        {
            get => Guild?.Model?.VerificationLevel == 3;
            set
            {
                SetVerifcationLevel(3);
            }
        }
        public bool VerficationLevel4
        {
            get => Guild?.Model?.VerificationLevel == 4;
            set
            {
                SetVerifcationLevel(4);
            }
        }

        #endregion

        #region ExplicitContentFilter

        public bool FilterLevel0
        {
            get => Guild?.Model?.ExplicitContentFilter == 0;
            set
            {
                SetFilterLevel(0);
            }
        }
        public bool FilterLevel1
        {
            get => Guild?.Model?.ExplicitContentFilter == 1;
            set
            {
                SetFilterLevel(1);
            }
        }
        public bool FilterLevel2
        {
            get => Guild?.Model?.ExplicitContentFilter == 2;
            set
            {
                SetFilterLevel(2);
            }
        }
        
        #endregion

        #endregion
    }
}
