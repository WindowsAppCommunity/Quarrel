using GalaSoft.MvvmLight;
using GitHubAPI.API;
using GitHubAPI.Models;
using Quarrel.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// Loads and Stores GitHub context for CreditPage
    /// </summary>
    public class CreditPageViewModel : ViewModelBase
    {
        #region Constructors

        public CreditPageViewModel()
        {
            GitHubService = RestFactory.GetGitHubService("Quarrel|UWP");
            LoadContributors();
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Loads Contributors and Lead Developer's data from GitHub
        /// </summary>
        public async void LoadContributors()
        {
            var contributors = await GitHubService.GetContributorsAsync(Constants.Store.GitHubRepoOwner, Constants.Store.GitHubRepoName);
            
            // TODO: Seperate Lead Developers from Contributors
            Contributors = contributors;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Preset list as User of Lead Developers
        /// </summary>
        public IEnumerable<User> LeadDevelopers
        {
            get => _LeadDevelopers;
            set => Set(ref _LeadDevelopers, value);
        }
        private IEnumerable<User> _LeadDevelopers;

        /// <summary>
        /// Everyone (except lead developers) with a contribution to Quarrel
        /// </summary>
        public IEnumerable<Contributor> Contributors
        {
            get => _Contributors;
            set => Set(ref _Contributors, value);
        }
        private IEnumerable<Contributor> _Contributors;

        /// <summary>
        /// API Service for GitHub
        /// </summary>
        private IGitHubService GitHubService;

        #endregion
    }
}
