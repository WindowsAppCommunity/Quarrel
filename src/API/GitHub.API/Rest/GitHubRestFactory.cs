// Quarrel © 2022

using GitHub.API.HttpHandlers;
using Refit;
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
        public GitHubRestFactory(string userAgent)
        {
            _userAgent = userAgent;
        }
        
        /// <summary>
        /// Gets an instance of the <see cref="IGitHubService"/>.
        /// </summary>
        public IGitHubService GetGitHubService()
        {
            return RestService.For<IGitHubService>(GetHttpClient());
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
