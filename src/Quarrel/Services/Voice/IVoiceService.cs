// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Services.Settings.Enums;
using Quarrel.Services.Voice.Audio;

namespace Quarrel.Services.Voice
{
    public interface IVoiceService
    {
        IAudioService InAudioService { get; }
        IAudioService OutAudioService { get; }
    }
}
