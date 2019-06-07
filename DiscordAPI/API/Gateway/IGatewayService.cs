using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.SharedModels;

namespace Quarrel.API.Gateway
{
    public interface IGatewayConfigService
    {
        [Get("/gateway")]
        Task<GatewayConfig> GetGatewayConfig();
    }
}
