// Quarrel © 2022

using Discord.API.Models.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;

namespace Quarrel.Bindables.Messages
{
    /// <summary>
    /// A wrapper of a <see cref="Discord.API.Models.Messages.Message"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableMessage : SelectableItem
    {
        [ObservableProperty]
        private Message _message;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(Message message)
        {
            _message = message;
        }
    }
}
