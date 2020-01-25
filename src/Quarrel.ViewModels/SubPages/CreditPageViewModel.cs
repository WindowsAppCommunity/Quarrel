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
        private async void LoadContributors()
        {
            var contributors = await GitHubService.GetContributorsAsync(Constants.Store.GitHubRepoOwner, Constants.Store.GitHubRepoName);

            contributors = contributors.OrderByDescending(x => x.CommitsCount);

            Developers = new string[] { "Avid29", "karmaecrivain94", "matthew4850" }.Select(name => contributors.FirstOrDefault(x => x.Name == name));
            Contributors = contributors.Where(x => !Developers.Contains(x));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Preset list as User of Lead Developers
        /// </summary>
        public IEnumerable<Contributor> Developers
        {
            get => _Developers;
            set => Set(ref _Developers, value);
        }
        private IEnumerable<Contributor> _Developers;

        /// <summary>
        /// Everyone (except developers) with a contribution to Quarrel
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
