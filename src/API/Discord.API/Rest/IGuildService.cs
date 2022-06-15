// Quarrel © 2022

using Discord.API.Models.Json.Guilds;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IGuildService
    {
        [Patch("/guilds/{guildId}")]
        Task<JsonGuild> ModifyGuild([AliasAs("guildId")] ulong guildId, [Body] JsonModifyGuild modifyGuild);
    }
}
