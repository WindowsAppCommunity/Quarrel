// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.APIs.GitHubService;
using Quarrel.Services.APIs.GitHubService.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.SubPages.Meta
{
    public class CreditPageViewModel : ObservableRecipient
    {
        private readonly static string[] DevelopersUsernames = new[] { "Avid29", "matthew4850" };
        private readonly IGitHubService _gitHubService;

        public CreditPageViewModel(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
            
            Developers = new ObservableCollection<BindableDeveloper>();
            Contributors = new ObservableRangeCollection<BindableContributor>();

            LoadLists();
        }

        public ObservableCollection<BindableDeveloper> Developers { get; }

        public ObservableRangeCollection<BindableContributor> Contributors { get; }

        private async void LoadLists()
        {
            var contributors = (await _gitHubService.GetContributors()).ToList();
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
