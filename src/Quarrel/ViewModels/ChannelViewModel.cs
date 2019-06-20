using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using Quarrel.Messages.Gateway;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using UICompositionAnimations.Helpers;

namespace Quarrel.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        public ChannelViewModel()
        {
            Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    Source.Clear();
                    var itemList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableChannel>>(Constants.Cache.Keys.ChannelList, m.GuildId);
                    foreach (var item in itemList)
                    {
                        Source.Add(item);
                    }
                });
            });
        }

        public ObservableCollection<BindableChannel> Source { get; private set; } = new ObservableCollection<BindableChannel>();
    }
}
