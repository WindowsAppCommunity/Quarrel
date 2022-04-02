// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IGuildTextChannel : IMessageChannel, INestedChannel
    {
        string? Topic { get; }

        bool? IsNSFW { get; }

        int? SlowModeDelay { get; }
    }
}
