// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.ObjectModel;
using System.Linq;
using Quarrel.ViewModels.Services.Discord.Channels;

namespace Quarrel.ViewModels.SubPages.GuildSettings.Pages
{
    /// <summary>
    /// AuditLog settings page data.
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
        /// Gets or sets the guild being viewed.
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

        public IGuildsService GuildsService { get; set; } = SimpleIoc.Default.GetInstance<IGuildsService>();
        public IChannelsService ChannelsService { get; set; } = SimpleIoc.Default.GetInstance<IChannelsService>();

        private async void LoadAuditLog()
        {
            AuditLog log = await SimpleIoc.Default.GetInstance<IDiscordService>().GuildService.GetAuditLog(Guild.Model.Id);

            foreach (var user in log.Users)
            {
                var member = GuildsService.GetGuildMember(user.Id, Guild.Model.Id);
                if (Users.Any(x => x.Id == user.Id)) continue;
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

            var users = Users.ToDictionary(x => x.Id, x =>
                (x.Username, GuildsService.GetGuildMember(x.Id, GuildsService.GetGuild(Guild.Model.Id).Model.Id)?.TopRole?.Color ?? 0x18363));

            var roles = GuildsService.GetGuild(Guild.Model.Id).Model.Roles.ToDictionary(x => x.Id, x => (x.Name, x.Color));

            var channels = GuildsService.GetGuild(Guild.Model.Id).Channels.ToDictionary(x => x.Model.Id, x => x.Model.Name);

            foreach (AuditLogEntry entry in log.AuditLogEntries)
            {
                entry.Users = users;
                entry.Channels = channels;
                entry.Roles = roles;
                Entries.Add(entry);
            }
        }
    }
}
