// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Quarrel.Bindables.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Quarrel.Selectors.Messages
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate InfoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            BindableMessage message = (BindableMessage)item;
            return message.Message.Type switch
            {
                MessageType.Default or MessageType.Reply => DefaultTemplate,
                MessageType.RecipientAdd or
                MessageType.RecipientRemove or
                MessageType.Call or
                MessageType.ChannelNameChange or
                MessageType.ChannelIconChange or
                MessageType.ChannelPinnedMessage or 
                MessageType.GuildMemberJoin or
                MessageType.UserPremiumGuildSubscription or
                MessageType.UserPremiumGuildSubscriptionTier1 or
                MessageType.UserPremiumGuildSubscriptionTier2 or
                MessageType.UserPremiumGuildSubscriptionTier3 or
                MessageType.ChannelFollowAdd or
                MessageType.GuildDiscoveryDisqualified or
                MessageType.GuildDiscoveryRequalified or
                MessageType.GuildDiscoveryGracePeriodInitialWarning or
                MessageType.GuildDiscoveryGracePeriodFinalWarning or
                MessageType.ThreadCreated or
                MessageType.ApplicationCommand or
                MessageType.ThreadStarterMessage or
                MessageType.GuildInviteReminder or
                MessageType.ContextMenuCommand => InfoTemplate,
                _ => null,
            };
        }
    }
}
