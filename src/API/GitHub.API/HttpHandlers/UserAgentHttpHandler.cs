// Quarrel © 2022

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.API.HttpHandlers
{
    /// <summary>
    /// A <see cref="HttpClientHandler"/> to make requests with a user-agent header.
    /// </summary>
    internal class UserAgentHttpHandler : HttpClientHandler
    {
        private readonly string _userAgent;

        public UserAgentHttpHandler(string userAgent)
        {
            _userAgent = userAgent;
        }

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("User-Agent", _userAgent);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
