// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Discord.Rest;
using System;

namespace Quarrel.ViewModels.Models.Bindables.Users
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Friend"/> model.
    /// </summary>
    public class BindableFriend : BindableModelBase<Friend>, IBindableUser
    {
        private RelayCommand _acceptFriendRequestCommand;
        private RelayCommand _declineFriendRequestCommand;
        private IDiscordService _discordService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableFriend"/> class.
        /// </summary>
        /// <param name="friend">The base friend object.</param>
        public BindableFriend([NotNull] Friend friend) : base(friend)
        {
            Member = new BindableGuildMember(new GuildMember() { User = RawModel }, "DM");
        }

        /// <inheritdoc/>
        public User RawModel => Model.User;

        /// <summary>
        /// Gets the user as a <see cref="BindableGuildMember"/>.
        /// </summary>
        public BindableGuildMember Member { get; }

        /// <inheritdoc/>
        public Presence Presence => null;

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

        /// <summary>
        /// Gets a command to accept a friend request from the friend.
        /// </summary>
        public RelayCommand AcceptFriendRequestCommand => _acceptFriendRequestCommand = new RelayCommand(async () =>
        {
            SendFriendRequest friendRequest = new SendFriendRequest()
            {
                Username = Model.User.Username,
                Discriminator = Convert.ToInt32(Model.User.Discriminator),
            };
            await DiscordService.UserService.SendFriendRequest(friendRequest);
        });

        /// <summary>
        /// Gets a command to accept a friend request from the friend.
        /// </summary>
        public RelayCommand DeclineFriendRequestCommand => _declineFriendRequestCommand = new RelayCommand(async () =>
        {
            await DiscordService.UserService.RemoveFriend(Model.User.Id);
        });

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());
    }
}
