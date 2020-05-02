using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
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
