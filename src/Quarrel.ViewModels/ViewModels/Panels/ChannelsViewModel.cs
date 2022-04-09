// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Panels
{
    public partial class ChannelsViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;

        private BindableGuild _currentGuild;
        private BindableChannel? _selectedChannel;

        private IEnumerable<BindableChannelGroup>? _groupedSource;

        public ChannelsViewModel(IMessenger messenger, IDiscordService discordService)
        {
            _messenger = messenger;
            _discordService = discordService;

            _messenger.Register<NavigateToGuildMessage>(this, (_, m) => LoadChannels(m.Guild));
        }

        public BindableChannel? SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (value is null || !value.IsTextChannel)
                    return;

                if (_selectedChannel is not null)
                {
                    _selectedChannel.IsSelected = false;
                }

                SetProperty(ref _selectedChannel, value);
                value.IsSelected = true;
                _currentGuild.SelectedChannel = value.Channel.Id;
                _messenger.Send(new NavigateToChannelMessage<BindableChannel>(value));
            }
        }

        public IEnumerable<BindableChannelGroup>? GroupedSource
        {
            get => _groupedSource;
            set => SetProperty(ref _groupedSource, value);
        }

        public void LoadChannels(BindableGuild guild)
        {
            if (guild == _currentGuild)
            {
                return;
            }

            _currentGuild = guild;
            var channels = _discordService.GetGuildChannelsGrouped(guild.Guild, out BindableGuildChannel? selected, guild.SelectedChannel);
            GroupedSource = channels;
            SelectedChannel = selected;
        }
    }
}
