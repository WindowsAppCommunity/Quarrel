// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.HttpHandlers;
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

        internal IChannelService GetChannelService()
        {
            return RestService.For<IChannelService>(GetHttpClient());
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
