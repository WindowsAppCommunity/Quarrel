// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell
{
    public sealed partial class CurrentUserButton : UserControl
    {
        public CurrentUserButton()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<CurrentUserViewModel>();
        }

        public CurrentUserViewModel ViewModel => (CurrentUserViewModel)DataContext;
    }
}
