// Quarrel © 2022

using Microsoft.Extensions.DependencyInjection;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.ViewModels.Panels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Controls.Panels.Channels
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
            if (e.ClickedItem is IBindableSelectableChannel selectableChannel)
            {
                if (!selectableChannel.IsAccessible)
                {
                    return; 
                }

                ViewModel.SelectedChannel = selectableChannel;
            }
            else if (e.ClickedItem is BindableVoiceChannel voiceChannel)
            {
                voiceChannel.JoinCall();
            }
        }
    }
}
