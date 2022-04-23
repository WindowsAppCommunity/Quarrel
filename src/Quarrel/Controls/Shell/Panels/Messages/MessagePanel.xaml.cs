// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels.Messages
{
    public sealed partial class MessagePanel : UserControl
    {
        public MessagePanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<MessagesViewModel>();
        }

        public MessagesViewModel ViewModel => (MessagesViewModel)DataContext;
    }
}
