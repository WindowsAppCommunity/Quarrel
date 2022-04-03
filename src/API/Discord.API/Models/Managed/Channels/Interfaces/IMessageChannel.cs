// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IMessageChannel : IChannel
    {
        int? MentionCount { get; set; }

        ulong? LastMessageId { get; set; }

        ulong? LastReadMessageId { get; set; }

        bool IsUnread { get; }
    }
}
