// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// Loads and stores AuditLog page data.
    /// </summary>
    public class AuditLogSettingsPageViewModel : ViewModelBase
    {
        private BindableGuild _guild;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogSettingsPageViewModel"/> class.
        /// </summary>
        /// <param name="guild">The guild to view the auditlog of.</param>
        public AuditLogSettingsPageViewModel(BindableGuild guild)
        {
            Guild = guild;

            LoadAuditLog();
        }

        /// <summary>
        /// Gets or sets the guild whos audit log is being viewed.
        /// </summary>
        public BindableGuild Guild
        {
            get => _guild;
            set => Set(ref _guild, value);
        }

        /// <summary>
        /// Gets the collection of entries listed in the auditlog.
        /// </summary>
        public ObservableCollection<AuditLogEntry> Entries { get; private set; } = new ObservableCollection<AuditLogEntry>();

        /// <summary>
        /// Gets the list of users mentioned in the AuditLog.
        /// </summary>
        public ObservableCollection<User> Users { get; private set; } = new ObservableCollection<User>();

        private async void LoadAuditLog()
        {
            AuditLog log = await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.GetAuditLog(Guild.Model.Id);

            foreach (var user in log.Users)
            {
                var member = SimpleIoc.Default.GetInstance<IGuildsService>().GetGuildMember(user.Id, Guild.Model.Id);
                if (member != null)
                {
                    Users.Add(member.Model.User);
                }
                else
                {
                    Users.Add(new User()
                    {
                        Id = user.Id,
                        Avatar = user.Avatar,
                        Username = user.Username,
                        Discriminator = user.Discriminator,
                        Bot = user.Bot.GetValueOrDefault(false),
                    });
                }
            }

            foreach (AuditLogEntry entry in log.AuditLogEntries)
            {
                entry.Users = Users;
                Entries.Add(entry);
            }
        }
    }
}
