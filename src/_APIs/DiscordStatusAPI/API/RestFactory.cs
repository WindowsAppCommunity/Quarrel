using DiscordStatusAPI.API.Status;
using Refit;
using System;
using System.Net.Http;

namespace DiscordStatusAPI.API
{
    public class RestFactory
    {
        private readonly StatusAPIConfiguration _apiConfig;

        public RestFactory()
        {
            _apiConfig = new StatusAPIConfiguration();
        }

        public RestFactory(StatusAPIConfiguration config)
        {
            _apiConfig = config;
        }

        public IStatusService GetStatusService()
        {
            return RestService.For<IStatusService>(GetHttpClient());
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri(_apiConfig.BaseUrl)
            };
        }
    }
}
