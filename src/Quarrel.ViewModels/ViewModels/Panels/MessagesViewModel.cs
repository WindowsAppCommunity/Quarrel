// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Messages;
using Quarrel.Client.Models.Messages;
using Quarrel.Messages.Discord.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Panels
{
    /// <summary>
    /// The view model for the message list in the app.
    /// </summary>
    public partial class MessagesViewModel : ObservableRecipient
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private bool _isLoading;
        private IBindableSelectableChannel? _selectedChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesViewModel"/> class.
        /// </summary>
        public MessagesViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableRangeCollection<BindableMessage>();
            IsLoading = false;

            _messenger.Register<NavigateToChannelMessage<IBindableSelectableChannel>>(this, (_, m) => SelectedChannel = m.Channel);
            _messenger.Register<MessageCreatedMessage>(this, (_, m) =>
            {
                if (SelectedChannel?.Id == m.Message.ChannelId)
                {
                    AppendMessage(m.Message);
                }
            });
        }

        /// <summary>
        /// The collection of loaded messages.
        /// </summary>
        public ObservableRangeCollection<BindableMessage> Source;

        public IBindableSelectableChannel? SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (SetProperty(ref _selectedChannel, value))
                {
                    LoadChannel(value);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not messages are loading.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private void LoadChannel(IBindableSelectableChannel? channel)
        {
            if (channel is IBindableMessageChannel messageChannel)
            {
                LoadInitialMessages(messageChannel);
            }
        }

        private void LoadInitialMessages(IBindableMessageChannel? channel)
        {
            Guard.IsNotNull(channel, nameof(channel));
            _dispatcherService.RunOnUIThread(async () =>
            {
                // Clear the messages and begin loading
                Source.Clear();
                IsLoading = true;

                // Load messages
                var messages = await _discordService.GetChannelMessagesAsync(channel);
                BindableMessage[] bindableMessages = new BindableMessage[messages.Length];
                bindableMessages[0] = new BindableMessage(_messenger, _discordService, _dispatcherService, messages[messages.Length-1]);
                for (int i = 1; i < messages.Length; i++)
                {
                    bindableMessages[i] = new BindableMessage(_messenger, _discordService, _dispatcherService, messages[messages.Length-1-i], messages[messages.Length-i]);
                }

                // Add messages to the UI and mark loading as finished
                Source.AddRange(bindableMessages);
                IsLoading = false;
            });
        }

        private void AppendMessage(Message message)
        {
            _dispatcherService.RunOnUIThread(() =>
            {
                Source.Add(new BindableMessage(_messenger, _discordService, _dispatcherService, message, Source.Count > 0 ? Source[Source.Count-1].Message : null));
            });
        }
    }
}
