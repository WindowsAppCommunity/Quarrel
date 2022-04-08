// Adam Dernis © 2022

using Discord.API.Models.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;

namespace Quarrel.Bindables.Messages
{
    public partial class BindableMessage : SelectableItem
    {
        [ObservableProperty]
        private Message _message;

        public BindableMessage(Message message)
        {
            Message = message;
        }
    }
}
