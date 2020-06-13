// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.Guild.Models;

namespace Quarrel.Converters.AuditLog
{
    /// <summary>
    /// Converter for AuditLogAction to an icon.
    /// </summary>
    public sealed class AuditLogActionTypeToGlyphConverter
    {
        /// <summary>
        /// Converts AuditLogAction to an icon.
        /// </summary>
        /// <param name="value">AuditLogAction.</param>
        /// <returns>Glyph representing action.</returns>
        public static string Convert(int value)
        {
            switch ((AuditLogActionType)value)
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

            return "?";
        }
    }
}
