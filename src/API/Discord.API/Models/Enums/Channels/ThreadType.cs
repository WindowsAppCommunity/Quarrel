// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Channels
{
    /// <summary>
    /// An enum representing a thread's type.
    /// </summary>
    /// <remarks>
    /// Values are the same as in <see cref="ChannelType"/>, however constained to threads.
    /// </remarks>
    public enum ThreadType : int
    {
        /// <inheritdoc cref="ChannelType.NewsThread"/>
        NewsThread = 10,

        /// <inheritdoc cref="ChannelType.PublicThread"/>
        PublicThread = 11,

        /// <inheritdoc cref="ChannelType.PrivateThread"/>
        PrivateThread = 12,
    }
}
