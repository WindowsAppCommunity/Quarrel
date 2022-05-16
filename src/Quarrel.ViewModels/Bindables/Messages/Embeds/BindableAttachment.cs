// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Messages.Embeds;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Text.RegularExpressions;

namespace Quarrel.Bindables.Messages.Embeds
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Messages.Embeds.Attachment"/> that can be bound to the UI.
    /// </summary>
    public class BindableAttachment : BindableItem
    {
        private const string FileExtensionRegex = @"\.([\w]+)$";

        private Attachment _attachment;

        internal BindableAttachment(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Attachment attachment) :
            base(messenger, discordService, dispatcherService)
        {
            _attachment = attachment;
        }

        /// <summary>
        /// Gets the wrapped <see cref="Client.Models.Messages.Embeds.Attachment"/>.
        /// </summary>
        public Attachment Attachment
        {
            get => _attachment;
            private set => SetProperty(ref _attachment, value);
        }

        /// <summary>
        /// Gets the file's extension.
        /// </summary>
        public string FileExtension
        {
            get
            {
                var match = Regex.Match(Attachment.Filename, FileExtensionRegex);
                return match.Groups[1].Value;
            }
        }
    }
}
