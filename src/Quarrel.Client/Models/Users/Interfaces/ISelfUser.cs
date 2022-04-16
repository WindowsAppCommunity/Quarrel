// Quarrel © 2022

using Discord.API.Models.Enums.Users;

namespace Quarrel.Client.Models.Users.Interfaces
{
    internal interface ISelfUser : IUser
    {
        /// <summary>
        /// Gets the user's email address.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Gets the user's phone number.
        /// </summary>
        string? Phone { get; }

        /// <summary>
        /// Gets whether or not the user is verified.
        /// </summary>
        bool? Verified { get; }

        /// <summary>
        /// Gets whether or not the user has multi-factor authentication enabled.
        /// </summary>
        bool? MfaEnabled { get; }

        /// <summary>
        /// Gets whether or not the user allows NSFW content in their messages.
        /// </summary>
        bool? NSFWAllowed { get; }

        /// <summary>
        /// Gets what 
        /// </summary>
        PremiumType PurchasedFlags { get; }

        /// <summary>
        /// Gets the user's locale.
        /// </summary>
        string? Locale { get; }
    }
}
