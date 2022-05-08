// Quarrel © 2022

using Discord.API.Models.Enums.Channels;
using Quarrel.Client.Models.Channels.Interfaces;

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
        /// Gets the channel type.
        /// </summary>
        public ChannelType Type { get; }

        /// <summary>
        /// Gets the id of the guild the channel belongs to, or null if a DM.
        /// </summary>
        public ulong? GuildId { get; }

        /// <summary>
        /// Gets the name of the channel as displayed.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets if the user has permission to open the channel.
        /// </summary>
        public bool IsAccessible { get; }
    }
}
