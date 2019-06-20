using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Navigation
{
    public sealed class ChannelNavigateMessage
    {
        public ChannelNavigateMessage(string channelId)
        {
            ChannelId = channelId;
        }

        public string ChannelId { get; }
    }
}
