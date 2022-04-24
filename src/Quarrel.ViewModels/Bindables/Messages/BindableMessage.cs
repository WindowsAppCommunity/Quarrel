// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Messages;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Messages
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Messages.Message"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableMessage : SelectableItem
    {
        [ObservableProperty]
        private Message _message;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(IDiscordService discordService, IDispatcherService dispatcherService, Message message) :
            base(discordService, dispatcherService)
        {
            _message = message;
        }
    }
}
