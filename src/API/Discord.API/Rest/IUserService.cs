// Quarrel © 2022

using Discord.API.Models.Json.Settings;
using Discord.API.Models.Json.Users;
using Refit;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal interface IUserService
    {
        [Patch("/users/@me/settings")]
        [Headers("Content-Type: application/json;")]
        Task UpdateSettings([Body] JsonModifyUserSettings settings);

        [Patch("/users/@me")]
        [Headers("Content-Type: application/json;")]
        Task ModifyMe([Body] JsonModifySelfUser modifyUser);
    }
}
