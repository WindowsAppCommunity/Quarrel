using DiscordAPI.API.User.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Settings.Pages
{
    public class PrivacySettingsViewModel : ViewModelBase
    {
        public ICurrentUsersService CurrentUsersService
        {
            get => SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        }

        public bool FilterAll
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 2;
            set
            {
                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 2;
                ApplyChanges(modify);
            }
        }

        public bool PublicFilter
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 1;
            set
            {
                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 1;
                ApplyChanges(modify);
            }
        }

        public bool NoFilter
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 0;
            set
            {
                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 0;
                ApplyChanges(modify);
            }
        }

        public async void ApplyChanges(ModifyUserSettings modify)
        {
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings(modify);
        }
    }
}
