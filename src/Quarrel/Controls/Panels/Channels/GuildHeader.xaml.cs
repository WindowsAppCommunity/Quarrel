// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Channels
{
    public sealed partial class GuildHeader : UserControl
    {
        public GuildHeader()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<GuildsViewModel>();
        }

        public GuildsViewModel ViewModel => (GuildsViewModel)DataContext;
    }
}
