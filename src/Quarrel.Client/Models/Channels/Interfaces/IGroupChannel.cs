// Quarrel © 2022

using Quarrel.Client.Models.Users.Interfaces;

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for group channels.
    /// </summary>
    public interface IGroupChannel : IPrivateChannel
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
