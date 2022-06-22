// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for an audio channel call panel.
    /// </summary>
    public class VoicePanelViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;
        private IBindableAudioChannel? _selectedChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoicePanelViewModel"/> class.
        /// </summary>
        public VoicePanelViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            _messenger.Register<ChannelSelectedMessage<IBindableSelectableChannel>>(this, (_, m) => SelectedChannel = m.Channel as IBindableAudioChannel);
        }

        /// <summary>
        /// Gets the currently selected channel.
        /// </summary>
        public IBindableAudioChannel? SelectedChannel
        {
            get => _selectedChannel;
            set => SetProperty(ref _selectedChannel, value);
        }
    }
}
