using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables;
using Quarrel.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.DataTemplates.Messages
{
    public partial class MessageTemplate
    {
        public MessageTemplate()
        {
            this.InitializeComponent();
        }

        private void Pin(object sender, RoutedEventArgs e)
        {
            var message = ((e.OriginalSource as MenuFlyoutItem).DataContext as BindableMessage);
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.AddPinnedChannelMessage(message.Model.ChannelId, message.Model.Id);
        }

        private void Unpin(object sender, RoutedEventArgs e)
        {

        }

        private void Edit(object sender, RoutedEventArgs e)
        {
            // TODO: Enter edit mode
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            var message = ((e.OriginalSource as MenuFlyoutItem).DataContext as BindableMessage);
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.DeleteMessage(message.Model.ChannelId, message.Model.Id);
        }
    }
}
