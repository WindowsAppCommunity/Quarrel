// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels.Guilds
{
    public sealed partial class GuildPanel : UserControl
    {
        public GuildPanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<GuildsViewModel>();
        }

        public GuildsViewModel ViewModel => (GuildsViewModel)DataContext;

        private void GuildList_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var container = (TreeViewItem)sender.ContainerFromItem(args.InvokedItem);
            if (args.InvokedItem is IBindableSelectableGuildItem guild)
            {
                ViewModel.SelectedGuild = guild;
            }
            else
            {
                args.Handled = true;
            }
        }
    }
}
