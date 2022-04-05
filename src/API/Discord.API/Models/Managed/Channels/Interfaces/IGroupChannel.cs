// Adam Dernis © 2022

using Discord.API.Models.Users;

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for group channels.
    /// </summary>
    internal interface IGroupChannel : IPrivateChannel, IMessageChannel, IAudioChannel
    {
        /// <summary>
        /// The id of the user that owns the channel.
        /// </summary>
        ulong OwnerId { get; }

        /// <summary>
        /// The list of users in the channel.
        /// </summary>
        IUser[] Recipients { get; }
    }
}
