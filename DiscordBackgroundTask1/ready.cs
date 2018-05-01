using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBackgroundTask1
{
    internal sealed class ReadyStructure
    {
        internal sealed class FriendSourceFlags
        {
            internal bool all { get; set; }
        }

        internal sealed class UserSettings
        {
            internal int timezone_offset { get; set; }
            internal string theme { get; set; }
            internal string status { get; set; }
            internal bool show_current_game { get; set; }
            internal List<object> restricted_guilds { get; set; }
            internal bool render_reactions { get; set; }
            internal bool render_embeds { get; set; }
            internal bool message_display_compact { get; set; }
            internal string locale { get; set; }
            internal bool inline_embed_media { get; set; }
            internal bool inline_attachment_media { get; set; }
            internal List<string> guild_positions { get; set; }
            internal FriendSourceFlags friend_source_flags { get; set; }
            internal int explicit_content_filter { get; set; }
            internal bool enable_tts_command { get; set; }
            internal bool developer_mode { get; set; }
            internal bool detect_platform_accounts { get; set; }
            internal bool default_guilds_restricted { get; set; }
            internal bool convert_emoticons { get; set; }
            internal bool animate_emoji { get; set; }
            internal int afk_timeout { get; set; }
        }

        internal sealed class UserGuildSetting
        {
            internal bool suppress_everyone { get; set; }
            internal bool muted { get; set; }
            internal bool mobile_push { get; set; }
            internal int message_notifications { get; set; }
            internal string guild_id { get; set; }
            internal List<object> channel_overrides { get; set; }
        }

        internal sealed class User
        {
            internal bool verified { get; set; }
            internal string username { get; set; }
            internal bool premium { get; set; }
            internal object phone { get; set; }
            internal bool mobile { get; set; }
            internal bool mfa_enabled { get; set; }
            internal string id { get; set; }
            internal int flags { get; set; }
            internal string email { get; set; }
            internal string discriminator { get; set; }
            internal string avatar { get; set; }
        }

        internal sealed class User2
        {
            internal string username { get; set; }
            internal string id { get; set; }
            internal string discriminator { get; set; }
            internal string avatar { get; set; }
        }

        internal sealed class Relationship
        {
            internal User2 user { get; set; }
            internal int type { get; set; }
            internal string id { get; set; }
        }

        internal sealed class ReadState
        {
            internal string last_message_id { get; set; }
            internal string id { get; set; }
            internal int? mention_count { get; set; }
            internal DateTime? last_pin_timestamp { get; set; }
        }

        internal sealed class Recipient
        {
            internal string username { get; set; }
            internal string id { get; set; }
            internal string discriminator { get; set; }
            internal string avatar { get; set; }
            internal bool? bot { get; set; }
        }

        internal sealed class PrivateChannel
        {
            internal int type { get; set; }
            internal List<Recipient> recipients { get; set; }
            internal string last_message_id { get; set; }
            internal string id { get; set; }
            internal object topic { get; set; }
            internal string owner_id { get; set; }
            internal string name { get; set; }
            internal DateTime? last_pin_timestamp { get; set; }
            internal object icon { get; set; }
        }

        internal sealed class Notes
        {
            internal string __invalid_name__88332367042727936 { get; set; }
            internal string __invalid_name__330500621956284416 { get; set; }
            internal string __invalid_name__281581635038609409 { get; set; }
            internal string __invalid_name__270372582795116545 { get; set; }
            internal string __invalid_name__250308103105413121 { get; set; }
            internal string __invalid_name__247800237362511883 { get; set; }
            internal string __invalid_name__238436483373989888 { get; set; }
            internal string __invalid_name__225322036271120384 { get; set; }
            internal string __invalid_name__216813599430017024 { get; set; }
            internal string __invalid_name__161447131020918784 { get; set; }
            internal string __invalid_name__159985870458322944 { get; set; }
            internal string __invalid_name__142311960061411337 { get; set; }
            internal string __invalid_name__140275885327646720 { get; set; }
            internal string __invalid_name__140221094501154816 { get; set; }
        }

        internal sealed class Role
        {
            internal int position { get; set; }
            internal int permissions { get; set; }
            internal string name { get; set; }
            internal bool mentionable { get; set; }
            internal bool managed { get; set; }
            internal string id { get; set; }
            internal bool hoist { get; set; }
            internal int color { get; set; }
        }

        internal sealed class User3
        {
            internal string username { get; set; }
            internal string id { get; set; }
            internal string discriminator { get; set; }
            internal string avatar { get; set; }
            internal bool? bot { get; set; }
        }

        internal sealed class Member
        {
            internal User3 user { get; set; }
            internal List<object> roles { get; set; }
            internal bool mute { get; set; }
            internal DateTime joined_at { get; set; }
            internal bool deaf { get; set; }
            internal string nick { get; set; }
        }

        internal sealed class Channel
        {
            internal int type { get; set; }
            internal string topic { get; set; }
            internal int position { get; set; }
            internal List<object> permission_overwrites { get; set; }
            internal string parent_id { get; set; }
            internal string name { get; set; }
            internal string last_message_id { get; set; }
            internal string id { get; set; }
            internal int? user_limit { get; set; }
            internal int? bitrate { get; set; }
            internal DateTime? last_pin_timestamp { get; set; }
            internal bool? nsfw { get; set; }
        }

        internal sealed class Guild
        {
            internal List<object> voice_states { get; set; }
            internal int verification_level { get; set; }
            internal string system_channel_id { get; set; }
            internal string splash { get; set; }
            internal List<Role> roles { get; set; }
            internal string region { get; set; }
            internal List<object> presences { get; set; }
            internal string owner_id { get; set; }
            internal string name { get; set; }
            internal int mfa_level { get; set; }
            internal List<Member> members { get; set; }
            internal int member_count { get; set; }
            internal bool large { get; set; }
            internal DateTime joined_at { get; set; }
            internal string id { get; set; }
            internal string icon { get; set; }
            internal List<object> features { get; set; }
            internal int explicit_content_filter { get; set; }
            internal List<object> emojis { get; set; }
            internal int default_message_notifications { get; set; }
            internal List<Channel> channels { get; set; }
            internal object application_id { get; set; }
            internal int afk_timeout { get; set; }
            internal string afk_channel_id { get; set; }
        }

        internal sealed class ConnectedAccount
        {
            internal int visibility { get; set; }
            internal bool verified { get; set; }
            internal string type { get; set; }
            internal bool show_activity { get; set; }
            internal bool revoked { get; set; }
            internal string name { get; set; }
            internal string id { get; set; }
            internal bool friend_sync { get; set; }
        }

        internal sealed class Ready
        {
            internal int v { get; set; }
            internal UserSettings user_settings { get; set; }
            internal List<UserGuildSetting> user_guild_settings { get; set; }
            internal User user { get; set; }
            internal object tutorial { get; set; }
            internal string session_id { get; set; }
            internal List<Relationship> relationships { get; set; }
            internal List<ReadState> read_state { get; set; }
            internal List<PrivateChannel> private_channels { get; set; }
            internal List<object> presences { get; set; }
            internal Notes notes { get; set; }
            internal List<Guild> guilds { get; set; }
            internal int friend_suggestion_count { get; set; }
            internal List<List<int>> experiments { get; set; }
            internal List<ConnectedAccount> connected_accounts { get; set; }
            internal string analytics_token { get; set; }
            internal List<string> _trace { get; set; }
        }
    }
}
