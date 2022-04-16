// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.APIs.GitHubService.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.Meta
{
    /// <summary>
    /// A view model for the credit page.
    /// </summary>
    public class CreditPageViewModel : ObservableRecipient
    {
        private readonly static string[] DevelopersUsernames = new[] { "Avid29", "matthew4850" };
        private readonly IGitHubService _gitHubService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditPageViewModel"/> class.
        /// </summary>
        public CreditPageViewModel(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
            
            Developers = new ObservableCollection<BindableDeveloper>();
            Contributors = new ObservableRangeCollection<BindableContributor>();

            LoadLists();
        }

        /// <summary>
        /// Gets the list of complete developers.
        /// </summary>
        public ObservableCollection<BindableDeveloper> Developers { get; }

        /// <summary>
        /// Gets the list of contributors.
        /// </summary>
        public ObservableRangeCollection<BindableContributor> Contributors { get; }

        private async void LoadLists()
        {
            List<BindableContributor>? contributors = await _gitHubService.GetContributors();

            if (contributors is null)
            {
                return;
            }

            foreach (string dev in DevelopersUsernames)
            {
                var contributor = contributors.FirstOrDefault(x => x.Contributor.Name == dev);
                contributors.Remove(contributor);
                Developers.Add(await _gitHubService.GetDeveloper(contributor.Contributor));
            }
         
            Contributors.AddRange(contributors);
        }
    }
}
