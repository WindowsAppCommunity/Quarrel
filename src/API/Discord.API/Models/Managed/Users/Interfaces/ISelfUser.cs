// Adam Dernis © 2022

using Discord.API.Models.Enums.Users;

namespace Discord.API.Models.Users.Interfaces
{
    public interface ISelfUser : IUser
    {
        string? Email { get; }

        string? Phone { get; }

        bool? Verified { get; }

        bool? MfaEnabled { get; }

        bool? NSFWAllowed { get; }

        PremiumType PurchasedFlags { get; }

        string Locale { get; }
    }
}
