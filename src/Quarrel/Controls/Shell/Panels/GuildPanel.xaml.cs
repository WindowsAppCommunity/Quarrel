// Adam Dernis © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels
{
    public sealed partial class GuildPanel : UserControl
    {
        public GuildPanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<GuildsViewModel>();
        }

        public GuildsViewModel ViewModel => (GuildsViewModel)DataContext;
    }
}
