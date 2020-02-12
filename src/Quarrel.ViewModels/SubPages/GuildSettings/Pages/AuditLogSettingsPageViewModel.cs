using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Rest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    public class AuditLogSettingsPageViewModel : ViewModelBase
    {
        public AuditLogSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;

            LoadAuditLog();
        }

        #region Methods

        public async void LoadAuditLog()
        {
            AuditLog log = await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.GetAuditLog(Guild.Model.Id);

            foreach (AuditLogEntry entry in log.AuditLogEntries)
            {
                Entries.Add(entry);
            }
        }

        #endregion

        #region Properties

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }
        private BindableGuild _Guild;

        public ObservableCollection<AuditLogEntry> Entries { get; set; } = new ObservableCollection<AuditLogEntry>();

        #endregion
    }
}
