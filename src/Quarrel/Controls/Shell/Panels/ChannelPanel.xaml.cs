// Adam Dernis © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels
{
    public sealed partial class ChannelPanel : UserControl
    {
        private static readonly DependencyProperty BottomMarginProperty =
            DependencyProperty.Register(nameof(BottomMargin), typeof(double), typeof(ChannelPanel), new PropertyMetadata(0d));

        public ChannelPanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<ChannelsViewModel>();
        }

        public ChannelsViewModel ViewModel => (ChannelsViewModel)DataContext;

        public double BottomMargin
        {
            get => (double)GetValue(BottomMarginProperty);
            set => SetValue(BottomMarginProperty, value);
        }

        private void ChannelList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BindableChannel {IsTextChannel: true} channel)
            {
                if (channel is BindableGuildChannel { Permissions: { ReadMessages: false } })
                {
                    return; 
                }

                ViewModel.SelectedChannel = channel;
            }
        }
    }
}
