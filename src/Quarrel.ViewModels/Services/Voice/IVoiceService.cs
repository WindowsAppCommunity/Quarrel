// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Services.Settings.Enums;
using Quarrel.Services.Voice.Audio.In;
using Quarrel.Services.Voice.Audio.Out;

namespace Quarrel.Services.Voice
{
    public interface IVoiceService
    {
        IAudioInService AudioInService { get; }

        IAudioOutService AudioOutService { get; }

        IDictionary<string, VoiceState> VoiceStates { get; }
    }
}
