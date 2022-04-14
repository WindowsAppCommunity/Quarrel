// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class WindowHost : UserControl
    {
        public WindowHost()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<WindowViewModel>();
        }

        public WindowViewModel ViewModel => (WindowViewModel)DataContext;
    }
}
