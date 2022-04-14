// Quarrel © 2022

namespace Discord.API.Models.Enums.Channels
{
    /// <summary>
    /// An enum representing a channel's type.
    /// </summary>
    public enum ChannelType : int
    {
        /// <summary>
        /// A text channel in a guild.
        /// </summary>
        GuildText = 0,

        /// <summary>
        /// A text/call channel between two people in DMs.
        /// </summary>
        DirectMessage = 1,

        /// <summary>
        /// A voice channel in a guild.
        /// </summary>
        GuildVoice = 2,

        /// <summary>
        /// A text/call channel for a group of people in DMs.
        /// </summary>
        GroupDM = 3,

        /// <summary>
        /// A category containing other channels in a guild.
        /// </summary>
        Category = 4,

        /// <summary>
        /// A news channel in a guild.
        /// </summary>
        News = 5,

        /// <summary>
        /// A store channel.
        /// </summary>
        /// <remarks>
        /// I'm unsure what this is.
        /// </remarks>
        Store = 6,

        /// <summary>
        /// A news thread.
        /// </summary>
        NewsThread = 10,

        /// <summary>
        /// A public thread under a channel in a guild.
        /// </summary>
        PublicThread = 11,

        /// <summary>
        /// A private thread under a channel in a guild.
        /// </summary>
        PrivateThread = 12,

        /// <summary>
        /// A voice channel in a guild with staging options.
        /// </summary>
        StageVoice = 13,

        /// <summary>
        /// A channel that marks a directory of servers.
        /// </summary>
        Directory = 14,
    }
}
