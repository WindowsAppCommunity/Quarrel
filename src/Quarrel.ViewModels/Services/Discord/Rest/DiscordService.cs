// Copyright (c) Quarrel. All rights reserved.

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
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Navigation;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Discord.Rest
{
    /// <summary>
    /// The default <see langword="interface"/> for the a service that executes REST calls to Discord.
    /// </summary>
    public class DiscordService : IDiscordService
    {
        private readonly ICacheService _cacheService;
        private readonly IGatewayService _gatewayService;
        private readonly ISubFrameNavigationService _subFrameNavigationService;

        /// <summary>
        /// The access token for the current user.
        /// </summary>
        [NotNull]
        private string _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordService"/> class.
        /// </summary>
        /// <param name="cacheService">The app's cache service.</param>
        /// <param name="gatewayService">The app's gateway service.</param>
        /// <param name="subFrameNavigationService">The app's subframe navigation service.</param>
        public DiscordService(
            ICacheService cacheService,
            IGatewayService gatewayService,
            ISubFrameNavigationService subFrameNavigationService)
        {
            _cacheService = cacheService;
            _gatewayService = gatewayService;
            _subFrameNavigationService = subFrameNavigationService;
        }

        /// <inheritdoc/>
        public IActivitiesService ActivitesService { get; private set; }

        /// <inheritdoc/>
        public IChannelService ChannelService { get; private set; }

        /// <inheritdoc/>
        public IConnectionsService ConnectionsService { get; private set; }

        /// <inheritdoc/>
        public IGameService GameService { get; private set; }

        /// <inheritdoc/>
        public IGatewayConfigService GatewayConfigService { get; private set; }

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
        public User CurrentUser { get; set; }

        /// <inheritdoc/>
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

            _accessToken = result.Token;

            await _cacheService.Persistent.Roaming.SetValueAsync(Constants.Cache.Keys.AccessToken, (object)_accessToken);

            return await Login();
        }

        /// <inheritdoc/>
        public async Task<bool> Login([NotNull] string token, bool storeToken = false)
        {
            if (storeToken)
            {
                await _cacheService.Persistent.Roaming.SetValueAsync(Constants.Cache.Keys.AccessToken, (object)token);
            }

            _accessToken = token;

            return await Login();
        }

        /// <inheritdoc/>
        public void Logout()
        {
            _cacheService.Persistent.Roaming.DeleteValueAsync(Constants.Cache.Keys.AccessToken);
            _subFrameNavigationService.NavigateTo("LoginPage");
        }

        private Task<bool> Login()
        {
            Messenger.Default.Send(new ConnectionStatusMessage(ConnectionStatus.Connecting));

            IAuthenticator authenticator = new DiscordAuthenticator(_accessToken);
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

            return _gatewayService.InitializeGateway(_accessToken);
        }
    }
}
