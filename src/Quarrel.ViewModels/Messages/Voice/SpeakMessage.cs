using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Voice.DownstreamEvents;

namespace Quarrel.Messages.Voice
{
    public sealed class SpeakMessage
    {
        public Speak EventData;

        public SpeakMessage(Speak eventData)
        {
            EventData = eventData;
        }
    }
}
