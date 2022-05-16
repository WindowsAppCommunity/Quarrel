// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the message box.
    /// </summary>
    public class MessageBoxViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private ulong? _channelId;
        private string? _draftText;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxViewModel"/> class.
        /// </summary>
        public MessageBoxViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            SendMessageCommand = new RelayCommand(SendMessage);

            _messenger.Register<NavigateToChannelMessage<IBindableSelectableChannel>>(this, (_, m) =>
            {
                ChannelId = m.Channel.Id;
            });
        }

        /// <summary>
        /// Gets a command that sends the drafted message.
        /// </summary>
        public RelayCommand SendMessageCommand { get; }

        /// <summary>
        /// Gets or sets the drafted text in the message box.
        /// </summary>
        public string? DraftText
        {
            get => _draftText;
            set => SetProperty(ref _draftText, value);
        }

        /// <summary>
        /// Gets the current channel id.
        /// </summary>
        public ulong? ChannelId
        {
            get => _channelId;
            private set => SetProperty(ref _channelId, value);
        }

        private void SendMessage()
        {
            if (ChannelId.HasValue && !string.IsNullOrEmpty(DraftText))
            {
                _discordService.SendMessage(ChannelId.Value, DraftText!);
                DraftText = string.Empty;
            }
        }
    }
}
