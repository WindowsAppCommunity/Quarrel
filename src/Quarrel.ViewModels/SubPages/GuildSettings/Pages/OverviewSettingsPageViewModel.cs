using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    public class OverviewSettingsPageViewModel : ViewModelBase
    {
        #region Constructor

        public OverviewSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;
        }

        #endregion

        #region Methods

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
            // Mainly for Rate Limit
            catch { }
        }

        #endregion

        #region Commands

        private RelayCommand deleteIcon;
        public RelayCommand DeleteIcon => deleteIcon = new RelayCommand(async () =>
        {
            ModifyGuildIcon modify = new ModifyGuildIcon(Guild.Model) { Icon = null };
            await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.ModifyGuild(Guild.Model.Id, modify);

            Guild.Model.Icon = null;
            Guild.RaisePropertyChanged(nameof(Guild.HasIcon));
            Guild.RaisePropertyChanged(nameof(Guild.IconUrl));
        });

        #endregion

        #region Properties

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }
        private BindableGuild _Guild;

        public string Name
        {
            get => Guild.Model.Name;
            set
            {
                SimpleIoc.Default.GetInstance<IDiscordService>()
                    .GuildService.ModifyGuild(Guild.Model.Id, new ModifyGuild(Guild.Model) { Name = value });
                Guild.Model.Name = value;
                Guild.RaisePropertyChanged(nameof(Guild.Model));
            }
        }

        #endregion
    }
}
