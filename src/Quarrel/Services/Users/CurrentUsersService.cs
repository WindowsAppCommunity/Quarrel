using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.UpstreamEvents;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Gateway.Voice;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services;

namespace Quarrel.Services.Users
{

    public class CurrentUsersService : ICurrentUsersService
    {
        public ICacheService CacheService;
        public IGuildsService GuildsService;

        public ConcurrentDictionary<string, BindableGuildMember> Users { get; } = 
            new ConcurrentDictionary<string, BindableGuildMember>();

        public ConcurrentDictionary<string, BindableGuildMember> DMUsers { get; } = 
            new ConcurrentDictionary<string, BindableGuildMember>();
        public ConcurrentDictionary<string, BindableFriend> Friends { get; } =
            new ConcurrentDictionary<string, BindableFriend>();
        public ConcurrentDictionary<string, GuildSetting> GuildSettings { get; } = 
            new ConcurrentDictionary<string, GuildSetting>();
        public ConcurrentDictionary<string, ChannelOverride> ChannelSettings { get; } = 
            new ConcurrentDictionary<string, ChannelOverride>();

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, GuildMember>> guildUsers =  
            new ConcurrentDictionary<string, ConcurrentDictionary<string, GuildMember>>();

        public BindableUser CurrentUser { get; } = new BindableUser(new User());

        public UserSettings CurrentUserSettings { get; private set; } = new UserSettings();

        public BindableGuildMember CurrentGuildMember => 
            Users.TryGetValue(CurrentUser.Model.Id, out var member) ? member : null;
        

        public string SessionId { get; set; }

        public CurrentUsersService(ICacheService cacheService, IGuildsService guildsService)
        {
            CacheService = cacheService;
            GuildsService = guildsService;

            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (var guild in m.EventData.Guilds)
                    {
                        guildUsers.TryAdd(guild.Id, new ConcurrentDictionary<string, GuildMember>());
                        foreach (var member in guild.Members)
                        {
                            guildUsers[guild.Id].TryAdd(member.User.Id, member);
                        }
                    }
                });
            });

            Messenger.Default.Register<GatewayGuildSyncMessage>(this, m =>
            {
                // Show members
                Users.Clear();
                List<BindableGuildMember> UsersList = new List<BindableGuildMember>();
                foreach (var member in m.Members)
                {
                    BindableGuildMember bGuildMember = new BindableGuildMember(member)
                    {
                        GuildId = m.GuildId,
                        IsOwner = member.User.Id == GuildsService.Guilds[m.GuildId].Model.OwnerId,
                        Presence = Messenger.Default.Request<PresenceRequestMessage, Presence>(new PresenceRequestMessage(member.User.Id))
                    };
                    Users.TryAdd(member.User.Id, bGuildMember);
                    UsersList.Add(bGuildMember);

                }
                Messenger.Default.Send(new GuildMembersSyncedMessage(UsersList));
            });
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                SessionId = m.EventData.SessionId;
                await DispatcherHelper.RunAsync(() => {
                    CurrentUser.Model = m.EventData.User;
                    CurrentUser.Presence = new Presence()
                    {
                        User = null,
                        Game = null,
                        GuildId = null,
                        Roles = null,
                        Status = m.EventData.Settings.Status
                    };

                    DMUsers.TryAdd(CurrentUser.Model.Id, new BindableGuildMember(new GuildMember() { User = CurrentUser.Model }) { Presence = CurrentUser.Presence, GuildId = "DM" });
                    foreach (var presence in m.EventData.Presences)
                    {
                        DMUsers.TryAdd(presence.User.Id, new BindableGuildMember(new GuildMember() { User = presence.User }) { Presence = presence, GuildId = "DM" });
                    }

                    CurrentUserSettings = m.EventData.Settings;
                });
            });
            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, m =>
            {
                _ = DispatcherHelper.RunAsync(() =>
                  {
                      if (CurrentUser.Model.Id == m.UserId)
                      {
                          CurrentUser.Presence = m.Presence;
                      }

                      if (Users.TryGetValue(m.UserId, out BindableGuildMember member))
                      {
                          member.Presence = m.Presence;
                      }
                  });
            });
            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, async m =>
            {
                CurrentUserSettings = m.Settings;

                if (!string.IsNullOrEmpty(m.Settings.Status))
                {
                    var newPresence = new Presence()
                    {
                        User = CurrentUser.Presence.User,
                        Game = CurrentUser.Presence.Game,
                        GuildId = CurrentUser.Presence.GuildId,
                        Roles = CurrentUser.Presence.Roles,
                        Status = m.Settings.Status
                    };
                    await DispatcherHelper.RunAsync(() =>
                    {

                        CurrentUser.Presence = newPresence;

                        if (Users.TryGetValue(CurrentUser.Model.Id, out var member))
                        {
                            member.Presence = newPresence;
                        }
                    });
                }
            });
            Messenger.Default.Register<GatewaySessionReplacedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                });
            });
            Messenger.Default.Register<GuildNavigateMessage>(this, async m => 
            {
                if (m.Guild.Model.Id == "DM")
                {
                    await DispatcherHelper.RunAsync(() => 
                    {
                        Users.Clear();
                    });
                }
            });
            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, async m =>
            {
                guildUsers.TryGetValue(m.GuildMembersChunk.GuildId, out var guild);
                foreach (var member in m.GuildMembersChunk.Members)
                {
                    guild.TryAdd(member.User.Id, member);
                }
            });
        }

        public async Task<GuildMember> GetGuildMember(string memberId, string guildId)
        {
            if (guildUsers.TryGetValue(guildId, out var guild) && guild.TryGetValue(memberId, out GuildMember member))
            {
                return member;
            }
            else
            {
                return null;
            }
        }
        public IReadOnlyDictionary<string, GuildMember> GetAndRequestGuildMembers(IEnumerable<string> memberIds, string guildId)
        {
            Dictionary<string, GuildMember> guildMembers = new Dictionary<string, GuildMember>();
            List<string> guildMembersToBeRequested = new List<string>();
            if (guildUsers.TryGetValue(guildId, out var guild))
            {
                foreach (string memberId in memberIds)
                {
                    if (guild.TryGetValue(memberId, out GuildMember member))
                    {
                        guildMembers.Add(memberId, member);
                    }
                    else
                    {
                        guildMembersToBeRequested.Add(memberId);
                    }
                }

                if (guildMembersToBeRequested.Count > 0)
                {
                    Messenger.Default.Send(new GatewayRequestGuildMembersMessage(new List<string> {guildId}, guildMembersToBeRequested));
                }

                return guildMembers;
            }
            return null;
        }
    }
}
