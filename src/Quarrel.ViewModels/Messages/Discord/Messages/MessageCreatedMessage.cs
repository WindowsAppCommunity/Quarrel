// Quarrel © 2022

using Quarrel.Bindables.Messages;
using Quarrel.Client.Models.Messages;

namespace Quarrel.Messages.Discord.Messages
{
    public class MessageCreatedMessage
    {
        public MessageCreatedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
