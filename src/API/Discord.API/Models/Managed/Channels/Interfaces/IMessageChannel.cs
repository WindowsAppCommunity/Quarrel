// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IMessageChannel
    {
        int? MentionCount { get; internal set; }

        ulong? LastReadMessageId { get; internal set; }
    }
}
