using Refit;
using System;
using System.Net.Http;
using Quarrel.API.Gateway;
using Quarrel.API.Login;

namespace Quarrel.API
{
    public class BasicRestFactory
    {
        private readonly DiscordApiConfiguration _apiConfig;

        public BasicRestFactory(DiscordApiConfiguration config)
        {
            _apiConfig = config;
        }

        public IGatewayConfigService GetGatewayConfigService()
        {
            return RestService.For<IGatewayConfigService>(GetHttpClient());
        }

        public ILoginService GetLoginService()
        {
            return RestService.For<ILoginService>(GetHttpClient());
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
