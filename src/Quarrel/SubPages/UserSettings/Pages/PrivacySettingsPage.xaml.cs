using Quarrel.ViewModels.SubPages.UserSettings.Pages;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages.UserSettings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrivacySettingsPage : Page
    {
        public PrivacySettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new PrivacySettingsViewModel();
        }

        public PrivacySettingsViewModel ViewModel => this.DataContext as PrivacySettingsViewModel;
    }
}
