// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Login;
using DiscordAPI.API.Misc;
using DiscordAPI.API.User;
using DiscordAPI.API.Voice;
using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Services.Gateway;
using DiscordAPI.API;
using DiscordAPI.Authentication;
using Windows.Networking.Connectivity;

namespace Quarrel.Services.Rest
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
        public IGatewayService Gateway { get; private set; }

        /// <inheritdoc/>
        public User CurrentUser { get; private set; }

        // The access token for the current user
        [NotNull]
        private string _AccessToken;

        #endregion

        #region Login

        public async void Login([NotNull] string email, [NotNull] string password)
        {
            BasicRestFactory restFactory = new BasicRestFactory();
            LoginService = restFactory.GetLoginService();

            var result = await LoginService.Login(new DiscordAPI.API.Login.Models.LoginRequest() { Email = email, Password = password });

            _AccessToken = result.Token;

            Gateway.InitializeGateway(_AccessToken);

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

            
        }

        #endregion
    }
}
