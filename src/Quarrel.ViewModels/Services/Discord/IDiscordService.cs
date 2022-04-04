// Adam Dernis © 2022

using Quarrel.Models.Bindables;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public interface IDiscordService
    {
        Task LoginAsync(string token);

        BindableGuild[] GetMyGuilds();
    }
}
