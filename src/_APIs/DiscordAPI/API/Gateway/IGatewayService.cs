using DiscordAPI.Models;
using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Gateway
{
    public interface IGatewayConfigService
    {
        [Get("/gateway")]
        Task<GatewayConfig> GetGatewayConfig();
    }
}
