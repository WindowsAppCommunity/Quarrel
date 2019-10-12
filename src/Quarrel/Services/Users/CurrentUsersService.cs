using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;
using Quarrel.ViewModels.Services;

namespace Quarrel.Services.Users
{

    public class CurrentUsersService : ICurrentUsersService
    {
        public ICacheService CacheService;

        public ConcurrentDictionary<string, BindableGuildMember> Users { get; } = 
            new ConcurrentDictionary<string, BindableGuildMember>();

        public ConcurrentDictionary<string, BindableGuildMember> DMUsers { get; } = 
            new ConcurrentDictionary<string, BindableGuildMember>();

        public BindableUser CurrentUser { get; } = new BindableUser(new User());

        public BindableGuildMember CurrentGuildMember => 
            Users.TryGetValue(CurrentUser.Model.Id, out var member) ? member : null;
        

        public string SessionId { get; set; }

        public CurrentUsersService(ICacheService cacheService)
        {
            CacheService = cacheService;
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
                });
            });
            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (CurrentUser.Model.Id == m.UserId)
                    {
                        CurrentUser.Presence = m.Presence;
                    }

                    if(Users.TryGetValue(m.UserId, out BindableGuildMember member))
                    {
                        member.Presence = m.Presence;
                    }
                });
            });
            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, async m =>
            {
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
        }
    }
}
