using Discord_UWP.Voice.DownstreamEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public class VoiceEventArgs<T> : EventArgs
    {
        public T EventData { get; }

        public VoiceEventArgs(T eventData)
        {
            EventData = eventData;
        }
    }

    class Voice
    {
        private delegate void VoiceEventHandler(VoiceEvent gatewayEvent);

        private Ready? lastReady;
        private VoiceEvent? lastVoiceEvent;


    }
}
