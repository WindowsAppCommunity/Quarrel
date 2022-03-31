// Adam Dernis © 2022

using Discord.API.Models.Users;

namespace Discord.API.Models.Channels.Interfaces
{
    public interface IDirectChannel : IMessageChannel, IPrivateChannel
    {
        IUser Recipient { get; }
    }
}
