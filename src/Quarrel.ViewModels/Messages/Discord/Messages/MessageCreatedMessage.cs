// Quarrel © 2022

using Quarrel.Bindables.Messages;

namespace Quarrel.Messages.Discord.Messages
{
    public class MessageCreatedMessage
    {
        public MessageCreatedMessage(BindableMessage message)
        {
            Message = message;
        }

        public BindableMessage Message { get; }
    }
}
