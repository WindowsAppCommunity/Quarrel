// Adam Dernis © 2022

using Discord.API.Models.Guilds;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Panels
{
    public partial class ChannelsViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private ulong _currentGuildId;

        private IEnumerable<BindableChannelGroup>? _groupedSource;

        public ChannelsViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            _messenger.Register<NavigateToGuildMessage<BindableGuild>>(this, (_, m) => LoadChannels(m.Guild.Guild));
            _messenger.Register<NavigateToGuildMessage<Guild>>(this, (_, m) => LoadChannels(m.Guild));
        }

        public IEnumerable<BindableChannelGroup>? GroupedSource
        {
            get => _groupedSource;
            set => SetProperty(ref _groupedSource, value);
        }

        public void LoadChannels(Guild guild)
        {
            if (guild.Id == _currentGuildId)
            {
                return;
            }

            _currentGuildId = guild.Id;
            var channels = _discordService.GetGuildChannelsGrouped(guild);
            GroupedSource = channels;
        }
    }
}
