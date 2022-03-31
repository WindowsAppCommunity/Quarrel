// Adam Dernis © 2022

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.HttpHandlers
{
    public class AuthenticatedHttpClientHandler : HttpClientHandler
    {
        private readonly string _token;

        public AuthenticatedHttpClientHandler(string token)
        {
            _token = token;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Authorization", _token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
