using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayTypingStartedMessage
    {
        public GatewayTypingStartedMessage(TypingStart typingStart)
        {
            TypingStart = typingStart;
        }

        public TypingStart TypingStart { get; }
    }
}
