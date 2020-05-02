using DiscordAPI.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Voice
{
    public interface IVoiceService
    {
        [Get("/voice/regions")]
        Task<IEnumerable<VoiceRegion>> ListVoiceRegions();
    }
}
