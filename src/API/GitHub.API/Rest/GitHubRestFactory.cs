// Quarrel © 2022

using GitHub.API.HttpHandlers;
using System;
using System.Net.Http;

namespace GitHub.API.Rest
{
    /// <summary>
    /// A class for initializing GitHub rest services.
    /// </summary>
    public class GitHubRestFactory
    {
        private const string BaseUrl = "https://api.github.com/";
        private readonly string _userAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubRestFactory"/> class.
        /// </summary>
        /// <param name="userAgent"></param>
        public GitHubRestFactory(string userAgent)
        {
            _userAgent = userAgent;
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient(new UserAgentHttpHandler(_userAgent))
            {
                BaseAddress = new Uri(BaseUrl),
            };
        }
    }
}
