// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Meta;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Meta
{
    public sealed partial class AboutPage : UserControl
    {
        private const string CommitResource = "About/Commit";
        private const string BranchResource = "About/Branch";
        private ILocalizationService _localizationService;

        public AboutPage()
        {
            this.InitializeComponent();
            _localizationService = App.Current.Services.GetRequiredService<ILocalizationService>();
        }

        public AboutPageViewModel ViewModel => (AboutPageViewModel)DataContext;

        public string AppVersion => string.Format("{0}.{1}.{2}",
            Package.Current.Id.Version.Major,
            Package.Current.Id.Version.Minor,
            Package.Current.Id.Version.Build);

        public string CommitInfo => _localizationService
            [CommitResource, ThisAssembly.Git.Commit];

        public string BranchInfo => _localizationService
            [BranchResource, ThisAssembly.Git.Branch];

    }
}
