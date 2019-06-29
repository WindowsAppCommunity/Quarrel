using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
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
                    GuildId = m.GuildId;
                    Source.Clear();
                    var itemList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableChannel>>(Constants.Cache.Keys.ChannelList, m.GuildId);
                    foreach (var item in itemList)
                    {
                        Source.Add(item);
                    }
                });
            });
        }

        private string _GuildId;

        public string GuildId
        {
            get => _GuildId;
            set
            {
                if (Set(ref _GuildId, value))
                    RaisePropertyChanged(nameof(Guild));
            }
        }

        public BindableGuild Guild => ServicesManager.Cache.Runtime.TryGetValue<BindableGuild>(Quarrel.Helpers.Constants.Cache.Keys.Guild, GuildId);

        public ObservableCollection<BindableChannel> Source { get; private set; } = new ObservableCollection<BindableChannel>();


        private RelayCommand<BindableChannel> navigateChannelCommand;

        public RelayCommand<BindableChannel> NavigateChannelCommand => navigateChannelCommand ?? (navigateChannelCommand = new RelayCommand<BindableChannel>((channel) =>
        {

            if (channel.IsCategory)
            {
                bool newState = !channel.Collapsed;
                for (int i = Source.IndexOf(channel);
                    i < Source.Count
                    && (Source[i] is BindableChannel bChannel)
                    && bChannel.ParentId == channel.Model.Id;
                    i++)
                {
                    bChannel.Collapsed = newState;
                }
            }
            else
            {
                Messenger.Default.Send(new ChannelNavigateMessage(channel.Model.Id, GuildId));
            }
        }));
    }
}
