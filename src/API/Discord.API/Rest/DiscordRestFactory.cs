using CommunityToolkit.Diagnostics;
using Discord.API.HttpHandlers;
using Discord.API.Rest.Gateway;
using Refit;
using System;
using System.Net.Http;

namespace Discord.API.Rest
{
    internal class DiscordRestFactory
    {
        private const string BaseUrl = "https://discordapp.com/api";

        public string? Token { get; set; }

        internal IGatewayService GetGatewayService()
        {
            return RestService.For<IGatewayService>(GetHttpClient());
        }

        private HttpClient GetHttpClient(bool authenticated = true)
        {
            HttpClientHandler handler = new HttpClientHandler();

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
