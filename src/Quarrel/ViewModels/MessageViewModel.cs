using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using UICompositionAnimations.Helpers;
using System.Collections;
using DiscordAPI.Models;
using Quarrel.Messages.Gateway;

namespace Quarrel.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        public MessageViewModel()
        {
            Messenger.Default.Register<ChannelNavigateMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(async () =>
                {
                    Channel = m.Channel;

                    Source.Clear();
                    var itemList = await Services.ServicesManager.Discord.ChannelService.GetChannelMessages(m.Channel.Model.Id);

                    Message lastItem = null;

                    foreach (Message item in itemList.Reverse())
                    {
                        Source.Add(new BindableMessage(item, guildId, lastItem));
                        lastItem = item;
                    }
                });
            });

            Messenger.Default.Register<GatewayMessageRecievedMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    if (Channel != null && Channel.Model.Id == m.Message.ChannelId)
                        Source.Add(new BindableMessage(m.Message, guildId, Source.LastOrDefault().Model));
                });
            });
        }

        private string guildId
        {
            get => Channel.Model is GuildChannel gChn ? gChn.GuildId : "DM";
        }

        private BindableChannel _Channel;

        public BindableChannel Channel
        {
            get => _Channel;
            set => Set(ref _Channel, value);
        }

        public ObservableCollection<BindableMessage> Source { get; private set; } = new ObservableCollection<BindableMessage>();
    }
}