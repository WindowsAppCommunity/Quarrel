// Adam Dernis © 2022

namespace Discord.API.Models.Enums.Messages
{
    public enum MessageType
    {
        Default = 0,
        RecipientAdd = 1,
        RecipientRemove = 2,
        Call = 3,
        ChannelNameChange = 4,
        ChannelIconChange = 5,
        ChannelPinnedMessage = 6,
        GuildMemberJoin = 7,
        UserPremiumGuildSubscription = 8,
        UserPremiumGuildSubscriptionTier1 = 9,
        UserPremiumGuildSubscriptionTier2 = 10,
        UserPremiumGuildSubscriptionTier3 = 11,
        ChannelFollowAdd = 12,
        GuildDiscoveryDisqualified = 14,
        GuildDiscoveryRequalified = 15,
        GuildDiscoveryGracePeriodInitialWarning = 16,
        GuildDiscoveryGracePeriodFinalWarning = 17,
        ThreadCreated = 18,
        Reply = 19,
        ApplicationCommand = 20,
        ThreadStarterMessage = 21,
        GuildInviteReminder = 22,
        ContextMenuCommand = 23,
    }
}
