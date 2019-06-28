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
                    Source.Clear();
                    var itemList = await Services.ServicesManager.Discord.ChannelService.GetChannelMessages(m.ChannelId);

                    Message lastItem = null;

                    foreach (Message item in itemList.Reverse())
                    {
                        Source.Add(new BindableMessage(item, m.GuildId, lastItem));
                        lastItem = item;
                    }
                });
            });
        }

        public ObservableCollection<BindableMessage> Source { get; private set; } = new ObservableCollection<BindableMessage>();
    }
}
