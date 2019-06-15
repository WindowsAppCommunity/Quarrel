using Refit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordAPI.API.Activities;
using DiscordAPI.API.Channel;
using DiscordAPI.API.Connections;
using DiscordAPI.API.Game;
using DiscordAPI.API.Guild;
using DiscordAPI.API.Invite;
using DiscordAPI.API.Misc;
using DiscordAPI.API.User;
using DiscordAPI.API.Voice;
using DiscordAPI.Authentication;

namespace DiscordAPI.API
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
