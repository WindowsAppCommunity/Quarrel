using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels;
using Quarrel.ViewModels.Services.Voice;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class VoiceConnection : UserControl
    {
        public MainViewModel ViewModel => App.ViewModelLocator.Main;

        public VoiceConnection()
        {
            this.InitializeComponent();
        }

        private void DeafenToggle(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IVoiceService>().ToggleDeafen();
        }

        private void MuteToggle(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IVoiceService>().ToggleMute();
        }
    }
}
