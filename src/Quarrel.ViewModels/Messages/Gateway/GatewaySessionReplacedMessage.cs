using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
