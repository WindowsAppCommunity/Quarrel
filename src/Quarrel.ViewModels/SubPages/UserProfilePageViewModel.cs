using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Rest;
using Quarrel.ViewModels.Services.Users;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.SubPages
{
    public class UserProfilePageViewModel : ViewModelBase
    {

        #region Constructors

        public UserProfilePageViewModel(BindableGuildMember user)
        {
            User = user;
            LoadProfile();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load UserProfile from User information
        /// </summary>
        public async void LoadProfile()
        {
            // Make sure user isn't a bot
            if (!User.Model.User.Bot)
                Profile = await discordService.UserService.GetUserProfile(User.Model.User.Id);
            else
                Profile = new UserProfile() { user = User.Model.User };

            // Check friend status
            if (SimpleIoc.Default.GetInstance<ICurrentUsersService>().Friends.TryGetValue(Profile.user.Id, out var bindableFriend))
                Profile.Friend = bindableFriend.Model;
            else
                Profile.Friend = new Friend() { Type = 0, Id = User.Model.User.Id, User = User.Model.User };

            // Show shared friends (if not a bot)
            if (!User.Model.User.Bot)
                Profile.SharedFriends = await discordService.UserService.GetUserReleations(User.Model.User.Id);

            MutualGuilds = Profile.MutualGuilds.Select(x => new BindableMutualGuild(x));
        }

        #endregion

        #region Properties

        #region Services

        private IDiscordService discordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ICacheService cacheService { get; } = SimpleIoc.Default.GetInstance<ICacheService>();
        
        #endregion

        /// <summary>
        /// User as BindableGuildMember
        /// </summary>
        public BindableGuildMember User { get; private set; }

        /// <summary>
        /// User as Profile
        /// </summary>
        public UserProfile Profile;

        /// <summary>
        /// Bindable form of Shared Guilds with User
        /// </summary>
        private IEnumerable<BindableMutualGuild> _MutualGuilds;
        public IEnumerable<BindableMutualGuild> MutualGuilds
        {
            get => _MutualGuilds;
            set => Set(ref _MutualGuilds, value);
        }

        #endregion
    }
}
