// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Users.Interfaces;
using Quarrel.Client;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System;

namespace Quarrel.Bindables.Users
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Users.SelfUser"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableSelfUser : BindableItem, IBindableUser
    {
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(User))]
        [AlsoNotifyChangeFor(nameof(AvatarUrl))]
        [AlsoNotifyChangeFor(nameof(AvatarUri))]
        private SelfUser _selfUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableSelfUser"/> class.
        /// </summary>
        internal BindableSelfUser(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            SelfUser selfUser) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
            _selfUser = selfUser;
        }

        /// <inheritdoc/>
        public User User => SelfUser;
        
        /// <inheritdoc/>
        public string? AvatarUrl => User.GetAvatarUrl(128);
        
        /// <inheritdoc/>
        public Uri? AvatarUri => AvatarUrl is null ? null : new(AvatarUrl);
    }
}
