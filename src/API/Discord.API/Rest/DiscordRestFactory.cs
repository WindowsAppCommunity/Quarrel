using Discord.API.HttpHandlers;
using System;
using System.Net.Http;

namespace Discord.API.Rest
{
    public class DiscordRestFactory
    {
        private const string BaseUrl = "https://discordapp.com/api";

        public string Token { get; set; }

        private HttpClient GetHttpClient(bool authenticated = true)
        {
            HttpClientHandler handler = new HttpClientHandler();

            if (authenticated)
            {
                handler = new AuthenticatedHttpClientHandler(Token);
            }

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }
    }
}
