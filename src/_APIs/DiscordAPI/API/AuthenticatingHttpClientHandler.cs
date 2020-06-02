using DiscordAPI.Authentication;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAPI.API
{
    /// <summary>
    /// An authenticated <see cref="HttpClientHandler"/>.
    /// </summary>
    public class AuthenticatingHttpClientHandler : HttpClientHandler
    {
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatingHttpClientHandler"/> class.
        /// </summary>
        /// <param name="authenticator">The authentication.</param>
        public AuthenticatingHttpClientHandler(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        /// <summary>
        /// Send a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">A cancelation token.</param>
        /// <returns>An Http Response.</returns>
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
