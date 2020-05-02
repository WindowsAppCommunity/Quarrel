using DiscordStatusAPI.Models;
using Refit;
using System.Threading.Tasks;

namespace DiscordStatusAPI.API.Status
{
    public interface IStatusService
    {
        [Get("/index.json")]
        Task<Index> GetStatus();

        [Get("/metrics-display/ztt4777v23lf/{duration}.json")]
        Task<AllMetrics> GetMetrics(string duration = "day");
    }
}
