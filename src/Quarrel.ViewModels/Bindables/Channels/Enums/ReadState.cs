// Quarrel © 2022

namespace Quarrel.Bindables.Channels.Enums
{
    /// <summary>
    /// An enum for the displayed read state of a channel.
    /// </summary>
    public enum ReadState
    {
        /// <summary>
        /// The channel contains no unread messages.
        /// </summary>
        Read,

        /// <summary>
        /// The channel contains unread messages.
        /// </summary>
        Unread,

        /// <summary>
        /// The channel is muted.
        /// </summary>
        Muted,
    }
}
