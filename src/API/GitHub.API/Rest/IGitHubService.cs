// Quarrel © 2022

using GitHub.API.Models;
using Refit;
using System.Threading.Tasks;

namespace GitHub.API.Rest
{
    /// <summary>
    /// A service for making requests to the GitHub rest api.
    /// </summary>
    public interface IGitHubService
    {   
        /// <summary>
        /// Gets a single GitHub user
        /// </summary>
        /// <param name="username">The name of the user to retrieve</param>
        [Get("/users/{username}")]
        Task<User> GetUserAsync(string username);

        /// <summary>
        /// Gets the list of contributors for a given repository.
        /// </summary>
        /// <param name="owner">The repository owner name.</param>
        /// <param name="repo">The repository name.</param>
        [Get("/repos/{owner}/{repo}/contributors")]
        Task<Contributor[]> GetContributorsAsync([AliasAs("owner")] string owner, [AliasAs("repo")] string repo);
    }
}
