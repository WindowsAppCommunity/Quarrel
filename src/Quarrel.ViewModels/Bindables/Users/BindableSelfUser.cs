// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Users.Interfaces;
using Quarrel.Client.Models.Users;
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
        internal BindableSelfUser(IDispatcherService dispatcherService, SelfUser selfUser) :
            base(dispatcherService)
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
