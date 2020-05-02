// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Gateway
{
    /// <summary>
    /// Manages all events from the Discord Gateway.
    /// </summary>
    public interface IGatewayService
    {
        /// <summary>
        /// Gets access to the raw Discord gateway.
        /// </summary>
        DiscordAPI.Gateway.Gateway Gateway { get; }

        /// <summary>
        /// Connects the Gateway.
        /// </summary>
        /// <param name="accessToken">Access Token for login.</param>
        /// <returns>Whether or not the gateway successfully connected.</returns>
        Task<bool> InitializeGateway([NotNull] string accessToken);
    }
}
