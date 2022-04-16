// Quarrel © 2022

namespace Quarrel.Client.Models.Base
{
    /// <summary>
    /// A base class for Discord items in the context of a client.
    /// </summary>
    public abstract class DiscordItem
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="QuarrelClient"/> class.
        /// </summary>
        protected DiscordItem(QuarrelClient context)
        {
            Context = context;
        }

        /// <summary>
        /// The <see cref="QuarrelClient"/> containing the <see cref="DiscordItem"/>.
        /// </summary>
        protected QuarrelClient Context { get; }
    }
}
