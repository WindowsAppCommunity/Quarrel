using DiscordAPI.API.Guild;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Quarrel.Controls.Messages
{
    public sealed partial class MessageFlyout : UserControl
    {
        public MessageFlyout()
        {
            this.InitializeComponent();
        }

        public BindableMessage ViewModel => DataContext as BindableMessage;

        public BindableChannel Channel => SimpleIoc.Default.GetInstance<IGuildsService>().CurrentChannels[ViewModel.Model.ChannelId];

        public bool ShowPin
        {
            get => !ViewModel.Model.Pinned && (Channel.Permissions.ManageMessages || Channel.IsDirectChannel);
        }

        public bool ShowUnpin
        {
            get => ViewModel.Model.Pinned && (Channel.Permissions.ManageMessages || Channel.IsDirectChannel);
        }

        public bool ShowEdit
        {
            get => ViewModel.Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id;
        }

        public bool ShowDelete
        {
            get => ViewModel.Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id
                || (Channel.Permissions.ManageMessages && !Channel.IsDirectChannel);
        }


        private void Pin(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.AddPinnedChannelMessage(ViewModel.Model.ChannelId, ViewModel.Model.Id);
        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            // TODO: Edit mode
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.DeleteMessage(ViewModel.Model.ChannelId, ViewModel.Model.Id);
        }
    }
}
