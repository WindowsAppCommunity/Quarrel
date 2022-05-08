// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Analytics;
using Quarrel.Services.Discord;

namespace Quarrel.ViewModels.Panels
{
    public class CommandBarViewModel : ObservableRecipient
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;

        private IBindableSelectableChannel? _selectedChannel;

        public CommandBarViewModel(IAnalyticsService analyticsService, IMessenger messenger, IDiscordService discordService)
        {
            _analyticsService = analyticsService;
            _messenger = messenger;
            _discordService = discordService;

            _messenger.Register<NavigateToChannelMessage<IBindableSelectableChannel>>(this, (_, m) => SelectedChannel = m.Channel);
        }

        public IBindableSelectableChannel? SelectedChannel
        {
            get => _selectedChannel;
            set => SetProperty(ref _selectedChannel, value);
        }
    }
}
