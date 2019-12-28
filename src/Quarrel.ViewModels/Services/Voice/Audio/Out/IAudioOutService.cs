using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Services.Voice.Audio.Out
{
    public interface IAudioOutService : IAudioService
    {
        unsafe void AddFrame(float[] framedata, uint samples);

        void Deafen();
        void Undeafen();
        void ToggleDeafen();

        bool Deafened { get; }
    }
}
