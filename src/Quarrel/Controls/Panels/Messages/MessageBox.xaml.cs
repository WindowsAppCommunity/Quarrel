// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Messages
{
    public sealed partial class MessageBox : UserControl
    {
        public MessageBox()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<MessageBoxViewModel>();
        }

        public MessageBoxViewModel ViewModel => (MessageBoxViewModel)DataContext;
    }
}
