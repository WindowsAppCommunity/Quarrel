using GalaSoft.MvvmLight;
using GitHubAPI.API;
using GitHubAPI.Models;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables.GitHub;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            // Get Contributor list
            var contributors = await GitHubService.GetContributorsAsync(Constants.Store.GitHubRepoOwner, Constants.Store.GitHubRepoName);

            // Order Contributors by commit count
            contributors = contributors.OrderByDescending(x => x.CommitsCount);

            // Get Developers list
            var developers = new string[] { "Avid29", "karmaecrivain94", "matthew4850" };
            foreach (string dev in developers)
            {
                var user = await GitHubService.GetUserAsync(dev);
                Developers.Add(new BindableDeveloper(user, contributors.FirstOrDefault(x => x.Name == dev)));
            }

            // Remove developers from Contributors list
            Contributors = contributors.Where(x => !developers.Contains(x.Name));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Preset list as User of Lead Developers
        /// </summary>
        public ObservableCollection<BindableDeveloper> Developers
        {
            get => _Developers;
            set => Set(ref _Developers, value);
        }
        private ObservableCollection<BindableDeveloper> _Developers = new ObservableCollection<BindableDeveloper>();

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
