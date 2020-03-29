// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Rest;

namespace Quarrel.ViewModels.SubPages.UserSettings.Pages
{
    /// <summary>
    /// Privacy settings page data.
    /// </summary>
    public class PrivacySettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the filter setting is set to all.
        /// </summary>
        public bool FilterAll
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 2;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 2;
                ApplyChanges(modify);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter setting is set to public.
        /// </summary>
        public bool PublicFilter
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 1;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 1;
                ApplyChanges(modify);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the filter setting is set to none.
        /// </summary>
        public bool NoFilter
        {
            get => CurrentUsersService.CurrentUserSettings.ExplicitContentFilter == 0;
            set
            {
                // Being set to false, don't update
                if (!value)
                {
                    return;
                }

                ModifyUserSettings modify = new ModifyUserSettings(CurrentUsersService.CurrentUserSettings);
                modify.ExplicitContentFilter = 0;
                ApplyChanges(modify);
            }
        }

        private ICurrentUserService CurrentUsersService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();

        /// <summary>
        /// Saves pending changes to the user.
        /// </summary>
        /// <param name="modify">User modify settings.</param>
        public async void ApplyChanges(ModifyUserSettings modify)
        {
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings(modify);
        }
    }
}
