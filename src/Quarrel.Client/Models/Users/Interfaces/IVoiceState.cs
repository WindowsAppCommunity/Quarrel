// Quarrel © 2022

using System;

namespace Quarrel.Client.Models.Users.Interfaces
{
    internal interface IVoiceState
    {
        bool IsDeafened { get; }

        bool IsMuted { get; }

        bool IsSelfDeafened { get; }

        bool IsSelfMuted { get; }

        bool IsSuppressed { get; }

        string VoiceSessionId { get; }

        bool IsStreaming { get; }

        bool IsVideoing { get; }

        DateTimeOffset? RequestToSpeakTimestamp { get; }
    }
}
