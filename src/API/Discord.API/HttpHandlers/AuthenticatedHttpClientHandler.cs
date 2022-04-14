// Quarrel © 2022

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.HttpHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that creates authenticated requests. 
    /// </summary>
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private readonly string _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedHttpClientHandler"/> class.
        /// </summary>
        /// <param name="token">The token to use for authentication.</param>
        public AuthenticatedHttpClientHandler(string token)
        {
            _token = token;
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", _token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
