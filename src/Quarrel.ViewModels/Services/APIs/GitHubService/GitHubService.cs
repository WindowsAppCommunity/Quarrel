// Quarrel © 2022

using GitHub.API.Models;
using GitHub.API.Rest;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.APIs.GitHubService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGitHubApiService = GitHub.API.Rest.IGitHubService;

namespace Quarrel.Services.APIs.GitHubService
{
    /// <summary>
    /// A service for making requests to the GitHub API.
    /// </summary>
    public class GitHubService : IGitHubService
    {
        private const string RepoOwner = "UWPCommunity";
        private const string RepoName = "Quarrel";

        private readonly IAnalyticsService _analyticsService;
        private IGitHubApiService _gitHubApiService;

        /// <summary>
        /// Initializes new instance of the <see cref="GitHubService"/> class.
        /// </summary>
        public GitHubService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            GitHubRestFactory restFactory = new GitHubRestFactory("Quarrel");
            _gitHubApiService = restFactory.GetGitHubService();
        }

        /// <inheritdoc/>
        public async Task<List<BindableContributor>?> GetContributors()
        {
            // Get contributors
            Contributor[]? contributors = null;
            try
            {
                contributors = await _gitHubApiService.GetContributorsAsync(RepoOwner, RepoName);
            }
            catch (Exception ex)
            {
                _analyticsService.Log(LoggedEvent.GitHubRequestFailed, ("Endpoint", "GetContributors"), ("Exception", ex.Message));
            }

            if (contributors is null)
            {
                return null;
            }

            // Sort by descending commit count
            Array.Sort(contributors, Comparer<Contributor>.Create((item1, item2) =>
                item2.CommitsCount.CompareTo(item1.CommitsCount)));

            List<BindableContributor> result = new List<BindableContributor>(contributors.Length);
            foreach (var contributor in contributors)
            {
                result.Add(new BindableContributor(contributor));
            }

            return result;
        }
        
        /// <inheritdoc/>
        public async Task<BindableDeveloper> GetDeveloper(Contributor contributor)
        {
            var user = await _gitHubApiService.GetUserAsync(contributor.Name);
            return new BindableDeveloper(user, contributor);
        }
    }
}
