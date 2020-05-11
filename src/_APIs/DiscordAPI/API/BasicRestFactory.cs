// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Gateway;
using DiscordAPI.API.Login;
using Refit;
using System;
using System.Net.Http;

namespace DiscordAPI.API
{
    /// <summary>
    /// Creates factory for unauthenticated endpoints.
    /// </summary>
    public class BasicRestFactory
    {
        private readonly DiscordApiConfiguration _apiConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRestFactory"/> class.
        /// </summary>
        public BasicRestFactory()
        {
            _apiConfig = new DiscordApiConfiguration() { BaseUrl = "https://discordapp.com/api" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRestFactory"/> class.
        /// </summary>
        /// <param name="config">Api configuration.</param>
        public BasicRestFactory(DiscordApiConfiguration config)
        {
            _apiConfig = config;
        }

        /// <summary>
        /// Get the gateway config service.
        /// </summary>
        /// <returns>An autheneticated gateway config service.</returns>
        public IGatewayConfigService GetGatewayConfigService()
        {
            return RestService.For<IGatewayConfigService>(GetHttpClient());
        }

        /// <summary>
        /// Get the login service.
        /// </summary>
        /// <returns>An autheneticated login service.</returns>
        public ILoginService GetLoginService()
        {
            return RestService.For<ILoginService>(GetHttpClient());
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri(_apiConfig.BaseUrl),
            };
        }
    }
}
