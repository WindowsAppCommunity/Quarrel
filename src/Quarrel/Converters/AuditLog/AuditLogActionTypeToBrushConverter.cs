using DiscordAPI.API.Guild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class AuditLogActionTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int iValue)
            {
                switch ((AuditLogActionType)iValue)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelOverwriteCreate:
                    case AuditLogActionType.EmojiCreate:
                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.RoleCreate:
                    case AuditLogActionType.WebhookCreate:
                    case AuditLogActionType.MemberBanRemove:
                        return App.Current.Resources["online"];

                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.ChannelOverwriteUpdate:
                    case AuditLogActionType.EmojiUpdate:
                    case AuditLogActionType.InviteUpdate:
                    case AuditLogActionType.RoleUpdate:
                    case AuditLogActionType.WebhookUpdate:
                    case AuditLogActionType.MemberKick:
                    case AuditLogActionType.GuildUpdate:
                    case AuditLogActionType.MemberPrune:
                    case AuditLogActionType.MemberRoleUpdate:
                    case AuditLogActionType.MemberUpdate:
                        return App.Current.Resources["idle"];

                    case AuditLogActionType.ChannelDelete:
                    case AuditLogActionType.ChannelOverwriteDelete:
                    case AuditLogActionType.EmojiDelete:
                    case AuditLogActionType.InviteDelete:
                    case AuditLogActionType.RoleDelete:
                    case AuditLogActionType.WebhookDelete:
                    case AuditLogActionType.MemberBanAdd:
                    case AuditLogActionType.MessageDelete:
                        return App.Current.Resources["dnd"];
                }
            }
            return App.Current.Resources["idle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
