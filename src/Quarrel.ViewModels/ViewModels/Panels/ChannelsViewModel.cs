// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Discord;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the channel list in the app.
    /// </summary>
    public partial class ChannelsViewModel : ObservableRecipient
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;

        private IBindableSelectableGuildItem? _currentGuild;
        private IBindableSelectableChannel? _selectedChannel;

        private IEnumerable<BindableChannelGroup>? _groupedSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelsViewModel"/> class.
        /// </summary>
        public ChannelsViewModel(IAnalyticsService analyticsService, IMessenger messenger, IDiscordService discordService)
        {
            _analyticsService = analyticsService;
            _messenger = messenger;
            _discordService = discordService;

            _messenger.Register<SelectChannelMessage<IBindableSelectableChannel>>(this, (_, m) => SelectedChannel = m.Channel);
            _messenger.Register<GuildSelectedMessage<IBindableSelectableGuildItem>>(this, (_, m) => LoadChannels(m.Guild));
        }

        /// <summary>
        /// Gets or sets the selected channel.
        /// </summary>
        public IBindableSelectableChannel? SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (value is null || _currentGuild is null)
                    return;

                if (_selectedChannel is not null)
                {
                    _selectedChannel.IsSelected = false;
                }

                if(SetProperty(ref _selectedChannel, value))
                {
                    value.IsSelected = true;
                    _currentGuild.SelectedChannelId = value.Id;

                    _analyticsService.Log(LoggedEvent.ChannelOpened,
                        ("Type", $"{_selectedChannel.Type}"));

                    _messenger.Send(new ChannelSelectedMessage<IBindableSelectableChannel>(value));
                }
            }
        }

        /// <summary>
        /// Gets the grouped channels loaded in the guild.
        /// </summary>
        public IEnumerable<BindableChannelGroup>? GroupedSource
        {
            get => _groupedSource;
            private set => SetProperty(ref _groupedSource, value);
        }

        /// <summary>
        /// Loads the channels for a guild.
        /// </summary>
        /// <param name="guild">The guild to load.</param>
        public void LoadChannels(IBindableSelectableGuildItem guild)
        {
            if (guild == _currentGuild)
            {
                return;
            }

            _currentGuild = guild;
            var channels = guild.GetGroupedChannels(out IBindableSelectableChannel? selected);
            GroupedSource = channels;
            SelectedChannel = selected;
        }
    }
}
