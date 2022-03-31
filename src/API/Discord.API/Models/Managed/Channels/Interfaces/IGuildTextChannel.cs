// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IGuildTextChannel : IMessageChannel, INestedChannel
    {
        string? Topic { get; }

        bool? IsNSFW { get; }

        int SlowModeDelay { get; }

        ulong? LastMessageId { get; }
    }
}
