// Copyright (c) Quarrel. All rights reserved.

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
    /// Credit page data.
    /// </summary>
    public class CreditPageViewModel : ViewModelBase
    {
        private IEnumerable<Contributor> _contributors;
        private ObservableCollection<BindableDeveloper> _developers = new ObservableCollection<BindableDeveloper>();
        private IGitHubService _gitHubService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditPageViewModel"/> class.
        /// </summary>
        public CreditPageViewModel()
        {
            _gitHubService = RestFactory.GetGitHubService("Quarrel|UWP");
            LoadContributors();
        }

        /// <summary>
        /// Gets or sets the preset list as User of Lead Developers.
        /// </summary>
        public ObservableCollection<BindableDeveloper> Developers
        {
            get => _developers;
            set => Set(ref _developers, value);
        }

        /// <summary>
        /// Gets or sets everyone (except developers) with a contribution to Quarrel.
        /// </summary>
        public IEnumerable<Contributor> Contributors
        {
            get => _contributors;
            set => Set(ref _contributors, value);
        }

        /// <summary>
        /// Loads Contributors and Lead Developer's data from GitHub.
        /// </summary>
        private async void LoadContributors()
        {
            // Get Contributor list
            var contributors = await _gitHubService.GetContributorsAsync(Constants.Store.GitHubRepoOwner, Constants.Store.GitHubRepoName);

            // Order Contributors by commit count
            contributors = contributors.OrderByDescending(x => x.CommitsCount);

            // Get Developers list
            var developers = new string[] { "Avid29", "karmaecrivain94", "matthew4850" };
            foreach (string dev in developers)
            {
                var user = await _gitHubService.GetUserAsync(dev);
                Developers.Add(new BindableDeveloper(user, contributors.FirstOrDefault(x => x.Name == dev)));
            }

            // Remove developers from Contributors list
            Contributors = contributors.Where(x => !developers.Contains(x.Name));
        }
    }
}
