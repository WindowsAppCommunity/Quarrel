// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Client;
using Quarrel.Client.Models.Messages;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Messages
{
    public class BindableReaction : SelectableItem
    {
        internal BindableReaction(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient client,
            IDispatcherService dispatcherService,
            Reaction reaction) :
            base(messenger, discordService, client, dispatcherService)
        {
            Reaction = reaction;
            IsSelected = reaction.Me;
        }

        public Reaction Reaction { get; }
    }
}
