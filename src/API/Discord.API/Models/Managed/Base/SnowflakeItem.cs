// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;

namespace Discord.API.Models.Base
{
    public abstract class SnowflakeItem : ISnowflakeItem
    {
        /// <inheritdoc/>
        public ulong Id { get; private set; }
    }
}
