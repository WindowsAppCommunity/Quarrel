// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface INestedChannel : IGuildChannel
    {
        ulong? CategoryId { get; }
    }
}
