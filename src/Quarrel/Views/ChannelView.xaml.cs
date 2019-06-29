using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Models.Bindables;
using Quarrel.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UICompositionAnimations.Brushes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Views
{
    public sealed partial class ChannelView : UserControl
    {
        public ChannelView()
        {
            this.InitializeComponent();
            DataContext = new ChannelViewModel();
        }

        public ChannelViewModel ViewModel => DataContext as ChannelViewModel;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as BindableChannel).IsCategory)
            {
                bool newState = !(e.ClickedItem as BindableChannel).Collapsed;
                for (int i = ChannelList.Items.IndexOf(e.ClickedItem);
                    i < ChannelList.Items.Count
                    && (ChannelList.Items[i] is BindableChannel bChannel)
                    && bChannel.ParentId == (e.ClickedItem as BindableChannel).Model.Id;
                    i++)
                {
                    bChannel.Collapsed = newState;
                }
            } else
            {
                Messenger.Default.Send(new Messages.Navigation.ChannelNavigateMessage((e.ClickedItem as BindableChannel), ViewModel.Guild));
            }
        }
    }
}
