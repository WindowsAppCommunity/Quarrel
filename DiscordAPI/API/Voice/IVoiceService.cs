using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Voice
{
    public interface IVoiceService
    {
        [Get("/voice/regions")]
        Task<IEnumerable<VoiceRegion>> ListVoiceRegions();
    }
}
