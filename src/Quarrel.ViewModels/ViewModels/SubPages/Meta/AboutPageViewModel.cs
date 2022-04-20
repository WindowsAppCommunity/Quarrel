// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Localization;
using Quarrel.Services.Versioning;
using Quarrel.Services.Versioning.Enums;

namespace Quarrel.ViewModels.SubPages.Meta
{
    /// <summary>
    /// ViewModel for the AboutPage.
    /// </summary>
    public partial class AboutPageViewModel : ObservableRecipient
    {
        private const string CommitResource = "About/Commit";
        private const string BranchResource = "About/Branch";
        private const string QuarrelDevResource = "About/QuarrelDev";
        private const string QuarrelAlphaResource = "About/QuarrelAlpha";
        private const string QuarrelInsiderResource = "About/QuarrelInsider";
        private const string QuarrelReleaseResource = "About/QuarrelRelease";

        private readonly IMessenger _messenger;
        private readonly ILocalizationService _localizationService;
        private readonly IVersioningService _versioningService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
        /// </summary>
        public AboutPageViewModel(
            IMessenger messenger,
            ILocalizationService localizationService,
            IVersioningService versioningService)
        {
            _messenger = messenger;
            _localizationService = localizationService;
            _versioningService = versioningService;
        }

        /// <summary>
        /// Gets the app version as a <see cref="string"/>.
        /// </summary>
        public string AppVersion => string.Format("{0}.{1}.{2}",
            _versioningService.AppVersion.MajorVersion,
            _versioningService.AppVersion.MinorVersion,
            _versioningService.AppVersion.BuildNumber);

        /// <summary>
        /// Gets the app's version type.
        /// </summary>
        public string AppVersionType
        {
            get
            {
                string resource = _versioningService.AppVersion.VersionType switch
                {
                    VersionType.Dev => QuarrelDevResource,
                    VersionType.Alpha => QuarrelAlphaResource,
                    VersionType.Insider => QuarrelInsiderResource,
                    VersionType.Release or _ => QuarrelReleaseResource,
                };

                return _localizationService[resource];
            }
        }

        /// <summary>
        /// Gets the commit info localized.
        /// </summary>
        public string CommitInfo => _localizationService
            [CommitResource, _versioningService.GitVersionInfo.Commit];
        
        /// <summary>
        /// Gets the branch info localized.
        /// </summary>
        public string BranchInfo => _localizationService
            [BranchResource, _versioningService.GitVersionInfo.Branch];

        /// <summary>
        /// Gets a value indicating whether or not the app's current language is the neutral language.
        /// </summary>
        public bool IsNeutralLanguage => _localizationService.IsNeutralLanguage;
        
        /// <summary>
        /// Sends a request to open the credit page.
        /// </summary>
        [ICommand]
        public void NavigateToCreditPage()
        {
            _messenger.Send(new NavigateToSubPageMessage(typeof(CreditPageViewModel)));
        }
    }
}
