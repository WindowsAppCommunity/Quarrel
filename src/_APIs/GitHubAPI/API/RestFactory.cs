using GitHubAPI.HttpHandlers;
using Refit;
using System;
using System.Net.Http;

namespace GitHubAPI.API
{
    public class RestFactory
    {
        /// <summary>
        /// Gets the base URL for the service
        /// </summary>
        public const string BaseUrl = "https://api.github.com/";

        // Helper method to return an authenticated client
        private static HttpClient GetHttpClient(string userAgent)
        {
            return new HttpClient(new UnauthenticatedHttpClientHandler(userAgent))
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        /// <summary>
        /// Gets a new instance of an unauthenticated GitHub service
        /// </summary>
        /// <param name="userAgent">The user agent to use to send web requests</param>
        public static IGitHubService GetGitHubService(string userAgent) => RestService.For<IGitHubService>(GetHttpClient(userAgent));
    }
}
