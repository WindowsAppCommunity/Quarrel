using GalaSoft.MvvmLight;
using Quarrel.ViewModels.Models.Bindables;
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

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }
        private BindableGuild _Guild;

        #region Properties

        #region Verfication Levels

        public bool VerficationLevel0
        {
            get => Guild?.Model?.VerificationLevel == 0;
        }
        public bool VerficationLevel1
        {
            get => Guild?.Model?.VerificationLevel == 1;
        }
        public bool VerficationLevel2
        {
            get => Guild?.Model?.VerificationLevel == 2;
        }
        public bool VerficationLevel3
        {
            get => Guild?.Model?.VerificationLevel == 3;
        }
        public bool VerficationLevel4
        {
            get => Guild?.Model?.VerificationLevel == 4;
        }

        #endregion

        #endregion
    }
}
