using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Gateway;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class CurrentUserButton : UserControl
    {
        public MainViewModel ViewModel => App.ViewModelLocator.Main;
        public CurrentUserButton()
        {
            this.InitializeComponent();
        }

        private async void StatusSelected(object sender, RoutedEventArgs e)
        {
            string status = (sender as RadioButton).Tag.ToString();
            SimpleIoc.Default.GetInstance<IGatewayService>().Gateway.UpdateStatus(status, 0, null);
            await SimpleIoc.Default.GetInstance<IDiscordService>().UserService.UpdateSettings("{\"status\":\"" + status + "\"}");
        }
    }
}
