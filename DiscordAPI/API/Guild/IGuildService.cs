using Discord_UWP.API.Guild.Models;
using Discord_UWP.SharedModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.API.Guild
{
    public interface IGuildService
    {
        [Post("/guilds")]
        Task<SharedModels.Guild> CreateGuild([Body] CreateGuild createGuild);

        [Get("/guilds/{guildId}")]
        Task<SharedModels.Guild> GetGuild([AliasAs("guildId")] string guildId);

        [Patch("/guilds/{guildId}")]
        Task<SharedModels.Guild> ModifyGuild([AliasAs("guildId")] string guildId, [Body] ModifyGuild modifyGuild);

        [Delete("/guilds/{guildId}")]
        Task<SharedModels.Guild> DeleteGuild([AliasAs("guildId")] string guildId);

        [Get("/guilds/{guildId}/channels")]
        Task<IEnumerable<GuildChannel>> GetGuildChannels([AliasAs("guildId")] string guildId);

        [Post("/guilds/{guildId}/channels")]
        Task<GuildChannel> CreateGuildChannel([AliasAs("guildId")] string guildId, [Body] CreateGuildChannel createGuildChannel);

        [Patch("/guilds/{guildId}/channels")]
        Task<IEnumerable<GuildChannel>> ModifyGuildChannel([AliasAs("guildId")] string guildId, [Body] ModifyGuildChannel modifyGuildChannel);

       // [Patch("/guilds/{guildId}/channels")]
       // Task<IEnumerable<GuildChannel>> ModifyGuildChannelPositions([AliasAs("id")] string Id, [AliasAs("position")] int Position);

        [Get("/guilds/{guildId}/members/{userId}")]
        Task<GuildMember> GetGuildMemeber([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);

        [Get("/guilds/{guildId}/members")]
        Task<IEnumerable<GuildMember>> ListGuildMemebers([AliasAs("guildId")] string guildId, [AliasAs("limit")] int limit = 1000, [AliasAs("after")] int after = 0);

        [Patch("/guilds/{guildId}/members/{userId}")]
        Task ModifyGuildMember([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId, [Body] ModifyGuildMember modifyGuildMember);

        [Delete("/guilds/{guildId}/members/{userId}")]
        Task RemoveGuildMember([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);

        [Get("/guilds/{guildId}/bans")]
        Task<IEnumerable<SharedModels.User>> GetGuildBans([AliasAs("guildId")] string guildId);

        [Put("/guilds/{guildId}/bans/{userId}")]
        Task CreateGuildBan([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId, [Body] CreateGuildBan createGuildBan);

        [Delete("/guilds/{guildId}/bans/{userId}")]
        Task RemoveGuildBan([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);

        [Get("/guilds/{guildId}/roles")]
        Task<IEnumerable<Role>> GetGuildRoles([AliasAs("guildId")] string guildId);

        [Post("/guilds/{guildId}/roles")]
        Task<Role> CreateGuildRole([AliasAs("guildId")] string guildId);

        [Patch("/guilds/{guildId}/roles")]
        Task<IEnumerable<Role>> BatchModifyGuildRole([AliasAs("guildId")] string guildId, [Body] IEnumerable<ModifyGuildRole> modifyGuildRoles);

        [Patch("/guilds/{guildId}/roles/{roleId}")]
        Task<Role> ModifyGuildRole([AliasAs("guildId")] string guildId, [AliasAs("roleId")] string roleId, [Body] ModifyGuildRole modifyGuildRole);

        [Delete("/guilds/{guildId}/roles/{roleId}")]
        Task<Role> DeleteGuildRole([AliasAs("guildId")] string guildId, [AliasAs("roleId")] string roleId);

        [Get("/guilds/{guildId}/prune")]
        Task<PruneStatus> GetGuildPruneCount([AliasAs("guildId")] string guildId, [AliasAs("days")] int daysToPrune);

        [Post("/guilds/{guildId}/prune")]
        Task<PruneStatus> BeginGuildPrune([AliasAs("guildId")] string guildId, [AliasAs("days")] int daysToPrune);

        [Get("/guilds/{guildId}/regions")]
        Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegions([AliasAs("guildId")] string guildId);

        [Get("/guilds/{guildId}/invites")]
        Task<IEnumerable<SharedModels.Invite>> GetGuildInvites([AliasAs("guildId")] string guildId);

        [Get("/guilds/{guildId}/integrations")]
        Task<IEnumerable<Integration>> GetGuildIntegrations([AliasAs("guildId")] string guildId);

        [Post("/guilds/{guildId}/integrations")]
        Task CreateGuildIntegration([AliasAs("guildId")] string guildId, [Body] CreateIntegration createIntegration);

        [Patch("/guilds/{guildId}/integrations/{integrationId}")]
        Task ModifyGuildIntegration([AliasAs("guildId")] string guildId, [AliasAs("integrationId")] string integrationId, [Body] ModifyIntegration modifyIntegration);

        [Delete("/guilds/{guildId}/integrations/{integrationId}")]
        Task DeleteGuildIntegration([AliasAs("guildId")] string guildId, [AliasAs("integrationId")] string integrationId);

        [Post("/guilds/{guildId}/integrations/{integrationId}")]
        Task SyncGuildIntegration([AliasAs("guildId")] string guildId, [AliasAs("integrationId")] string integrationId);

        [Get("/guilds/{guildId}/embed")]
        Task<GuildEmbed> GetGuildEmbed([AliasAs("guildId")] string guildId);

        [Get("/guilds/{guildId}/embed")]
        Task<GuildEmbed> ModifyGuildEmbed([AliasAs("guildId")] string guildId, [Body] GuildEmbed guildEmbed);
    }
}
