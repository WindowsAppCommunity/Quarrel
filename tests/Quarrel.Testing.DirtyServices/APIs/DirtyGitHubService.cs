// Quarrel © 2022

using GitHub.API.Models;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.APIs.GitHubService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.Testing.DirtyServices.APIs
{
    /// <summary>
    /// A dirty implementation of the <see cref="IGitHubService"/>.
    /// </summary>
    public class DirtyGitHubService : IGitHubService
    {
        /// <inheritdoc/>
        public Task<List<BindableContributor>?> GetContributors()
        {
            return Task.FromResult<List<BindableContributor>?>(null);
        }
        
        /// <inheritdoc/>
        public Task<BindableDeveloper?> GetDeveloper(Contributor contributor)
        {
            return Task.FromResult<BindableDeveloper?>(null);
        }
    }
}
