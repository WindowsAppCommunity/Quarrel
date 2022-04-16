// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Localization;

namespace Quarrel.ViewModels.SubPages.Meta
{
    /// <summary>
    /// ViewModel for the AboutPage.
    /// </summary>
    public partial class AboutPageViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
        /// </summary>
        public AboutPageViewModel(IMessenger messenger, ILocalizationService localizationService)
        {
            _messenger = messenger;
            _localizationService = localizationService;
        }

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
