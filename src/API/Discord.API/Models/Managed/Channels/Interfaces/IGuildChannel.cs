// Adam Dernis © 2022

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IGuildChannel : IChannel
    {
        int Position { get; }

        ulong GuildId { get; }
    }
}
