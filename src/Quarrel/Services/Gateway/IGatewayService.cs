using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Services.Gateway
{
    public interface IGatewayService
    {
        DiscordAPI.Gateway.Gateway Gateway { get; }

        void InitializeGateway([NotNull] string accessToken);
    }
}
