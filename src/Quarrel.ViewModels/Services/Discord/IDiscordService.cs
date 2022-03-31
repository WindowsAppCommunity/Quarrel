// Adam Dernis © 2022

using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public interface IDiscordService
    {
        Task LoginAsync(string token);
    }
}
