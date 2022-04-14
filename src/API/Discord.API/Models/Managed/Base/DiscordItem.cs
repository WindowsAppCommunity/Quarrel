// Quarrel © 2022

namespace Discord.API.Models.Base
{
    /// <summary>
    /// A base class for Discord items in the context of a client.
    /// </summary>
    public abstract class DiscordItem
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="DiscordClient"/> class.
        /// </summary>
        protected DiscordItem(DiscordClient context)
        {
            Context = context;
        }

        /// <summary>
        /// The <see cref="DiscordClient"/> containing the <see cref="DiscordItem"/>.
        /// </summary>
        protected DiscordClient Context { get; }
    }
}
