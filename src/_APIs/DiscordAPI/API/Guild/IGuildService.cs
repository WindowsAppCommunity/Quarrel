// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Guild
{
    /// <summary>
    /// A service for guild REST calls.
    /// </summary>
    public interface IGuildService
    {
        [Post("/v6/guilds")]
        Task<DiscordAPI.Models.Guilds.Guild> CreateGuild([Body] CreateGuild createGuild);

        [Get("/guilds/{guildId}")]
        Task<DiscordAPI.Models.Guilds.Guild> GetGuild([AliasAs("guildId")] string guildId);

        [Patch("/v6/guilds/{guildId}")]
        Task<DiscordAPI.Models.Guilds.Guild> ModifyGuild([AliasAs("guildId")] string guildId, [Body] ModifyGuild modifyGuild);

        [Delete("/guilds/{guildId}")]
        Task<DiscordAPI.Models.Guilds.Guild> DeleteGuild([AliasAs("guildId")] string guildId);

        [Get("/guilds/{guildId}/channels")]
        Task<IEnumerable<GuildChannel>> GetGuildChannels([AliasAs("guildId")] string guildId);

        [Post("/guilds/{guildId}/channels")]
        Task<GuildChannel> CreateGuildChannel([AliasAs("guildId")] string guildId, [Body] CreateGuildChannel createGuildChannel);

        [Post("/guilds/{guildId}/ack")]
        Task AckGuild([AliasAs("guildId")] string guildId);

        [Patch("/guilds/{guildId}/channels")]
        Task<IEnumerable<GuildChannel>> ModifyGuildChannel([AliasAs("guildId")] string guildId, [Body] ModifyGuildChannel modifyGuildChannel);

        [Patch("/guilds/{guildId}/channels")]
        Task<IEnumerable<GuildChannel>> ModifyGuildChannelPositions([AliasAs("guildId")] string guildId, [AliasAs("position")] int Position);

        [Patch("/guilds/{guildId}/members/@me/nick")]
        Task<GuildMember> ModifyCurrentUserNickname([AliasAs("guildId")] string guildId, [Body] ModifyGuildMember member);

        [Get("/guilds/{guildId}/members/{userId}")]
        Task<GuildMember> GetGuildMemeber([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);

        [Get("/guilds/{guildId}/members")]
        Task<IEnumerable<GuildMember>> ListGuildMemebers([AliasAs("guildId")] string guildId, [AliasAs("limit")] int limit = 1000, [AliasAs("after")] int after = 0);

        [Patch("/v6/guilds/{guildId}/members/{userId}")]
        Task ModifyGuildMember([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId, [Body] ModifyGuildMember modifyGuildMember);

        [Patch("/v6/guilds/{guildId}/members/@me/nick")]
        Task ModifySelfNickname([AliasAs("guildId")] string guildId, [Body] IModifyGuildMember modifyGuildMember);

        [Patch("/v6/guilds/{guildId}/members/{userId}")]
        Task ModifyGuildMemberNickname([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId, [Body] IModifyGuildMember modifyGuildMember);

        [Delete("/guilds/{guildId}/members/{userId}")]
        Task RemoveGuildMember([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);

        [Get("/guilds/{guildId}/bans")]
        Task<IEnumerable<Ban>> GetGuildBans([AliasAs("guildId")] string guildId);

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
        Task<IEnumerable<DiscordAPI.Models.Invite>> GetGuildInvites([AliasAs("guildId")] string guildId);

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

        [Get("/v6/guilds/{guildId}/messages/search?{args}&{offset}")]
        Task<SearchResults> SearchGuildMessages([AliasAs("guildId")] string guildId, [AliasAs("args")] string args, [AliasAs("offset")] int offset);

        //[Get("/v6/guilds/{guildId}/audit-logs?limit={limit}&before={beforeId}")]
        //Task<AuditLog> GetAuditLog([AliasAs("guildId")] string guildId, [AliasAs("beforeId")] string beforeId, [AliasAs("limit")] int limit = 50);

        //[Get("/v6/guilds/{guildId}/audit-logs?limit={limit}")]
        //Task<AuditLog> GetAuditLog([AliasAs("guildId")] string guildId, [AliasAs("limit")] int limit = 50);

        //[Get("/v6/guilds/{guildId}/audit-logs?user_id={userId")]
        //Task<AuditLog> GetAuditLog([AliasAs("guildId")] string guildId, [AliasAs("userId")] string userId);


        [Get("/v6/guilds/{guildId}/audit-logs")]
        Task<AuditLog> GetAuditLog([AliasAs("guildId")] string guildId);

        //[Get("/v6/guilds/{guildId}/audit-logs?before={beforeId}")]
        //Task<AuditLog> GetAuditLog([AliasAs("guildId")] string guildId, [AliasAs("beforeId")] string allArgs);

        [Get("/v6/guilds/{guildId}/webhooks")]
        Task<List<Webhook>> GetWebhooks([AliasAs("guildId")] string guildId);


        [Patch("/v6/webhooks/{webhookId}")]
        Task<Webhook> ModifyWebhook([AliasAs("webhookId")] string webhookId, [Body] ModifyWebhook webhook);
        [Delete("/v6/webhooks/{webhookId}")]
        Task DeleteWebhook([AliasAs("webhookId")] string webhookId);
    }
}
