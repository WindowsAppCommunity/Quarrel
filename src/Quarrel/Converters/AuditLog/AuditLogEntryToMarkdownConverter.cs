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

        public string ReplaceChannel(string format, string channelId)
        {
            string formattedChannel = string.Format("<#{0}>", channelId);
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

        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AuditLogEntry entry)
            {
                string format = ResourceLoader.GetForCurrentView("AuditLog").GetString(((AuditLogActionType)entry.ActionType).ToString());

                format = ReplaceUser(format, entry.UserId);
                
                switch ((AuditLogActionType)entry.ActionType)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.ChannelDelete:
                        return ReplaceChannel(format, entry.TargetId);

                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.InviteUpdate:
                    case AuditLogActionType.InviteDelete:
                        return ReplaceInvite(format, entry.Changes, (AuditLogActionType)entry.ActionType == AuditLogActionType.InviteDelete);
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
