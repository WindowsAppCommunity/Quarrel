// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Microsoft.Extensions.DependencyInjection;
using Quarrel.Bindables.Messages;
using Quarrel.Services.Localization;

namespace Quarrel.Converters.DataTemplates.Messages
{
    public class InfoMessageContentConverter
    {
        public static string Convert(BindableMessage message)
        {
            string resource = message.Message.Type switch
            {
                MessageType.RecipientAdd => "InfoMessage/RecipientAdd",
                MessageType.RecipientRemove => "InfoMessage/RecipientRemove",
                MessageType.Call => "InfoMessage/Call",
                MessageType.ChannelNameChange => "InfoMessage/ChannelNameChange",
                MessageType.ChannelIconChange => "InfoMessage/ChannelIconChange",
                MessageType.ChannelPinnedMessage => "InfoMessage/ChannelPinnedMessage",
                MessageType.GuildMemberJoin => "InfoMessage/GuildMemberJoin",
                _ => "InfoMessage/Unknown",
            };

            return App.Current.Services.GetRequiredService<ILocalizationService>()[resource];
        }
    }
}
