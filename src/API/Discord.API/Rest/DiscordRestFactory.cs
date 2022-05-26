// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.HttpHandlers;
using Discord.API.JsonConverters;
using Refit;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Discord.API.Rest
{
    /// <summary>
    /// A class for initializing Discord rest services.
    /// </summary>
    internal class DiscordRestFactory
    {
        private const string BaseUrl = "https://discordapp.com/api";

        public string? Token { get; set; }

        private readonly RefitSettings _settings;

        public DiscordRestFactory()
        {
            var options = new JsonSerializerOptions();
            options.AddContext<JsonModelsContext>();
            _settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };
        }
        
        /// <summary>
        /// Gets an instance of the <see cref="IChannelService"/>.
        /// </summary>
        internal IChannelService GetChannelService()
        {
            return RestService.For<IChannelService>(GetHttpClient(), _settings);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IGatewayService"/>.
        /// </summary>
        internal IGatewayService GetGatewayService()
        {
            return RestService.For<IGatewayService>(GetHttpClient(), _settings);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IGuildService"/>.
        /// </summary>
        internal IGuildService GetGuildService()
        {
            return RestService.For<IGuildService>(GetHttpClient(), _settings);
        }

        /// <summary>
        /// Gets an instance of the <see cref="IUserService"/>.
        /// </summary>
        internal IUserService GetUserService()
        {
            return RestService.For<IUserService>(GetHttpClient(), _settings);
        }

        private HttpClient GetHttpClient(bool authenticated = true)
        {
            var handler = new HttpClientHandler();

            if (authenticated)
            {
                Guard.IsNotNull(Token, nameof(Token));
                handler = new AuthenticatedHttpClientHandler(Token);
            }

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }
    }
}
