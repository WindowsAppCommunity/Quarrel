// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Client.Models.Channels
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
