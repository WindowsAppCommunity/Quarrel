// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Voice;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IChannelService
    {
        [Post("/v9/channels/{channelId}/messages/{messageId}/ack")]
        [Headers("Content-Type: application/json;")]
        Task MarkRead([AliasAs("channelId")] ulong channelId, [AliasAs("messageId")] ulong messageId, [Body] JsonMarkRead markRead);

        [Get("/v9/channels/{channelId}/messages?limit={limit}")]
        Task<JsonMessage[]> GetChannelMessages([AliasAs("channelId")] ulong channelId, [AliasAs("limit")] int limit = 50);

        [Get("/v9/channels/{channelId}/messages?limit={limit}&before={before}")]
        Task<JsonMessage[]> GetChannelMessagesBefore([AliasAs("channelId")] ulong channelId, [AliasAs("before")] ulong before, [AliasAs("limit")] int limit = 50);

        [Post("/v9/channels/{channelId}/messages")]
        Task<JsonMessage> CreateMessage([AliasAs("channelId")] ulong channelId, [Body] JsonMessageUpsert message);

        [Delete("/v9/channels/{channelId}/messages/{messageId}")]
        Task DeleteMessage([AliasAs("channelId")] ulong channelId, [AliasAs("messageId")] ulong messageId);

        [Post("/v9/channels/{channelId}/call/ring")]
        [Headers("Content-Type: application/json;")]
        Task StartCall([AliasAs("channelId")] ulong channelId, [Body] JsonRecipients recipients);
    }
}
