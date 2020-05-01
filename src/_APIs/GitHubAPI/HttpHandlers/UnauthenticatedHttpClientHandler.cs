using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubAPI.HttpHandlers
{
    /// <summary>
    /// A custom <see cref="HttpClientHandler"/> to perform public actions
    /// </summary>
    internal sealed class UnauthenticatedHttpClientHandler : HttpClientHandler
    {
        // The user agent to use to send the requests
        private readonly string UserAgent;

        public UnauthenticatedHttpClientHandler(string userAgent) => UserAgent = userAgent;

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Headers setup
            request.Headers.Add("User-Agent", UserAgent);

            // Send the request and handle errors
            return base.SendAsync(request, cancellationToken);
        }
    }
}
