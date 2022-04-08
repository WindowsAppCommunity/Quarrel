// Adam Dernis © 2022

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Messages.Navigation;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Shell.Panels
{
    public sealed partial class ChannelPanel : UserControl
    {
        private static readonly DependencyProperty BottomMarginProperty =
            DependencyProperty.Register(nameof(BottomMargin), typeof(double), typeof(ChannelPanel), new PropertyMetadata(0d));

        private readonly IMessenger _messenger;

        public ChannelPanel()
        {
            this.InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<ChannelsViewModel>();
            _messenger = App.Current.Services.GetRequiredService<IMessenger>();
        }

        public ChannelsViewModel ViewModel => (ChannelsViewModel)DataContext;

        public double BottomMargin
        {
            get => (double)GetValue(BottomMarginProperty);
            set => SetValue(BottomMarginProperty, value);
        }

        private void ChannelList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is BindableChannel channel)
            {
                if (channel.IsTextChannel)
                {
                    ViewModel.SelectedChannel = channel;
                    _messenger.Send(new NavigateToChannelMessage<BindableChannel>(channel));
                }
            }
        }
    }
}
