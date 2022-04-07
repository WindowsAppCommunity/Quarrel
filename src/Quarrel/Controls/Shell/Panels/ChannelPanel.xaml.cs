// Adam Dernis © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels
{
    public sealed partial class ChannelPanel : UserControl
    {
        public ChannelPanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<ChannelsViewModel>();
        }
        public ChannelsViewModel ViewModel => (ChannelsViewModel)DataContext;

        private void ChannelList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (e.ClickedItem is BindableChannel channel)
            {
                if (channel.IsSelectable)
                    ViewModel.SelectedChannel = channel;
            }
        }
    }
}
