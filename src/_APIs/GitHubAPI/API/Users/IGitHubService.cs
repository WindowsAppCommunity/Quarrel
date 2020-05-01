using GitHubAPI.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitHubAPI.API
{
    /// <summary>
    /// A basic GitHub service that uses the public APIs
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
        /// Gets the list of contributors for a given repository
        /// </summary>
        /// <param name="owner">The repository owner account name</param>
        /// <param name="repo">The repository name</param>
        [Get("/repos/{owner}/{repo}/contributors")]
        Task<IEnumerable<Contributor>> GetContributorsAsync([AliasAs("owner")] string owner, [AliasAs("repo")] string repo);
    }
}
