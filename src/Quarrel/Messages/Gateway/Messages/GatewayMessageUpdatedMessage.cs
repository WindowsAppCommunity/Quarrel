using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayMessageUpdatedMessage
    {
        public GatewayMessageUpdatedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
