// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using Windows.UI.Xaml.Media;

namespace Quarrel.Converters.AuditLog
{
    /// <summary>
    /// Converter for AuditLogAction to a brush color.
    /// </summary>
    public class AuditLogActionTypeToBrushConverter
    {
        /// <summary>
        /// Converts AuditLogAction to a brush color.
        /// </summary>
        /// <param name="value">AuditLogAction.</param>
        /// <returns>A <see cref="SolidColorBrush"/> for the action.</returns>
        public static SolidColorBrush Convert(int value)
        {
            switch ((AuditLogActionType)value)
            {
                case AuditLogActionType.ChannelCreate:
                case AuditLogActionType.ChannelOverwriteCreate:
                case AuditLogActionType.EmojiCreate:
                case AuditLogActionType.InviteCreate:
                case AuditLogActionType.RoleCreate:
                case AuditLogActionType.WebhookCreate:
                case AuditLogActionType.MemberBanRemove:
                    return (SolidColorBrush)App.Current.Resources["online"];

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
                    return (SolidColorBrush)App.Current.Resources["idle"];

                case AuditLogActionType.ChannelDelete:
                case AuditLogActionType.ChannelOverwriteDelete:
                case AuditLogActionType.EmojiDelete:
                case AuditLogActionType.InviteDelete:
                case AuditLogActionType.RoleDelete:
                case AuditLogActionType.WebhookDelete:
                case AuditLogActionType.MemberBanAdd:
                case AuditLogActionType.MessageDelete:
                    return (SolidColorBrush)App.Current.Resources["dnd"];
            }

            return (SolidColorBrush)App.Current.Resources["idle"];
        }
    }
}
