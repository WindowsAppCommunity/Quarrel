// Adam Dernis © 2022

using Discord.API.Models.Channels.Interfaces;

namespace Quarrel.Bindables.Channels.Interfaces
{
    /// <summary>
    /// An interface for an <see cref="IChannel"/> in a bindable context.
    /// </summary>
    public interface IBindableChannel
    {
        /// <summary>
        /// Gets the id of the channel.
        /// </summary>
        public ulong Id { get; }
        
        /// <summary>
        /// Gets if the user has permission to open the channel.
        /// </summary>
        public bool IsAccessible { get; }
    }
}
