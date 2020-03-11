// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Friend"/> model.
    /// </summary>
    public class BindableFriend : BindableModelBase<Friend>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableFriend"/> class.
        /// </summary>
        /// <param name="friend">The base friend object.</param>
        public BindableFriend([NotNull] Friend friend) : base(friend)
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not the user has no friend status.
        /// </summary>
        public bool NotFriend => Model.Type == 0;

        /// <summary>
        /// Gets a value indicating whether or not the user is a friend.
        /// </summary>
        public bool IsFriend => Model.Type == 1;

        /// <summary>
        /// Gets a value indicating whether or not the user is blocked.
        /// </summary>
        public bool IsBlocked => Model.Type == 2;

        /// <summary>
        /// Gets a value indicating whether or not the user has an incoming friend request.
        /// </summary>
        public bool IsIncoming => Model.Type == 3;

        /// <summary>
        /// Gets a value indicating whether or not the user has an outgoing friend request.
        /// </summary>
        public bool IsOutgoing => Model.Type == 4;
    }
}
