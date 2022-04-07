// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.DispatcherService;
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

        public ObservableCollection<BindableChannel> Source { get; private set; }

        public void LoadChannels(Guild guild)
        {
            if (guild.Id == _currentGuildId)
            {
                return;
            }

            _currentGuildId = guild.Id;
            var channels = _discordService.GetChannels(guild);
            Source.Clear();
            _dispatcherService.RunOnUIThread(() =>
            {
                foreach (var channel in channels)
                {
                    Source.Add(channel);
                }
            });
        }
    }
}
