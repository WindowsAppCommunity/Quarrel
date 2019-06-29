using DiscordAPI.Models;
using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Navigation
{
    public sealed class ChannelNavigateMessage
    {
        public ChannelNavigateMessage(BindableChannel channel, BindableGuild guild)
        {
            Channel = channel;
            Guild = guild;
        }

        public BindableChannel Channel { get; }

        public BindableGuild Guild { get; }
    }
}
