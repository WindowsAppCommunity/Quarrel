// Adam Dernis © 2022

using Discord.API.Models.Enums.Users;

namespace Discord.API.Models.Users.Interfaces
{
    public interface Presence
    {
        UserStatus Status { get; }
    }
}
