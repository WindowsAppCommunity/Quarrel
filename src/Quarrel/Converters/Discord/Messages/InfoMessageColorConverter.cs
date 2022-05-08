// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.Discord.Messages
{
    public class InfoMessageColorConverter
    {
        public static Brush Convert(MessageType type)
        {
            string resource = type switch
            {
                MessageType.RecipientAdd or 
                MessageType.GuildMemberJoin or
                MessageType.ChannelFollowAdd or
                MessageType.GuildDiscoveryRequalified or
                MessageType.ThreadCreated=> "DiscordGreenBrush",

                MessageType.ChannelNameChange or
                MessageType.ChannelIconChange or
                MessageType.GuildDiscoveryGracePeriodInitialWarning => "DiscordYellowBrush",

                MessageType.GuildDiscoveryDisqualified or
                MessageType.GuildDiscoveryGracePeriodFinalWarning or
                MessageType.RecipientRemove => "DiscordRedBrush",

                MessageType.Call or
                MessageType.ChannelPinnedMessage or
                _ => "InvertedBackground",
            };

            return (Brush)App.Current.Resources[resource];
        }
    }
}
