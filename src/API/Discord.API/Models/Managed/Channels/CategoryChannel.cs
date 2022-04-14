// Quarrel © 2022

using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Managed.Channels.Abstract;

namespace Discord.API.Models.Channels
{
    /// <summary>
    /// A category channel managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class CategoryChannel : GuildChannel, ICategoryChannel
    {
        internal CategoryChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
            base(restChannel, guildId, context)
        {
        }
    }
}
