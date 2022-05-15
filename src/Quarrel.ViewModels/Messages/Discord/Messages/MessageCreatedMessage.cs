// Quarrel © 2022

using Quarrel.Client.Models.Messages;

namespace Quarrel.Messages.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is created.
    /// </summary>
    public class MessageCreatedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCreatedMessage"/> class.
        /// </summary>
        /// <param name="message">The message created.</param>
        public MessageCreatedMessage(Message message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message created.
        /// </summary>
        public Message Message { get; }
    }
}
