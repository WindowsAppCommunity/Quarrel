// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Friends;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// User Profile page data.
    /// </summary>
    public class UserProfilePageViewModel : ViewModelBase
    {
        private bool _isLoadingProfile;
        private IEnumerable<BindableMutualGuild> _mutualGuilds;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfilePageViewModel"/> class.
        /// </summary>
        /// <param name="user">The user to load.</param>
        public UserProfilePageViewModel(BindableGuildMember user)
        {
            User = user;
            LoadProfile();
        }

        /// <summary>
        /// Gets user as BindableGuildMember.
        /// </summary>
        public BindableGuildMember User { get; private set; }

        /// <summary>
        /// Gets or sets the user as Profile.
        /// </summary>
        public UserProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets bindable forms of Shared Guilds with User.
        /// </summary>
        public IEnumerable<BindableMutualGuild> MutualGuilds
        {
            get => _mutualGuilds;
            set => Set(ref _mutualGuilds, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether pr not the profile is loading.
        /// </summary>
        public bool IsLoadingProfile
        {
            get => _isLoadingProfile;
            set => Set(ref _isLoadingProfile, value);
        }

        private IDiscordService DiscordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();

        private ICacheService CacheService { get; } = SimpleIoc.Default.GetInstance<ICacheService>();

        /// <summary>
        /// Load UserProfile from User information.
        /// </summary>
        public async void LoadProfile()
        {
            // Shows loading indicator
            IsLoadingProfile = true;

            // Make sure user isn't a bot
            if (!User.Model.User.Bot)
            {
                try
                {
                    Profile = await DiscordService.UserService.GetUserProfile(User.Model.User.Id);
                }
                catch
                {
                    Profile = new UserProfile() { user = User.Model.User };
                }
            }
            else
            {
                Profile = new UserProfile() { user = User.Model.User };
            }

            if (User.Model.User.Id == DiscordService.CurrentUser.Id)
            {
                Profile.Friend = new Friend()
                {
                    Type = -1,
                    User = DiscordService.CurrentUser,
                    Id = DiscordService.CurrentUser.Id,
                };
            }
            else
            {
                // Check friend status
                if (SimpleIoc.Default.GetInstance<IFriendsService>().Friends.TryGetValue(Profile.user.Id, out var bindableFriend))
                {
                    Profile.Friend = bindableFriend.Model;
                }
                else
                {
                    Profile.Friend = new Friend()
                    {
                        Type = 0,
                        Id = User.Model.User.Id,
                        User = User.Model.User,
                    };
                }
            }

            RaisePropertyChanged(nameof(Profile));

            // Show shared friends (if not a bot)
            if (!User.Model.User.Bot)
            {
                Profile.SharedFriends = await DiscordService.UserService.GetUserReleations(User.Model.User.Id);
            }

            MutualGuilds = Profile.MutualGuilds?.Select(x => new BindableMutualGuild(x));

            // Hides loading indicator
            IsLoadingProfile = false;
        }
    }
}
