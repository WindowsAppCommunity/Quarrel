// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Messages
{
    public sealed partial class VoicePanel : UserControl
    {
        public VoicePanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<VoicePanelViewModel>();
        }

        public VoicePanelViewModel ViewModel => (VoicePanelViewModel)DataContext;
    }
}
