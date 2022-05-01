// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Messages.Embeds;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Messages.Embeds
{
    public class BindableAttachment : BindableItem
    {
        private Attachment _attachment;

        internal BindableAttachment(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Attachment attachment) :
            base(messenger, discordService, dispatcherService)
        {
            Attachment = attachment;
        }

        public Attachment Attachment
        {
            get => _attachment;
            set => SetProperty(ref _attachment, value);
        }
    }
}
