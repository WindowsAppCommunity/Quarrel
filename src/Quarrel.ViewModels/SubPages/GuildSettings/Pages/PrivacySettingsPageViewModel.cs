using DiscordAPI.API.User.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    public class PrivacySettingsPageViewModel : ViewModelBase
    {
        #region Constructors

        public PrivacySettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        #endregion

        #region Methods

        public async void ModifySettings(ModifyUserSettings modify)
        {
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings(modify);
        }

        #endregion

        #region Properties

        #region Services

        public ICurrentUserService CurrentUserService => SimpleIoc.Default.GetInstance<ICurrentUserService>();

        #endregion

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }
        private BindableGuild _Guild;

        public bool AllowDMs
        {
            get => !CurrentUserService.CurrentUserSettings.RestrictedGuilds.Contains(Guild.Model.Id);
            set
            {
                List<string> restrictedGuilds = CurrentUserService.CurrentUserSettings.RestrictedGuilds.ToList();

                if (restrictedGuilds.Contains(Guild.Model.Id))
                    restrictedGuilds.Remove(Guild.Model.Id);
                else
                    restrictedGuilds.Add(Guild.Model.Id);


                ModifyUserSettings modify = new ModifyUserSettings(CurrentUserService.CurrentUserSettings)
                {
                    RestrictedGuilds = restrictedGuilds.ToArray()
                };

                ModifySettings(modify);
            }
        }

        #endregion
    }
}
