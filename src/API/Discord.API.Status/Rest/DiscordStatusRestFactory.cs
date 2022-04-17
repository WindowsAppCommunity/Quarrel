// Quarrel © 2022

using Refit;
using System;
using System.Net.Http;

namespace Discord.API.Status.Rest
{
    public class DiscordStatusRestFactory
    {
        private const string BaseUrl = "https://discord.statuspage.io/";

        public DiscordStatusRestFactory()
        {
        }

        public IStatusService GetStatusService()
        {
            return RestService.For<IStatusService>(GetHttpClient());
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }
    }
}
