// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    public interface INestedChannel : IGuildChannel
    {
        ulong CategoryId { get; }
    }
}
