﻿using System;
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
using Quarrel.Messages.Posts.Requests;
using Quarrel.Models.Bindables;
using Quarrel.Services.Cache;

namespace Quarrel.Services.Users
{

    public class CurrentUsersService : ICurrentUsersService
    {
        public ICacheService CacheService;

        public Dictionary<string, BindableGuildMember> Users { get; } = new Dictionary<string, BindableGuildMember>();

        public BindableUser CurrentUser { get; } = new BindableUser(new User());
        public string SessionId { get; set; }

        public CurrentUsersService(ICacheService cacheService)
        {
            CacheService = cacheService;
            Messenger.Default.Register<GatewayGuildSyncMessage>(this, m =>
            {

                // Show members
                Users.Clear();
                foreach (var member in m.Members)
                {
                    BindableGuildMember bGuildMember = new BindableGuildMember(member)
                    {
                        GuildId = m.GuildId,
                        Presence = Messenger.Default.Request<PresenceRequestMessage, Presence>(new PresenceRequestMessage(member.User.Id))
                    };
                    Users.Add(member.User.Id, bGuildMember);
                }
                Messenger.Default.Send("UsersSynced");

            });
            Messenger.Default.Register<GatewayReadyMessage>(this, async m =>
            {
                SessionId = m.EventData.SessionId;
                await DispatcherHelper.RunAsync(() => {
                    CurrentUser.Model = m.EventData.User;
                    CurrentUser.Presence = m.EventData.Presences.LastOrDefault();
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

                    Users.TryGetValue(m.UserId, out BindableGuildMember member);
                    if(member != null)
                    {
                        member.Presence = m.Presence;
                    }
                });
            });

            Messenger.Default.Register<GatewayUserSettingsUpdatedMessage>(this, async m =>
            {
                if (string.IsNullOrEmpty(m.Settings.Status))
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

                        Users.TryGetValue(CurrentUser.Model.Id, out BindableGuildMember member);
                        if (member != null)
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
                    var session = m.Session.FirstOrDefault(x => x.SessionId == SessionId);
                    var newPresence = new Presence()
                    {
                        User = CurrentUser.Presence.User,
                        Game = session.Game,
                        GuildId = CurrentUser.Presence.GuildId,
                        Roles = CurrentUser.Presence.Roles,
                        Status = session.Status
                    };
                    CurrentUser.Presence = newPresence;

                    Users.TryGetValue(CurrentUser.Model.Id, out BindableGuildMember member);
                    if (member != null)
                    {
                        member.Presence = newPresence;
                    }
                });
            });

        }
    }
}