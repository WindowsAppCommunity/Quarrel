// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using GitHubContributor = GitHub.API.Models.Contributor;

namespace Quarrel.Services.APIs.GitHubService.Models
{
    /// <summary>
    /// A wrapper for a <see cref="GitHubContributor"/>.
    /// </summary>
    public class BindableContributor : ObservableObject
    {
        private GitHubContributor _contributor;

        /// <summary>
        /// Initializes new instance of the <see cref="BindableDeveloper"/> class.
        /// </summary>
        public BindableContributor(GitHubContributor contributor)
        {
            _contributor = contributor;
        }
        
        /// <summary>
        /// Gets the developer's information as a contributor to Quarrel.
        /// </summary>
        public GitHubContributor Contributor
        {
            get => _contributor;
            set => SetProperty(ref _contributor, value);
        }
    }
}
