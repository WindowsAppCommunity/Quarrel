// Quarrel © 2022

using Quarrel.Client.Models.Messages;

namespace Quarrel.Messages.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is updated.
    /// </summary>
    public class MessageUpdatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageUpdatedMessage"/> class.
        /// </summary>
        /// <param name="message">The message updated.</param>
        public MessageUpdatedMessage(Message message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the updated message.
        /// </summary>
        public Message Message { get; }
    }
}
