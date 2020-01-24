using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayVoiceServerUpdateMessage
    {
        public GatewayVoiceServerUpdateMessage(VoiceServerUpdate voiceServer)
        {
            VoiceServer = voiceServer;
        }

        public VoiceServerUpdate VoiceServer { get; }
    }
}
