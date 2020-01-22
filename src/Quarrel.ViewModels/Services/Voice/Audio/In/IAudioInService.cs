using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Voice.Audio.In
{
    public interface IAudioInService : IAudioService
    {
        event EventHandler<float[]> InputRecieved;
        event EventHandler<int> SpeakingChanged;

        void Mute();
        void Unmute();
        void ToggleMute();

        bool Muted { get; }
    }
}
