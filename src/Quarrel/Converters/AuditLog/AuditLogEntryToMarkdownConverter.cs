using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class AuditLogEntryToMarkdownConverter : IValueConverter
    {
        #region Services

        IChannelsService ChannelsService => SimpleIoc.Default.GetInstance<IChannelsService>();
        IGuildsService GuildsService => SimpleIoc.Default.GetInstance<IGuildsService>();

        #endregion

        #region Methods

        public string ReplaceUser(string format, string userId)
        {
            string formattedUser = string.Format("<@{0}>", userId);
            return format.Replace("<user>", formattedUser);
        }

        public string ReplaceChannel(string format, string channelId, Change[] changes)
        {
            string formattedChannel = "";
            if (ChannelsService.AllChannels.ContainsKey(channelId))
                formattedChannel = string.Format("<#{0}>", channelId);
            else
            {
                foreach (Change change in changes)
                {
                    if (change.Key == "name")
                    {
                        formattedChannel = string.Format("**{0}**", change.OldValue);
                        break;
                    }
                }
                formattedChannel = "**<deleted-channel>**";
            }

            return format.Replace("<channel>", formattedChannel);
        }

        public string ReplaceInvite(string format, Change[] changes, bool deleted)
        {
            foreach (var change in changes)
            {
                if (change.Key == "code")
                {
                    return format.Replace("<invite>", string.Format("**{0}**", (deleted ? change.OldValue : change.NewValue).ToString()));
                }
            }
            return null;
        }

        public string ReplaceRecipient(string format, string userId)
        {
            string formattedUser = string.Format("<@{0}>", userId);
            return format.Replace("<recipient>", formattedUser);
        }

        public string ReplaceRole(string format, string roleId)
        {
            string formattedRole = string.Format("<@&{0}>", roleId);
            return format.Replace("<role>", formattedRole);
        }

        public string ReplaceEmoji(string format, string emojiId)
        {
            // TODO: Display emoji with markdown
            return format.Replace("<emoji>", "emoji: " + emojiId);
        }

        public string ReplaceWebhook(string format, Change[] changes, bool deleted)
        {
            foreach (var change in changes)
            {
                if (change.Key == "name")
                {
                    return format.Replace("<webhook>", string.Format("**{0}**", (deleted ? change.OldValue : change.NewValue).ToString()));
                }
            }
            return null;
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AuditLogEntry entry)
            {
                string action = ((AuditLogActionType)entry.ActionType).ToString() ?? "Unknown";
                string format = ResourceLoader.GetForCurrentView("AuditLog").GetString(action);

                format = ReplaceUser(format, entry.UserId);
                
                switch ((AuditLogActionType)entry.ActionType)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.ChannelDelete:
                    case AuditLogActionType.ChannelOverwriteCreate:
                    case AuditLogActionType.ChannelOverwriteUpdate:
                    case AuditLogActionType.ChannelOverwriteDelete:
                        return ReplaceChannel(format, entry.TargetId, entry.Changes);

                    case AuditLogActionType.EmojiCreate:
                    case AuditLogActionType.EmojiUpdate:
                    case AuditLogActionType.EmojiDelete:
                        return ReplaceEmoji(format, entry.TargetId);

                    case AuditLogActionType.RoleCreate:
                    case AuditLogActionType.RoleUpdate:
                    case AuditLogActionType.RoleDelete:
                        return ReplaceRole(format, entry.TargetId);

                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.InviteUpdate:
                    case AuditLogActionType.InviteDelete:
                        return ReplaceInvite(format, entry.Changes,
                            (AuditLogActionType)entry.ActionType == AuditLogActionType.InviteDelete);

                    case AuditLogActionType.WebhookCreate:
                    case AuditLogActionType.WebhookUpdate:
                    case AuditLogActionType.WebhookDelete:
                        return ReplaceWebhook(format, entry.Changes,
                            (AuditLogActionType)entry.ActionType == AuditLogActionType.WebhookDelete);

                    case AuditLogActionType.MemberBanAdd:
                    case AuditLogActionType.MemberBanRemove:
                    case AuditLogActionType.MemberKick:
                    case AuditLogActionType.MemberRoleUpdate:
                    case AuditLogActionType.MemberUpdate:
                        return ReplaceRecipient(format, entry.TargetId);

                    case AuditLogActionType.MessageDelete:
                        format = ReplaceRecipient(format, entry.TargetId);
                        return ReplaceChannel(format, entry.Options.ChannelId, entry.Changes);
                }
                return format;
            }
            return "Unknown action";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
