using DiscordAPI.Gateway.DownstreamEvents;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewaySessionReplacedMessage
    {
        public SessionReplace[] Session { get; }

        public GatewaySessionReplacedMessage(SessionReplace[] session)
        {
            Session = session;
        }
    }
}
