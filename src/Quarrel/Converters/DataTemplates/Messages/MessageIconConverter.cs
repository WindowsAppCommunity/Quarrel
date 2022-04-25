// Quarrel © 2022

using Discord.API.Models.Enums.Messages;

namespace Quarrel.Converters.DataTemplates.Messages
{
    public class MessageIconConverter
    {
        public static string Convert(MessageType type)
        {
            return type switch
            {
                MessageType.Call => "",
                MessageType.ChannelPinnedMessage => "",
                MessageType.ChannelIconChange or
                MessageType.ChannelNameChange => "",
                _ => "?",
            };
        }
    }
}
