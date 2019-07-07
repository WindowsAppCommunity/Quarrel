using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Navigation;
using Quarrel.Models.Bindables;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services;
using DiscordAPI.Models;

namespace Quarrel.ViewModels
{
    public class ChannelViewModel : ViewModelBase
    {
        public ChannelViewModel()
        {
            Messenger.Default.Register<GuildNavigateMessage>(this, async m =>
            {
                if (Guild != m.Guild)
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        Guild = m.Guild;
                        Source.Clear();

                        var itemList = m.Guild.Channels;
                        foreach (var item in itemList)
                        {
                            Source.Add(item);
                        }
                    });
                }
            });

            Messenger.Default.Register<CurrentGuildRequestMessage>(this, m => m.ReportResult(Guild));
        }

        private BindableGuild _Guild;

        public BindableGuild Guild
        {
            get => _Guild;
            set => Set(ref _Guild, value);
        }

        public ObservableCollection<BindableChannel> Source { get; private set; } = new ObservableCollection<BindableChannel>();


        private RelayCommand<BindableChannel> navigateChannelCommand;

        public RelayCommand<BindableChannel> NavigateChannelCommand => navigateChannelCommand ?? (navigateChannelCommand = new RelayCommand<BindableChannel>(async (channel) =>
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
            else if (channel.IsVoiceChannel)
            {
                if (channel.Model is GuildChannel gChannel)
                    await ServicesManager.Discord.Gateway.Gateway.VoiceStatusUpdate(Guild.Model.Id, gChannel.Id, false, false);
            }
            else
            {
                Messenger.Default.Send(new ChannelNavigateMessage(channel, Guild));
            }
        }));
    }
}
