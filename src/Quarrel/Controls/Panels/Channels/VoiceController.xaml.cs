// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Channels
{
    public sealed partial class VoiceController : UserControl
    {
        public VoiceController()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<VoiceControllerViewModel>();
        }

        public VoiceControllerViewModel ViewModel => (VoiceControllerViewModel)DataContext;
    }
}
