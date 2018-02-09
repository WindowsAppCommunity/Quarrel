using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using Discord_UWP.API.Channel.Models;
using Discord_UWP.SharedModels;

namespace Discord_UWP.API.Channel
{
    public interface IChannelService
    {
        [Get("/channels/{channelId}")]
        Task<GuildChannel> GetGuildChannel([AliasAs("channelId")] string channelId);

        [Get("/channels/{channelId}")]
        Task<DirectMessageChannel> GetDMChannel([AliasAs("channelId")] string channelId);

        [Put("/channels/{channelId}")]
        Task ModifyChannel([AliasAs("channelId")] string channelId, [Body] ModifyChannel modifyChannel);

        [Delete("/channels/{channelId}")]
        Task DeleteChannel([AliasAs("channelId")] string channelId);

        [Get("/channels/{channelId}/messages")]
        Task DeleteChannelMessages([AliasAs("channelId")] string channelId);

        [Get("/v6/channels/{channelId}/messages/{messageId)")]
        Task<Message> GetChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Get("/v6/channels/{channelId}/messages?limit={limit}")]
        Task<IEnumerable<Message>> GetChannelMessages([AliasAs("channelId")] string channelId, [AliasAs("limit")] int limit = 1000);

        [Get("/v6/channels/{channelId}/messages?limit=50&before={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesBefore([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Get("/v6/channels/{channelId}/messages?limit=50&after={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesAfter([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Post("/channels/{channelId}/messages")]
        Task<Message> CreateMessage([AliasAs("channelId")] string channelId, [Body] MessageUpsert message);

        [Put("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task CreateReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        // Set up Properly
        [Post("/channels/{channelId}/messages")]
        Task<Message> UploadFile([AliasAs("channelId")] string channelId);

        [Patch("/channels/{channelId}/messages/{messageId}")]
        Task<Message> EditMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] EditMessage editMessage);

        [Delete("/channels/{channelId}/messages/{messageId}")]
        Task DeleteMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Post("/channels/{channelId}/messages/bulk_delete")]
        Task BulkDeleteMessages([AliasAs("channelId")] string channelId, [Body] BulkDelete messages);

        [Delete("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task DeleteReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        [Post("/v6/channels/{channelId}/messages/{messageId}/ack")]
        [Headers("Content-Type: application/json;")]
        Task AckMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] string body = "{}");

        [Put("/channels/{channelId}/permissions/{overwriteId}")]
        Task EditChannelPermissions([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId, [Body] EditChannel editChannel);

        [Get("/channels/{channelId}/invites")]
        Task<IEnumerable<SharedModels.Invite>> GetChannelInvites([AliasAs("channelId")] string channelId);

        [Delete("/channels/{channelId}/permissions/{overwriteId}")]
        Task DeleteChannelPermission([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId);

        [Post("/v6/channels/{channelId}/typing")]
        Task TriggerTypingIndicator([AliasAs("channelId")] string channelId);

        [Get("/channels/{channelId}/pins")]
        Task<IEnumerable<Message>> GetPinnedMessages([AliasAs("channelId")] string channelId);

        [Put("/channels/{channelId}/pins/{messageId}")]
        Task AddPinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Delete("/channels/{channelId}/pins/{messageId}")]
        Task DeletePinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        [Post("/channels/{channelId}/invites")]
        Task<SharedModels.Invite> CreateChannelInvite([AliasAs("channelId")] string channelid, [Body] CreateInvite invite);

        [Delete("/channels/{channelId}/recipients/{userId}")]
        Task RemoveGroupUser([AliasAs("channelId")] string channelid, [AliasAs("userId")] string userId);

        [Post("/v6/channels/{channelId}/call/ring")]
        Task<SharedModels.Invite> Call([AliasAs("channelId")] string channelid, [Body] CreateInvite invite);
    }
}
