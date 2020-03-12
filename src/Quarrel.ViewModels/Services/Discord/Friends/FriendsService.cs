// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
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
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    foreach (var presence in m.EventData.Presences)
                    {
                        DMUsers.Add(presence.User.Id, new BindableGuildMember(new GuildMember() { User = presence.User }, "DM", presence));
                    }
                });
            });
        }

        /// <inheritdoc/>
        public IDictionary<string, BindableFriend> Friends { get; } =
            new ConcurrentDictionary<string, BindableFriend>();

        /// <inheritdoc/>
        public IDictionary<string, BindableGuildMember> DMUsers { get; } =
            new ConcurrentDictionary<string, BindableGuildMember>();

        private IDispatcherHelper DispatcherHelper => SimpleIoc.Default.GetInstance<IDispatcherHelper>();
    }
}
