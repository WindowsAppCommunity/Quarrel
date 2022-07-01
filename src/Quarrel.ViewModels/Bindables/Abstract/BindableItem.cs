// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Client;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Abstract
{
    /// <summary>
    /// An item that can be bound to the UI and contains an <see cref="IDispatcherService"/>.
    /// </summary>
    public abstract class BindableItem : ObservableObject
    {
        /// <summary>
        /// Gets the <see cref="IMessenger"/> for the <see cref="BindableItem"/>.
        /// </summary>
        protected readonly IMessenger _messenger;

        /// <summary>
        /// Gets the <see cref="IDiscordService"/> for the <see cref="BindableItem"/>.
        /// </summary>
        protected readonly IDiscordService _discordService;

        /// <summary>
        /// Gets the <see cref="QuarrelClient"/> for the <see cref="BindableItem"/>.
        /// </summary>
        protected readonly QuarrelClient _quarrelClient;

        /// <summary>
        /// Gets an <see cref="IDispatcherService"/> that can run code on the UI Thread.
        /// </summary>
        protected readonly IDispatcherService _dispatcherService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableItem"/> class.
        /// </summary>
        public BindableItem(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _quarrelClient = quarrelClient;
            _dispatcherService = dispatcherService;
        }
    }
}
