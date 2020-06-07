// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Channel.Models;
using DiscordAPI.Models;
using DiscordAPI.Models.Messages;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAPI.API.Channel
{
    /// <summary>
    /// A service for channel REST calls.
    /// </summary>
    public interface IChannelService
    {
        /// <summary>
        /// Get a channel by id via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>The channel by <paramref name="channelId"/>.</returns>
        [Get("/channels/{channelId}")]
        Task<DiscordAPI.Models.Channels.Channel> GetGuildChannel([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Modifies a channel by id via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="modifyChannel">Modified channel details.</param>
        /// <returns>A task.</returns>
        [Put("/channels/{channelId}")]
        Task ModifyChannel([AliasAs("channelId")] string channelId, [Body] ModifyChannel modifyChannel);

        /// <summary>
        /// Get a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <returns>The message.</returns>
        [Get("/v6/channels/{channelId}/messages/{messageId)")]
        Task<Message> GetChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        /// <summary>
        /// Get a channel's messages via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="limit">The number of messages to get.</param>
        /// <returns>A list of messages.</returns>
        [Get("/v6/channels/{channelId}/messages?limit={limit}")]
        Task<IList<Message>> GetChannelMessages([AliasAs("channelId")] string channelId, [AliasAs("limit")] int limit = 50);

        /// <summary>
        /// Get a channel's messages before a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message id to get from before.</param>
        /// <param name="limit">The number of messages to get.</param>
        /// <returns>A list of messages.</returns>
        [Get("/v6/channels/{channelId}/messages?limit={limit}&before={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesBefore([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        /// <summary>
        /// Get a channel's messages after a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message id to get from after.</param>
        /// <param name="limit">The number of messages to get.</param>
        /// <returns>A list of messages.</returns>
        [Get("/v6/channels/{channelId}/messages?limit={limit}&after={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesAfter([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        /// <summary>
        /// Get a channel's messages around a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message id to get from around.</param>
        /// <param name="limit">The number of messages to get.</param>
        /// <returns>A list of messages.</returns>
        [Get("/v6/channels/{channelId}/messages?limit={limit}&around={messageId}")]
        Task<IEnumerable<Message>> GetChannelMessagesAround([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("limit")] int limit = 50);

        /// <summary>
        /// Get a channel's pinned messages via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A list of messages.</returns>
        [Get("/channels/{channelId}/pins")]
        Task<IEnumerable<Message>> GetPinnedMessages([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Gets a channel's invites via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A list of invites.</returns>
        [Get("/channels/{channelId}/invites")]
        Task<IEnumerable<DiscordAPI.Models.Invite>> GetChannelInvites([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Gets a channel's webhooks via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A list of webhooks.</returns>
        [Get("/v6/channels/{channelId}/webhooks")]
        Task<IEnumerable<Webhook>> GetWebhooks([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Starts a call in the (DM) channel via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A task.</returns>
        [Get("/v6/channels/{channelId}/call")]
        Task StartCall([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Gets list of users who have reacted to a certain message with a certain emoji.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <param name="emoji">The emoji.</param>
        /// <returns>A task.</returns>
        [Get("/channels/{channelId}/messages/{messageId}/reactions/{emoji}")]
        Task<IEnumerable<DiscordAPI.Models.User>> GetReactions([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        /// <summary>
        /// Creates a reaction via REST.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="emoji">The emoji.</param>
        /// <returns>A task.</returns>
        [Put("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task CreateReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        /// <summary>
        /// Edit a channel's overwrite permissions.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="overwriteId">The overwrite's id.</param>
        /// <param name="editChannel">The new permissions.</param>
        /// <returns>A task.</returns>
        [Put("/channels/{channelId}/permissions/{overwriteId}")]
        Task EditChannelPermissions([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId, [Body] EditChannelPermissions editChannel);

        /// <summary>
        /// Pins a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <returns>A task.</returns>
        [Put("/channels/{channelId}/pins/{messageId}")]
        Task AddPinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        /// <summary>
        /// Create a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="message">The message upsert.</param>
        /// <returns>The message.</returns>
        [Post("/channels/{channelId}/messages")]
        Task<Message> CreateMessage([AliasAs("channelId")] string channelId, [Body] MessageUpsert message);

        /// <summary>
        /// Creates an attachment message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="file">The file stream.</param>
        /// <returns>The message.</returns>
        [Post("/channels/{channelId}/messages")]
        [Multipart]
        Task<Message> UploadFile([AliasAs("channelId")] string channelId, [AliasAs("content")]string content,
            StreamPart file1 = null, StreamPart file2 = null, StreamPart file3 = null, StreamPart file4 = null, StreamPart file5 = null,
            StreamPart file6 = null, StreamPart file7 = null, StreamPart file8 = null, StreamPart file9 = null, StreamPart file10 = null);

        /// <summary>
        /// Marks a message as read via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <param name="body">Raw body details.</param>
        /// <returns>A task.</returns>
        [Post("/v6/channels/{channelId}/messages/{messageId}/ack")]
        [Headers("Content-Type: application/json;")]
        Task AckMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] string body = "{}");

        /// <summary>
        /// Creates a channel invite via REST.
        /// </summary>
        /// <param name="channelid">The channel's id.</param>
        /// <param name="invite">The invite properties.</param>
        /// <returns>The invite.</returns>
        [Post("/channels/{channelId}/invites")]
        Task<DiscordAPI.Models.Invite> CreateChannelInvite([AliasAs("channelId")] string channelid, [Body] CreateInvite invite);

        /// <summary>
        /// Triggers the typing indicator via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A task.</returns>
        [Post("/v6/channels/{channelId}/typing")]
        Task TriggerTypingIndicator([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Creates a webhook via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="webhook">The webhook properties.</param>
        /// <returns>The new webhook.</returns>
        [Post("/v6/channels/{channelId}/webhooks")]
        Task<Webhook> CreateWebhook([AliasAs("channelId")] string channelId, [Body] Webhook webhook);

        /// <summary>
        /// Edits a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <param name="editMessage">New message body details.</param>
        /// <returns>The new message.</returns>
        [Patch("/channels/{channelId}/messages/{messageId}")]
        Task<Message> EditMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [Body] EditMessage editMessage);

        /// <summary>
        /// Deletes a message via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}/messages/{messageId}")]
        Task DeleteMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        /// <summary>
        /// Delete a reaction via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <param name="emoji">The emoji.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task DeleteReaction([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId, [AliasAs("emoji")] string emoji);

        /// <summary>
        /// Deletes a permissions overwrite from a channel via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="overwriteId">The overwrite's id.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}/permissions/{overwriteId}")]
        Task DeleteChannelPermission([AliasAs("channelId")] string channelId, [AliasAs("overwriteId")] string overwriteId);

        /// <summary>
        /// Removes a pinned message via REST.
        /// </summary>
        /// <param name="channelId">The channe's id.</param>
        /// <param name="messageId">The message's id.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}/pins/{messageId}")]
        Task DeletePinnedChannelMessage([AliasAs("channelId")] string channelId, [AliasAs("messageId")] string messageId);

        /// <summary>
        /// Deletes a channel via REST.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}")]
        Task DeleteChannel([AliasAs("channelId")] string channelId);

        /// <summary>
        /// Removes a user from a group via REST.
        /// </summary>
        /// <param name="channelid">The channel's id.</param>
        /// <param name="userId">The user's id.</param>
        /// <returns>A task.</returns>
        [Delete("/channels/{channelId}/recipients/{userId}")]
        Task RemoveGroupUser([AliasAs("channelId")] string channelid, [AliasAs("userId")] string userId);
    }
}
