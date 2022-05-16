// Quarrel © 2022

using Discord.API.Models.Json.Settings;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IUserService
    {
        [Patch("/v6/users/@me/settings")]
        [Headers("Content-Type: application/json;")]
        Task UpdateSettings([Body] JsonModifyUserSettings settings);
    }
}
