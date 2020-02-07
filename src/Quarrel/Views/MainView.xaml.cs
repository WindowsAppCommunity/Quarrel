using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Messages;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quarrel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView(SplashScreen splash)
        {
            this.InitializeComponent();
            ExtendedSplashScreen.InitializeAnimation(splash);
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            if (connections != null)
            {
                ViewModel.Login();
            }
            else
            {
                Messenger.Default.Send(new ConnectionStatusMessage(Status.Offline));
            }
        }

        public MainViewModel ViewModel => App.ViewModelLocator.Main;
    }
}
