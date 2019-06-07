using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Quarrel.API.Activities;
using Quarrel.API.Channel;
using Quarrel.API.Connections;
using Quarrel.API.Game;
using Quarrel.API.Guild;
using Quarrel.API.Invite;
using Quarrel.API.Misc;
using Quarrel.API.User;
using Quarrel.API.Voice;
using Quarrel.Authentication;

namespace Quarrel.API
{
    public class AuthenticatedRestFactory
    {
        private readonly IAuthenticator _authenticator;
        private readonly DiscordApiConfiguration _apiConfig;
        
        public AuthenticatedRestFactory(DiscordApiConfiguration config, IAuthenticator authenticator)
        {
            _apiConfig = config;
            _authenticator = authenticator;
        }
        public IConnectionsService GetConnectionService()
        {
            return RestService.For<IConnectionsService>(GetAuthenticatingHttpClient());
        }

        public IActivitesService GetActivitesService()
        {
            return RestService.For<IActivitesService>(GetAuthenticatingHttpClient());
        }

        public IGameService GetGameService()
        {
            return RestService.For<IGameService>(GetAuthenticatingHttpClient());
        }

        public IUserService GetUserService()
        {
            return RestService.For<IUserService>(GetAuthenticatingHttpClient());
        }

        public IMiscService GetMiscService()
        {
            return RestService.For<IMiscService>(GetAuthenticatingHttpClient());
        }

        public IChannelService GetChannelService()
        {
            return RestService.For<IChannelService>(GetAuthenticatingHttpClient());
        }

        public IGuildService GetGuildService()
        {
            return RestService.For<IGuildService>(GetAuthenticatingHttpClient());
        }

        public IInviteService GetInviteService()
        {
            return RestService.For<IInviteService>(GetAuthenticatingHttpClient());
        }

        public IVoiceService GetVoiceService()
        {
            return RestService.For<IVoiceService>(GetAuthenticatingHttpClient());
        }

        public HttpClient GetAuthenticatingHttpClient()
        {
            return new HttpClient(GetAuthenticationHandler())
            {
                BaseAddress = new Uri(_apiConfig.BaseUrl)
            };
        }

        private HttpClientHandler GetAuthenticationHandler()
        {
            return new AuthenticatingHttpClientHandler(_authenticator);
        }
    }
}
