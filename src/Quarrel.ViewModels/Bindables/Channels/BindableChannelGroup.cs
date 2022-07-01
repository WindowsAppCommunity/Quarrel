// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Client;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper for a <see cref="BindableCategoryChannel"/> that contains all child channels in a <see cref="IGrouping{BindableCategoryChannel, BindableChannel}"/>
    /// </summary>
    public class BindableChannelGroup : BindableItem, IGrouping<BindableCategoryChannel?, BindableChannel?>
    {
        internal BindableChannelGroup(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            BindableCategoryChannel? key) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
            Key = key;            
            Children = new ObservableCollection<BindableChannel>();
        }

        /// <inheritdoc/>
        public BindableCategoryChannel? Key { get; }

        /// <summary>
        /// Gets the <see cref="BindableChannel"/>s that belong to the <see cref="BindableCategoryChannel"/>.
        /// </summary>
        public ObservableCollection<BindableChannel> Children { get; }

        /// <inheritdoc/>
        public IEnumerator<BindableChannel> GetEnumerator() => Children.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds a child to the <see cref="BindableChannelGroup"/>.
        /// </summary>
        /// <param name="child">The <see cref="BindableChannel"/> to add.</param>
        public void AddChild(BindableChannel child)
        {
            Children.Add(child);
        }
    }
}
