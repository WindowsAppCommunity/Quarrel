using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
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
