// Quarrel © 2022

using Refit;
using System;
using System.Net.Http;

namespace Discord.API.Status.Rest
{
    /// <summary>
    /// A class for initializing Discord Status rest services.
    /// </summary>
    public class DiscordStatusRestFactory
    {
        private const string BaseUrl = "https://discord.statuspage.io/";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordStatusRestFactory"/> class.
        /// </summary>
        public DiscordStatusRestFactory()
        {
        }
        
        /// <summary>
        /// Gets an instance of the <see cref="IStatusService"/>.
        /// </summary>
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
