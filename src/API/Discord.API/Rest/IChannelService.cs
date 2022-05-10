// Quarrel © 2022

using Discord.API.Models.Json.Messages;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IChannelService
    {
        [Get("/v9/channels/{channelId}/messages?limit={limit}")]
        Task<JsonMessage[]> GetChannelMessages([AliasAs("channelId")] ulong channelId, [AliasAs("limit")] int limit = 50);

        [Get("/v9/channels/{channelId}/messages?limit={limit}&before={before}")]
        Task<JsonMessage[]> GetChannelMessagesBefore([AliasAs("channelId")] ulong channelId, [AliasAs("before")] ulong before, [AliasAs("limit")] int limit = 50);

        [Post("/v9/channels/{channelId}/messages")]
        Task<JsonMessage> CreateMessage([AliasAs("channelId")] ulong channelId, [Body] JsonMessageUpsert message);
    }
}
