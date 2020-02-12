using DiscordAPI.API.Guild.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Guilds;
using System;
using System.Runtime.InteropServices.ComTypes;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class AuditLogEntryToMarkdownConverter : IValueConverter
    {
        IChannelsService ChannelsService => SimpleIoc.Default.GetInstance<IChannelsService>();
        IGuildsService GuildsService => SimpleIoc.Default.GetInstance<IGuildsService>();

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

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AuditLogEntry entry)
            {
                string format = ResourceLoader.GetForCurrentView("AuditLog").GetString(((AuditLogActionType)entry.ActionType).ToString());
                
                
                switch ((AuditLogActionType)entry.ActionType)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.ChannelDelete:
                        return ReplaceChannel(format, entry.TargetId);
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
