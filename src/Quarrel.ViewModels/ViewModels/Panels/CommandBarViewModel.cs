// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Analytics;
using Quarrel.Services.Discord;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the command bar.
    /// </summary>
    public class CommandBarViewModel : ObservableRecipient
    {
        private readonly ILoggingService _loggingService;
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;

        private IBindableSelectableChannel? _selectedChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBarViewModel"/> class.
        /// </summary>
        public CommandBarViewModel(ILoggingService loggingService, IMessenger messenger, IDiscordService discordService)
        {
            _loggingService = loggingService;
            _messenger = messenger;
            _discordService = discordService;

            StartCallCommand = new RelayCommand(StartCall);

            _messenger.Register<ChannelSelectedMessage<IBindableSelectableChannel>>(this, (_, m) => SelectedChannel = m.Channel);
        }

        /// <summary>
        /// Gets the selected channel.
        /// </summary>
        public IBindableSelectableChannel? SelectedChannel
        {
            get => _selectedChannel;
            private set => SetProperty(ref _selectedChannel, value);
        }

        public RelayCommand StartCallCommand { get; }

        private void StartCall()
        {
            if (SelectedChannel is BindablePrivateChannel privateChannel)
            {
                privateChannel.JoinCall();
            }
        }
    }
}
