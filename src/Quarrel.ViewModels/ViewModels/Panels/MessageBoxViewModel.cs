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
    public class MessageBoxViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private ulong? _channelId;
        private string _draftText;

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

        public RelayCommand SendMessageCommand { get; }

        public string DraftText
        {
            get => _draftText;
            set => SetProperty(ref _draftText, value);
        }

        public ulong? ChannelId
        {
            get => _channelId;
            set => SetProperty(ref _channelId, value);
        }

        public void SendMessage()
        {
            if (ChannelId.HasValue)
            {
                _discordService.SendMessage(ChannelId.Value, DraftText);
                DraftText = string.Empty;
            }
        }
    }
}
