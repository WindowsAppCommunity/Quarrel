// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.API;
using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Login;
using DiscordAPI.API.Login.Models;
using DiscordAPI.API.Misc;
using DiscordAPI.API.User;
using DiscordAPI.API.Voice;
using DiscordAPI.Authentication;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.Navigation;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Rest
{
    public class DiscordService : IDiscordService
    {
        #region Exposed services

        /// <inheritdoc/>
        public IActivitesService ActivitesService { get; private set; }

        /// <inheritdoc/>
        public IChannelService ChannelService { get; private set; }

        /// <inheritdoc/>
        public IConnectionsService ConnectionsService { get; private set; }

        /// <inheritdoc/>
        public IGameService GameService { get; private set; }

        /// <inheritdoc/>
        public IGatewayConfigService GatewayService { get; private set; }

        /// <inheritdoc/>
        public IGuildService GuildService { get; private set; }

        /// <inheritdoc/>
        public IInviteService InviteService { get; private set; }

        /// <inheritdoc/>
        public ILoginService LoginService { get; private set; }

        /// <inheritdoc/>
        public IMiscService MiscService { get; private set; }

        /// <inheritdoc/>
        public IUserService UserService { get; private set; }

        /// <inheritdoc/>
        public IVoiceService VoiceService { get; private set; }

        /// <inheritdoc/>
        [NotNull]
        public IGatewayService Gateway { get; private set; } = SimpleIoc.Default.GetInstance<IGatewayService>();

        /// <inheritdoc/>
        public User CurrentUser { get; set; }

        // The access token for the current user
        [NotNull]
        private string _AccessToken;

        #endregion

        #region Login
        private ICacheService CacheService;

        public DiscordService(ICacheService cacheService)
        {
            CacheService = cacheService;
        }

        public async Task<bool> Login([NotNull] string email, [NotNull] string password)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            LoginService = restFactory.GetLoginService();

            LoginResult result;
            try
            {
                result = await LoginService.Login(new LoginRequest() { Email = email, Password = password });
            }
            catch
            {
                return false;
            }

            _AccessToken = result.Token;

            await CacheService.Persistent.Roaming.SetValueAsync(Constants.Cache.Keys.AccessToken, (object)_AccessToken);

            return await Login();
        }

        public async Task<bool> Login([NotNull] string token, bool storeToken = false)
        {
            if (storeToken)
            {
                await CacheService.Persistent.Roaming.SetValueAsync(Constants.Cache.Keys.AccessToken, (object)token);
            }

            _AccessToken = token;

            return await Login();
        }

        private Task<bool> Login()
        {
            Messenger.Default.Send(new StartUpStatusMessage(Status.Connecting));

            IAuthenticator authenticator = new DiscordAuthenticator(_AccessToken);
            AuthenticatedRestFactory authenticatedRestFactory = new AuthenticatedRestFactory(new DiscordApiConfiguration() { BaseUrl = "https://discordapp.com/api" }, authenticator);

            ActivitesService = authenticatedRestFactory.GetActivitesService();
            ChannelService = authenticatedRestFactory.GetChannelService();
            ConnectionsService = authenticatedRestFactory.GetConnectionService();
            GameService = authenticatedRestFactory.GetGameService();
            GuildService = authenticatedRestFactory.GetGuildService();
            InviteService = authenticatedRestFactory.GetInviteService();
            MiscService = authenticatedRestFactory.GetMiscService();
            UserService = authenticatedRestFactory.GetUserService();
            VoiceService = authenticatedRestFactory.GetVoiceService();

            return Gateway.InitializeGateway(_AccessToken);
        }

        public void Logout()
        {
            CacheService.Persistent.Roaming.DeleteValueAsync(Constants.Cache.Keys.AccessToken);
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("LoginPage");
        }

        #endregion

        #region Channel



        #endregion
    }
}
