using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway
{
    public class EventNames
    {
        public const string READY = "READY";
        public const string IDENTIFY = "IDENTIFY";

        public const string CHANNEL_CREATED = "CHANNEL_CREATE";
        public const string CHANNEL_UPDATED = "CHANNEL_UPDATE";
        public const string CHANNEL_DELETED = "CHANNEL_DELETE";

        public const string GUILD_CREATED = "GUILD_CREATE";
        public const string GUILD_UPDATED = "GUILD_UPDATE";
        public const string GUILD_DELETED = "GUILD_DELETE";

        public const string GUILD_BAN_ADDED = "GUILD_BAN_ADD";
        public const string GUILD_EMOJI_UPDATED = "GUILD_EMOJI_UPDATE";
        public const string GUILD_INTEGRATIONS_UPDATED = "GUILD_INTEGRATIONS_UPDATE";

        public const string GUILD_MEMBER_ADDED = "GUILD_MEMBER_ADD";
        public const string GUILD_MEMBER_REMOVED = "GUILD_MEMBER_REMOVE";
        public const string GUILD_MEMBER_UPDATED = "GUILD_MEMBER_UPDATE";
        public const string GUILD_MEMBER_CHUNK = "GUILD_MEMBER_CHUNK";

        public const string GUILD_ROLE_CREATED = "GUILD_ROLE_CREATE";
        public const string GUILD_ROLE_UPDATED = "GUILD_ROLE_UPDATE";
        public const string GUILD_ROLE_DELETED = "GUILD_ROLE_DELETE";

        public const string MESSAGE_CREATED = "MESSAGE_CREATE";
        public const string MESSAGE_UPDATED = "MESSAGE_UPDATE";
        public const string MESSAGE_DELETED = "MESSAGE_DELETE";
        public const string MESSAGE_DELETED_BULK = "MESSAGE_DELETE_BULK";

        public const string PRESENCE_UPDATED = "PRESENCE_UPDATE";
        public const string TYPING_START = "TYPING_START";
        public const string USER_SETTINGS_UPDATED = "USER_SETTINGS_UPDATED";
        public const string USER_UPDATED = "USER_UPDATE";
        public const string VOICE_STATE_UPDATED = "VOICE_STATE_UPDATE";
        public const string VOICE_SERVER_UPDATED = "VOICE_SERVER_UPDATE";
    }
}
