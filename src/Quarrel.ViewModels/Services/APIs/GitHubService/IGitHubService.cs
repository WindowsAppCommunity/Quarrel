// Quarrel © 2022

using GitHub.API.Models;
using Quarrel.Services.APIs.GitHubService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Services.APIs.GitHubService
{
    /// <summary>
    /// A service for a service that makes requests to the GitHub API.
    /// </summary>
    public interface IGitHubService
    {
        /// <summary>
        /// Gets the list contributors to the repository.
        /// </summary>
        Task<List<BindableContributor>?> GetContributors();

        /// <summary>
        /// Gets the full developer profile for a user in a repository.
        /// </summary>
        /// <param name="contributor">The contributor to get the a full profile for.</param>
        /// <returns></returns>
        Task<BindableDeveloper> GetDeveloper(Contributor contributor);
    }
}
