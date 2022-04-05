// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Guilds
{
    /// <summary>
    /// An enum representing the subscription tier of a server.
    /// </summary>
    public enum PremiumTier : int
    {
        /// <summary>
        /// The guild has no boosted tier.
        /// </summary>
        None = 0,

        /// <summary>
        /// The guild has been boosted to tier 1.
        /// </summary>
        Tier1 = 1,

        /// <summary>
        /// The guild has been boosted to tier 2.
        /// </summary>
        Tier2 = 2,

        /// <summary>
        /// The guild has been boosted to tier 3.
        /// </summary>
        Tier3 = 3,
    }
}
