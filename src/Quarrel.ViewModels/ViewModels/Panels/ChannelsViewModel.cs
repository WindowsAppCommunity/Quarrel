// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels.Panels
{
    public class ChannelsViewModel
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private ulong _currentGuildId;

        public ChannelsViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<BindableChannel>();

            _messenger.Register<NavigateToGuildMessage<BindableGuild>>(this, (_, m) => LoadChannels(m.Guild.Guild));
            _messenger.Register<NavigateToGuildMessage<Guild>>(this, (_, m) => LoadChannels(m.Guild));
        }

        public ObservableCollection<BindableChannel> Source { get; set; }

        public void LoadChannels(Guild guild)
        {
            if (guild.Id == _currentGuildId)
            {
                return;
            }

            _currentGuildId = guild.Id;
            var channels = _discordService.GetGuildChannelsHierarchy(guild);
            _dispatcherService.RunOnUIThread(() =>
            {
                Source.Clear();
                foreach (var channel in channels)
                {
                    Source.Add(channel);
                }
            });
        }
    }
}
