// Quarrel © 2022

using Discord.API.Models.Enums.Messages;

namespace Quarrel.Converters.DataTemplates.Messages
{
    public class InfoMessageIconConverter
    {
        public static string Convert(MessageType type)
        {
            return type switch
            {
                MessageType.Call => "",

                MessageType.ChannelPinnedMessage => "",

                MessageType.ChannelIconChange or
                MessageType.ChannelNameChange => "",

                MessageType.GuildMemberJoin or
                MessageType.RecipientAdd => "",

                MessageType.RecipientRemove => "",
                _ => "?",
            };
        }
    }
}
