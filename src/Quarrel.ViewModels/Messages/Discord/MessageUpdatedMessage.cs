// Quarrel © 2022

using Quarrel.Client.Models.Messages;

namespace Quarrel.Messages.Discord
{
    public class MessageUpdatedMessage
    {
        public MessageUpdatedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
