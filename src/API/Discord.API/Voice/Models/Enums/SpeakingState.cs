// Quarrel © 2022

using System;

namespace Discord.API.Voice.Models.Enums
{
    [Flags]
    public enum SpeakingState : int
    {
        Microphone = 0x1,
        Soundshare = 0x2,
        Priority = 0x4,
    }
}
