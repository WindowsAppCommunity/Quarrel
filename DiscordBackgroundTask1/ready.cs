using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBackgroundTask1
{
    public class ReadyStructure
    {
        public class FriendSourceFlags
        {
            public bool all { get; set; }
        }

        public class UserSettings
        {
            public int timezone_offset { get; set; }
            public string theme { get; set; }
            public string status { get; set; }
            public bool show_current_game { get; set; }
            public List<object> restricted_guilds { get; set; }
            public bool render_reactions { get; set; }
            public bool render_embeds { get; set; }
            public bool message_display_compact { get; set; }
            public string locale { get; set; }
            public bool inline_embed_media { get; set; }
            public bool inline_attachment_media { get; set; }
            public List<string> guild_positions { get; set; }
            public FriendSourceFlags friend_source_flags { get; set; }
            public int explicit_content_filter { get; set; }
            public bool enable_tts_command { get; set; }
            public bool developer_mode { get; set; }
            public bool detect_platform_accounts { get; set; }
            public bool default_guilds_restricted { get; set; }
            public bool convert_emoticons { get; set; }
            public bool animate_emoji { get; set; }
            public int afk_timeout { get; set; }
        }

        public class UserGuildSetting
        {
            public bool suppress_everyone { get; set; }
            public bool muted { get; set; }
            public bool mobile_push { get; set; }
            public int message_notifications { get; set; }
            public string guild_id { get; set; }
            public List<object> channel_overrides { get; set; }
        }

        public class User
        {
            public bool verified { get; set; }
            public string username { get; set; }
            public bool premium { get; set; }
            public object phone { get; set; }
            public bool mobile { get; set; }
            public bool mfa_enabled { get; set; }
            public string id { get; set; }
            public int flags { get; set; }
            public string email { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
        }

        public class User2
        {
            public string username { get; set; }
            public string id { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
        }

        public class Relationship
        {
            public User2 user { get; set; }
            public int type { get; set; }
            public string id { get; set; }
        }

        public class ReadState
        {
            public string last_message_id { get; set; }
            public string id { get; set; }
            public int? mention_count { get; set; }
            public DateTime? last_pin_timestamp { get; set; }
        }

        public class Recipient
        {
            public string username { get; set; }
            public string id { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
            public bool? bot { get; set; }
        }

        public class PrivateChannel
        {
            public int type { get; set; }
            public List<Recipient> recipients { get; set; }
            public string last_message_id { get; set; }
            public string id { get; set; }
            public object topic { get; set; }
            public string owner_id { get; set; }
            public string name { get; set; }
            public DateTime? last_pin_timestamp { get; set; }
            public object icon { get; set; }
        }

        public class Notes
        {
            public string __invalid_name__88332367042727936 { get; set; }
            public string __invalid_name__330500621956284416 { get; set; }
            public string __invalid_name__281581635038609409 { get; set; }
            public string __invalid_name__270372582795116545 { get; set; }
            public string __invalid_name__250308103105413121 { get; set; }
            public string __invalid_name__247800237362511883 { get; set; }
            public string __invalid_name__238436483373989888 { get; set; }
            public string __invalid_name__225322036271120384 { get; set; }
            public string __invalid_name__216813599430017024 { get; set; }
            public string __invalid_name__161447131020918784 { get; set; }
            public string __invalid_name__159985870458322944 { get; set; }
            public string __invalid_name__142311960061411337 { get; set; }
            public string __invalid_name__140275885327646720 { get; set; }
            public string __invalid_name__140221094501154816 { get; set; }
        }

        public class Role
        {
            public int position { get; set; }
            public int permissions { get; set; }
            public string name { get; set; }
            public bool mentionable { get; set; }
            public bool managed { get; set; }
            public string id { get; set; }
            public bool hoist { get; set; }
            public int color { get; set; }
        }

        public class User3
        {
            public string username { get; set; }
            public string id { get; set; }
            public string discriminator { get; set; }
            public string avatar { get; set; }
            public bool? bot { get; set; }
        }

        public class Member
        {
            public User3 user { get; set; }
            public List<object> roles { get; set; }
            public bool mute { get; set; }
            public DateTime joined_at { get; set; }
            public bool deaf { get; set; }
            public string nick { get; set; }
        }

        public class Channel
        {
            public int type { get; set; }
            public string topic { get; set; }
            public int position { get; set; }
            public List<object> permission_overwrites { get; set; }
            public string parent_id { get; set; }
            public string name { get; set; }
            public string last_message_id { get; set; }
            public string id { get; set; }
            public int? user_limit { get; set; }
            public int? bitrate { get; set; }
            public DateTime? last_pin_timestamp { get; set; }
            public bool? nsfw { get; set; }
        }

        public class Guild
        {
            public List<object> voice_states { get; set; }
            public int verification_level { get; set; }
            public string system_channel_id { get; set; }
            public string splash { get; set; }
            public List<Role> roles { get; set; }
            public string region { get; set; }
            public List<object> presences { get; set; }
            public string owner_id { get; set; }
            public string name { get; set; }
            public int mfa_level { get; set; }
            public List<Member> members { get; set; }
            public int member_count { get; set; }
            public bool large { get; set; }
            public DateTime joined_at { get; set; }
            public string id { get; set; }
            public string icon { get; set; }
            public List<object> features { get; set; }
            public int explicit_content_filter { get; set; }
            public List<object> emojis { get; set; }
            public int default_message_notifications { get; set; }
            public List<Channel> channels { get; set; }
            public object application_id { get; set; }
            public int afk_timeout { get; set; }
            public string afk_channel_id { get; set; }
        }

        public class ConnectedAccount
        {
            public int visibility { get; set; }
            public bool verified { get; set; }
            public string type { get; set; }
            public bool show_activity { get; set; }
            public bool revoked { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public bool friend_sync { get; set; }
        }

        public class Ready
        {
            public int v { get; set; }
            public UserSettings user_settings { get; set; }
            public List<UserGuildSetting> user_guild_settings { get; set; }
            public User user { get; set; }
            public object tutorial { get; set; }
            public string session_id { get; set; }
            public List<Relationship> relationships { get; set; }
            public List<ReadState> read_state { get; set; }
            public List<PrivateChannel> private_channels { get; set; }
            public List<object> presences { get; set; }
            public Notes notes { get; set; }
            public List<Guild> guilds { get; set; }
            public int friend_suggestion_count { get; set; }
            public List<List<int>> experiments { get; set; }
            public List<ConnectedAccount> connected_accounts { get; set; }
            public string analytics_token { get; set; }
            public List<string> _trace { get; set; }
        }
    }
}
