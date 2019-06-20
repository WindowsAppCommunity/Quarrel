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
                    _Source.Clear();
                    var itemList = await Services.ServicesManager.Discord.ChannelService.GetChannelMessages(m.ChannelId);
                    foreach (var item in itemList)
                    {
                        _Source.Add(new BindableMessage(item));
                        RaisePropertyChanged(nameof(Source));
                    }
                });
            });
        }
        public ObservableCollection<BindableMessage> _Source { get; private set; } = new ObservableCollection<BindableMessage>();

        public IEnumerable<BindableMessage> Source { get => _Source.Reverse(); }
    }
}
