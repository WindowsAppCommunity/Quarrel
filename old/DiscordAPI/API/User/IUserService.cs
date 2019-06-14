using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.API.User.Models;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.User
{
    public interface IUserService
    {
        [Get("/users")]
        Task<IEnumerable<SharedModels.User>> GetUsers([AliasAs("q")] string usernameQuery, [AliasAs("limit")] int limit);

        [Get("/v6/users/@me")]
        Task<SharedModels.User> GetCurrentUser();
        
        [Patch("/v6/users/@me/guilds/{guildId}/settings")]
        [Headers("Content-Type: application/json;")]
        Task<GuildSetting> ModifyGuildSettings([AliasAs("guildId")] string guildId, [Body] GuildSettingModify guildSetting);

        [Patch("/v6/users/@me/settings")]
        [Headers("Content-Type: application/json;")]
        Task UpdateSettings([Body] string settings);

        [Patch("/v6/users/@me/settings")]
        [Headers("Content-Type: application/json;")]
        Task<UserSettings> UpdateSettings([Body] ModifyUserSettings settings);

        [Post("/v6/users/@me/games/start")]
        [Headers("Content-Type: application/json;")]
        Task UpdateGame([Body] string game);

        [Patch("/users/@me")]
        Task<SharedModels.User> ModifyCurrentUser([Body] ModifyUser modifyUser);

        [Get("/users/{userId}")]
        Task<SharedModels.User> GetUser([AliasAs("userId")] string userId);

        [Get("/users/@me/guilds")]
        Task<IEnumerable<UserGuild>> GetCurrentUserGuilds();

        [Delete("/users/@me/guilds/{guildId}")]
        Task LeaveGuild([AliasAs("guildId")] string guildId);

        [Get("/users/@me/channels")]
        Task<IEnumerable<DirectMessageChannel>> GetCurrentUserDirectMessageChannels();

        [Post("/v6/users/@me/channels")]
        [Headers("Content-Type: application/json;")]
        Task<DirectMessageChannel> CreateDirectMessageChannelForCurrentUser([Body] CreateDM createDM);

        [Get("/users/@me/connections")]
        Task<IEnumerable<Connection>> GetCurrentUserConnections();

        [Put("/users/@me/notes/{userId}")]
        Task AddNote([AliasAs("userId")] string userId, [Body] Note note);

        [Get("/v6/users/{userID}/profile")]
        Task<UserProfile> GetUserProfile([AliasAs("userId")] string id);

        [Get("/v6/users/{userID}/relationships")]
        Task<IEnumerable<SharedFriend>> GetUserReleations([AliasAs("userId")] string id);

        [Put("/v6/users/@me/relationships/{userID}")]
        [Headers("Content-Type: application/json;")]
        Task SendFriendRequest([AliasAs("userId")] string id, [Body] string body = "{}");

        [Post("/v6/users/@me/relationships")]
        [Headers("Content-Type: application/json;")]
        Task<SendFriendRequestResponse> SendFriendRequest([Body] SendFriendRequest friendRequest);

        [Put("/v6/users/@me/relationships/{userID}")]
        [Headers("Content-Type: application/json;")]
        Task BlockUser([AliasAs("userId")] string id, [Body] string body = "{\"type\":2}");

        [Delete("/v6/users/@me/relationships/{userID}")]
        [Headers("Content-Type: application/json;")]
        Task RemoveFriend([AliasAs("userId")] string id, [Body] string body = "{}");

        [Get("/v6/users/@me/mentions?limit={limit}&roles={ShowRoles}&everyone={ShowEveryone}")]
        Task<IEnumerable<Message>> GetRecentMentions([AliasAs("limit")] int limit, [AliasAs("ShowRoles")] bool ShowRoles, [AliasAs("ShowEveryone")] bool ShowEveryone);
    }
}
