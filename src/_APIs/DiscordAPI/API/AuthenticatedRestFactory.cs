// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Misc;
using DiscordAPI.API.User;
using DiscordAPI.API.Voice;
using DiscordAPI.Authentication;
using Refit;
using System;
using System.Net.Http;

namespace DiscordAPI.API
{
    /// <summary>
    /// Creates factory for authenticated endpoints.
    /// </summary>
    public class AuthenticatedRestFactory
    {
        private readonly DiscordApiConfiguration _apiConfig;
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedRestFactory"/> class.
        /// </summary>
        /// <param name="config">The API configuration.</param>
        /// <param name="authenticator">The authentication info for the services.</param>
        public AuthenticatedRestFactory(DiscordApiConfiguration config, IAuthenticator authenticator)
        {
            _apiConfig = config;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Get the connection service.
        /// </summary>
        /// <returns>An autheneticated connection service.</returns>
        public IConnectionsService GetConnectionService()
        {
            return RestService.For<IConnectionsService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the activites service.
        /// </summary>
        /// <returns>An autheneticated activites service.</returns>
        public IActivitiesService GetActivitesService()
        {
            return RestService.For<IActivitiesService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the game service.
        /// </summary>
        /// <returns>An autheneticated game service.</returns>
        public IGameService GetGameService()
        {
            return RestService.For<IGameService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the user service.
        /// </summary>
        /// <returns>An autheneticated user service.</returns>
        public IUserService GetUserService()
        {
            return RestService.For<IUserService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the misc service.
        /// </summary>
        /// <returns>An autheneticated misc service.</returns>
        public IMiscService GetMiscService()
        {
            return RestService.For<IMiscService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the channel service.
        /// </summary>
        /// <returns>An autheneticated channel service.</returns>
        public IChannelService GetChannelService()
        {
            return RestService.For<IChannelService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the guild service.
        /// </summary>
        /// <returns>An autheneticated guild service.</returns>
        public IGuildService GetGuildService()
        {
            return RestService.For<IGuildService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the invite service.
        /// </summary>
        /// <returns>An autheneticated invite service.</returns>
        public IInviteService GetInviteService()
        {
            return RestService.For<IInviteService>(GetAuthenticatingHttpClient());
        }

        /// <summary>
        /// Get the voice service.
        /// </summary>
        /// <returns>An autheneticated voice service.</returns>
        public IVoiceService GetVoiceService()
        {
            return RestService.For<IVoiceService>(GetAuthenticatingHttpClient());
        }

        private HttpClient GetAuthenticatingHttpClient()
        {
            return new HttpClient(GetAuthenticationHandler())
            {
                BaseAddress = new Uri(_apiConfig.BaseUrl),
            };
        }

        private HttpClientHandler GetAuthenticationHandler()
        {
            return new AuthenticatingHttpClientHandler(_authenticator);
        }
    }
}
