// Adam Dernis © 2022

using Discord.API.Models.Json.Messages;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IChannelService
    {
        [Get("/v9/channels/{channelId}/messages?limit={limit}")]
        Task<JsonMessage[]> GetChannelMessages([AliasAs("channelId")] ulong channelId, [AliasAs("limit")] int limit = 50);
    }
}
