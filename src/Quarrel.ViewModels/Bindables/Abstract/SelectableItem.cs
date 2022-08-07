// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Interfaces;
using Quarrel.Client;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Abstract
{
    /// <summary>
    /// A base class for items that can be selected in a bindable context.
    /// </summary>
    public abstract class SelectableItem : BindableItem, ISelectableItem
    {
        private bool _isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem"/> class.
        /// </summary>
        public SelectableItem(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
        }

        /// <summary>
        /// Gets or sets whether or not the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
