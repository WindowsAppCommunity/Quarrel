// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Friends
{
    /// <summary>
    /// Manages all relationship status with the current user.
    /// </summary>
    public class FriendsService : IFriendsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendsService"/> class.
        /// </summary>
        public FriendsService()
        {
            Messenger.Default.Register<GatewayReadyMessage>(this, m =>
            {
                foreach (var channel in m.EventData.PrivateChannels)
                {
                    foreach (var user in channel.Users)
                    {
                        if (!DMUsers.ContainsKey(user.Id))
                        {
                            DMUsers.Add(user.Id, new BindableGuildMember(new GuildMember() { User = user }, "DM"));
                        }
                    }
                }
            });
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, BindableFriend> Friends { get; } =
            new ConcurrentDictionary<string, BindableFriend>();

        /// <inheritdoc/>
        public IDictionary<string, BindableGuildMember> DMUsers { get; } =
            new ConcurrentDictionary<string, BindableGuildMember>();
    }
}
