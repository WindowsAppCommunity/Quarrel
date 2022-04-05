// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Users
{
    /// <summary>
    /// The type of Discord Nitro a user has.
    /// </summary>
    public enum PremiumType
    {
        /// <summary>
        /// The user does not have Discord Nitro.
        /// </summary>
        None = 0,

        /// <summary>
        /// The user has Discord Nitro Classic.
        /// </summary>
        NitroClassic = 1,

        /// <summary>
        /// The user has regular Discord Nitro.
        /// </summary>
        Nitro = 2
    }
}
