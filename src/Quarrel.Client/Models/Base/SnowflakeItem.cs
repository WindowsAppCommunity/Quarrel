// Quarrel © 2022

using Quarrel.Client.Models.Base.Interfaces;

namespace Quarrel.Client.Models.Base
{
    /// <summary>
    /// A base class for <see cref="DiscordItem"/>s that use a snowflake for an id.
    /// </summary>
    public abstract class SnowflakeItem : DiscordItem, ISnowflakeItem
    {
        /// <summary>
        /// Create a new instance of a <see cref="SnowflakeItem"/> class.
        /// </summary>
        protected SnowflakeItem(DiscordClient context) :
            base(context)
        {
        }

        /// <inheritdoc/>
        public ulong Id { get; protected set; }
    }
}
