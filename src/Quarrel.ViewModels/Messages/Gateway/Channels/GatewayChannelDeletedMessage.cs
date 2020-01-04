using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayChannelDeletedMessage
    {
        public GatewayChannelDeletedMessage(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }
    }
}
