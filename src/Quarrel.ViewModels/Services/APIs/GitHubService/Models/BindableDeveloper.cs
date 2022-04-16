// Quarrel © 2022

using System;
using GitHubContributor = GitHub.API.Models.Contributor;
using GitHubUser = GitHub.API.Models.User;

namespace Quarrel.Services.APIs.GitHubService.Models
{
    /// <summary>
    /// A wrapper for a <see cref="GitHubUser"/> and <see cref="GitHubContributor"/>.
    /// </summary>
    public class BindableDeveloper : BindableContributor
    {
        private GitHubUser _user;

        /// <summary>
        /// Initializes new instance of the <see cref="BindableDeveloper"/> class.
        /// </summary>
        public BindableDeveloper(GitHubUser user, GitHubContributor contributor) : base(contributor)
        {
            _user = user;
        }
        
        /// <summary>
        /// Gets the developer's GitHub user info.
        /// </summary>
        public GitHubUser User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }
    }
}
