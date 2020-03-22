// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;
using System;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    /// <summary>
    /// Converter for AuditLogAction to an icon.
    /// </summary>
    public sealed class AuditLogActionTypeToGlyphConverter : IValueConverter
    {
        /// <summary>
        /// Converts AuditLogAction to an icon.
        /// </summary>
        /// <param name="value">AuditLogAction.</param>
        /// <param name="targetType">Requested out type.</param>
        /// <param name="parameter">Extra info.</param>
        /// <param name="language">What language the user is using.</param>
        /// <returns>Glyph representing action.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int iValue)
            {
                switch ((AuditLogActionType)iValue)
                {
                    case AuditLogActionType.ChannelCreate:
                    case AuditLogActionType.ChannelUpdate:
                    case AuditLogActionType.ChannelDelete:
                        return "";

                    case AuditLogActionType.ChannelOverwriteCreate:
                    case AuditLogActionType.ChannelOverwriteDelete:
                    case AuditLogActionType.ChannelOverwriteUpdate:
                        return "";

                    case AuditLogActionType.EmojiCreate:
                    case AuditLogActionType.EmojiDelete:
                    case AuditLogActionType.EmojiUpdate:
                        return "";

                    case AuditLogActionType.GuildUpdate:
                        return "";

                    case AuditLogActionType.InviteCreate:
                    case AuditLogActionType.InviteDelete:
                    case AuditLogActionType.InviteUpdate:
                        return "";

                    case AuditLogActionType.MemberBanAdd:
                    case AuditLogActionType.MemberBanRemove:
                    case AuditLogActionType.MemberKick:
                        return "";

                    case AuditLogActionType.MemberPrune:
                        return "";

                    case AuditLogActionType.MemberRoleUpdate:
                        return ""; // TODO: Change

                    case AuditLogActionType.MemberUpdate:
                        return "";

                    case AuditLogActionType.MessageDelete:
                        return "";

                    case AuditLogActionType.RoleCreate:
                    case AuditLogActionType.RoleDelete:
                    case AuditLogActionType.RoleUpdate:
                        return "";

                    case AuditLogActionType.WebhookCreate:
                    case AuditLogActionType.WebhookDelete:
                    case AuditLogActionType.WebhookUpdate:
                        return "";
                }
            }

            return "?";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
