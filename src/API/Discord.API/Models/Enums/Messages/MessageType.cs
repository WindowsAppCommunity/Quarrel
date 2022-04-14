// Quarrel © 2022

namespace Discord.API.Models.Enums.Messages
{
    /// <summary>
    /// The type of message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// A standard message sent by a user containing content.
        /// </summary>
        Default = 0,

        /// <summary>
        /// A message indicating a user was added to a group DM.
        /// </summary>
        RecipientAdd = 1,

        /// <summary>
        /// A message indicating a user was removed from a group DM.
        /// </summary>
        RecipientRemove = 2,

        /// <summary>
        /// A message indicating a call was sent in a DM.
        /// </summary>
        Call = 3,

        /// <summary>
        /// A message indicating a group DM's name has changed.
        /// </summary>
        ChannelNameChange = 4,

        /// <summary>
        /// A message indicating a group DM's icon has changed.
        /// </summary>
        ChannelIconChange = 5,

        /// <summary>
        /// A message indicating a message was pinned.
        /// </summary>
        ChannelPinnedMessage = 6,

        /// <summary>
        /// A message indicating a guild member has joined.
        /// </summary>
        GuildMemberJoin = 7,

        /// <summary>
        /// A message indicating someone boosted a guild.
        /// </summary>
        UserPremiumGuildSubscription = 8,

        /// <summary>
        /// A message indicating someone boosted a guild to tier 1.
        /// </summary>
        UserPremiumGuildSubscriptionTier1 = 9,

        /// <summary>
        /// A message indicating someone boosted a guild to tier 2.
        /// </summary>
        UserPremiumGuildSubscriptionTier2 = 10,

        /// <summary>
        /// A message indicating someone boosted a guild to tier 3.
        /// </summary>
        UserPremiumGuildSubscriptionTier3 = 11,

        /// <summary>
        /// A message indicating a channel is followed.
        /// </summary>
        ChannelFollowAdd = 12,

        /// <summary>
        /// A message indicating a guild is disqualified from discovery.
        /// </summary>
        GuildDiscoveryDisqualified = 14,

        /// <summary>
        /// A message indicating a guild is requalified for discovery.
        /// </summary>
        GuildDiscoveryRequalified = 15,

        /// <summary>
        /// A message indicating a guild is entering the grace period for discovery disqualification.
        /// </summary>
        GuildDiscoveryGracePeriodInitialWarning = 16,

        /// <summary>
        /// A message indicating a guild is leaving the grace period for discovery disqualification.
        /// </summary>
        GuildDiscoveryGracePeriodFinalWarning = 17,

        /// <summary>
        /// A message indicating a thread was created.
        /// </summary>
        ThreadCreated = 18,

        /// <summary>
        /// A message that is a reply to another message.
        /// </summary>
        Reply = 19,

        /// <summary>
        /// A message that is an application command.
        /// </summary>
        ApplicationCommand = 20,

        /// <summary>
        /// TODO: Invesitage
        /// </summary>
        ThreadStarterMessage = 21,

        /// <summary>
        /// TODO: Invesitage
        /// </summary>
        GuildInviteReminder = 22,

        /// <summary>
        /// TODO: Invesitage
        /// </summary>
        ContextMenuCommand = 23,
    }
}
