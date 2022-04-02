// Adam Dernis © 2022

using Discord.API.Models.Enums.Users;
using Discord.API.Models.Users.Interfaces;

namespace Discord.API.Models.Managed.Users
{
    internal class Presence : IPresence
    {
        public UserStatus Status { get; set; }
    }
}
