// Quarrel © 2022

using System;

namespace Discord.API.Voice
{
    internal class VoiceConnectionEventArgs<T> : EventArgs
    {
        public T? EventData { get; }

        public VoiceConnectionEventArgs(T? eventData)
        {
            EventData = eventData;
        }
    }
}
