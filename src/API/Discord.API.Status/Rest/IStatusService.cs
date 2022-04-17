using Discord.API.Status.Models;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Status.Rest
{
    public interface IStatusService
    {
        [Get("/index.json")]
        Task<Index> GetStatus();

        [Get("/metrics-display/ztt4777v23lf/{duration}.json")]
        Task<AllMetrics> GetMetrics(string duration = "day");
    }
}
