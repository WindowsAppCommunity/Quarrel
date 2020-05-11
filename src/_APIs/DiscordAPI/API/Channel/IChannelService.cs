using DiscordAPI.API.Channel.Models;
using DiscordAPI.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Channel
{
    public interface IChannelService
    {
        // GET
        [Get("/channels/{channelId}")]
        Task<GuildChannel> GetGuildChannel([AliasAs("channelId")] string channelId);

        [Get("/channels/{channelId}")]
        Task<DirectMessageChannel> GetDMChannel([AliasAs("channelId")] string channelId);

        [Put("/channels/{channelId}")]
        Task ModifyChannel([AliasAs("channelId")] string channelId, [Body] ModifyChannel modifyChannel);

        [Get("/v6/channels/{channelId}/messages")]
        Task DeleteChannelMessages([AliasAs("channelId")] string channelId);

        [Get("/v6/channels/{channelId}/messages/{messageId)")]
        Task<Message> GetChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Get("/v6/channels/{channelId}/messages?limit={limit}")]
        Task<IList<Message>> GetChannelMessages([AliasAs("channelId")] string channelId, [AliasAs("limit")] int limit = 50);

        [Get("/v6/channels/{channelId}/messages?limit={limit}&before={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesBefore([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        [Get("/v6/channels/{channelId}/messages?limit={limit}&after={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesAfter([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        [Get("/v6/channels/{channelId}/messages?limit={limit}&around={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesAround([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        [Get("/channels/{channelId}/pins")]
        Task<IEnumerable<Message>> GetPinnedMessages([AliasAs("channelId")] string channelId);

        [Get("/channels/{channelId}/invites")]
        Task<IEnumerable<DiscordAPI.Models.Invite>> GetChannelInvites([AliasAs("channelId")] string channelId);

        [Get("/v6/channels/{channelId}/webhooks")]
        Task<IEnumerable<Webhook>> GetWebhooks([AliasAs("channelId")] string channelId);

        [Get("/v6/channels/{channelId}/call")]
        Task StartCall([AliasAs("channelId")] string channelId);

        // PUT
        [Put("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task CreateReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        [Put("/channels/{channelId}/permissions/{overwriteId}")]
        Task EditChannelPermissions([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId, [Body] EditChannelPermissions editChannel);

        [Put("/channels/{channelId}/pins/{messageId}")]
        Task AddPinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        // POST
        [Post("/channels/{channelId}/messages")]
        Task<Message> CreateMessage([AliasAs("channelId")] string channelId, [Body] MessageUpsert message);

        [Post("/channels/{channelId}/messages")]
        [Multipart]
        Task<Message> UploadFile([AliasAs("channelId")] string channelId, StreamPart file);

        [Post("/channels/{channelId}/messages/bulk_delete")]
        Task BulkDeleteMessages([AliasAs("channelId")] string channelId, [Body] BulkDelete messages);

        [Post("/v6/channels/{channelId}/messages/{messageId}/ack")]
        [Headers("Content-Type: application/json;")]
        Task AckMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] string body = "{}");

        [Post("/channels/{channelId}/invites")]
        Task<DiscordAPI.Models.Invite> CreateChannelInvite([AliasAs("channelId")] string channelid, [Body] CreateInvite invite);

        [Post("/v6/channels/{channelId}/typing")]
        Task TriggerTypingIndicator([AliasAs("channelId")] string channelId);

        [Post("/v6/channels/{channelId}/webhooks")]
        Task<Webhook> CreateWebhook([AliasAs("channelId")] string channelId, [Body] Webhook webhook);

        // PATCH
        [Patch("/channels/{channelId}/messages/{messageId}")]
        Task<Message> EditMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] EditMessage editMessage);

        // DELETE
        [Delete("/channels/{channelId}/messages/{messageId}")]
        Task DeleteMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Delete("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task DeleteReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        [Delete("/channels/{channelId}/permissions/{overwriteId}")]
        Task DeleteChannelPermission([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId);

        [Delete("/channels/{channelId}/pins/{messageId}")]
        Task DeletePinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Delete("/channels/{channelId}")]
        Task DeleteChannel([AliasAs("channelId")] string channelId);

        [Delete("/channels/{channelId}/recipients/{userId}")]
        Task RemoveGroupUser([AliasAs("channelId")] string channelid, [AliasAs("userId")] string userId);
    }
}
