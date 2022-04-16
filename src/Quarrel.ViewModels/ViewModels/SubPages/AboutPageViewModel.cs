// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages
{
    /// <summary>
    /// ViewModel for the AboutPage.
    /// </summary>
    public class AboutPageViewModel : ObservableRecipient
    {
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
        /// </summary>
        public AboutPageViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        /// Gets a value indicating whether or not the app's current language is the neutral language.
        /// </summary>
        public bool IsNeutralLanguage => _localizationService.IsNeutralLanguage;
    }
}
