using Refit;
using System.Threading.Tasks;

namespace DiscordAPI.API.Connections
{
    public interface IConnectionsService
    {
        [Get("/v6/connections/{service}/authorize")]
        Task<Connection> GetOauthUrl([AliasAs("service")] string service);
    }
}
