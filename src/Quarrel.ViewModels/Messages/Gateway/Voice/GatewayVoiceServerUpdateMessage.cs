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
