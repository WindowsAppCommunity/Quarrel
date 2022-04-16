// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels.SubPages.Host;
using Windows.UI.Xaml.Controls;

namespace Quarrel.SubPages.Host
{
    public sealed partial class SubPageHost : UserControl
    {
        public SubPageHost()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<SubPageHostViewModel>();
        }

        SubPageHostViewModel ViewModel => (SubPageHostViewModel)DataContext;
    }
}
