// Adam Dernis © 2022

using Discord.API.Models.Base;

namespace Discord.API.Models.Base
{
    public abstract class SnowflakeItem : DiscordItem
    {
        protected SnowflakeItem(DiscordClient context) :
            base(context)
        {
        }

        /// <inheritdoc/>
        public ulong Id { get; protected set; }
    }
}
