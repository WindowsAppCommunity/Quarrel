// Adam Dernis © 2022

using Discord.API.Models.Users;

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IGroupChannel : IMessageChannel, IPrivateChannel, IAudioChannel
    {
        ulong OwnerId { get; }

        IUser[] Recipients { get; }
    }
}
