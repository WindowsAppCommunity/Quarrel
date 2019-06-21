using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordAPI.Authentication;

namespace DiscordAPI.API
{
    public class AuthenticatingHttpClientHandler : HttpClientHandler
    {
        private readonly IAuthenticator _authenticator;

        public AuthenticatingHttpClientHandler(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _authenticator.GetToken();
            request.Headers.Add("Authorization", token);
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }
    }
}
