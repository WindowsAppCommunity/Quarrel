// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Messages;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
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

            _messenger.Register<NavigateToChannelMessage>(this, (_, m) => LoadChannel(m.Channel));
        }

        /// <summary>
        /// The collection of loaded messages.
        /// </summary>
        public ObservableRangeCollection<BindableMessage> Source;

        /// <summary>
        /// Gets a value indicating whether or not messages are loading.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        private void LoadChannel(IBindableSelectableChannel channel)
        {
            if (channel is IBindableMessageChannel messageChannel)
            {
                LoadInitialMessages(messageChannel.MessageChannel);
            }
        }

        private void LoadInitialMessages(IMessageChannel? channel)
        {
            Guard.IsNotNull(channel, nameof(channel));
            _dispatcherService.RunOnUIThread(async () =>
            {
                // Clear the messages and begin loading
                Source.Clear();
                IsLoading = true;

                // Load messages
                var messages = await _discordService.GetChannelMessagesAsync(channel);
                
                // Add messages to the UI and mark loading as finished
                Source.AddRange(messages.Reverse());
                IsLoading = false;
            });
        }
    }
}
