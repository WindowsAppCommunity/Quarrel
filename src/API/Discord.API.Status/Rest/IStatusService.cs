using Discord.API.Status.Models;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Status.Rest
{
    /// <summary>
    /// An interface for a service that 
    /// </summary>
    public interface IStatusService
    {
        /// <summary>
        /// Gets the current discord server status.
        /// </summary>
        [Get("/index.json")]
        Task<Index> GetStatus();

        /// <summary>
        /// Gets recent historical metrics for Discord status.
        /// </summary>
        /// <param name="duration">The duration </param>
        [Get("/metrics-display/ztt4777v23lf/{duration}.json")]
        Task<AllMetrics> GetMetrics(string duration = "day");
    }
}
