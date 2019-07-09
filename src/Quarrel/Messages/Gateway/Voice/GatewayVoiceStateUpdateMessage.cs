using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Gateway.DownstreamEvents;
using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayVoiceStateUpdateMessage
    {
        public GatewayVoiceStateUpdateMessage(VoiceState voiceState)
        {
            VoiceState = voiceState;
        }

        public VoiceState VoiceState { get; }
    }
}
