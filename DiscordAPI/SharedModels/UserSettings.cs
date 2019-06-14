using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.SharedModels
{
    public class UserSettings
    {
        [JsonProperty("theme")]
        public string Theme { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("show_current_game")]
        public string ShowCurrentGame { get; set; }

        [JsonProperty("restricted_guilds")]
        public string[] RestrictedGuilds { get; set; }

        [JsonProperty("render_reactions")]
        public bool RenderReactions { get; set; }

        [JsonProperty("render_embeds")]
        public bool RenderEmbeds { get; set; }

        [JsonProperty("developer_mode")]
        public bool DevMode { get; set; }

        [JsonProperty("message_display_compact")]
        public bool MessageDisplayCompact { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("inline_embed_media")]
        public bool InlineEmbedMedia { get; set; }

        [JsonProperty("inline_attachement_media")]
        public bool InlineAttachementMedia { get; set; }

        [JsonProperty("guild_positions")]
        public IEnumerable<string> GuildOrder { get; set; }

        [JsonProperty("friend_source_flag")]
        public FriendSourceFlag FriendSourceFlag { get; set; }

        [JsonProperty("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }

        [JsonProperty("enable_tts_command")]
        public bool EnableTtsCommand { get; set; }

        [JsonProperty("convert_emoticons")]
        public bool ConvertEmoticons { get; set; }

        [JsonProperty("afk_timeout")]
        public int AfkTimeout { get; set; }
    }

    public class GuildSetting
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("suppress_everyone")]
        public bool SuppressEveryone { get; set; }

        [JsonProperty("muted")]
        public bool Muted { get; set; }

        [JsonProperty("mobile_push")]
        public bool MobilePush { get; set; }

        [JsonProperty("message_notifications")]
        public int MessageNotifications { get; set; }

        [JsonProperty("channel_overrides")]
        public IEnumerable<ChannelOverride> ChannelOverrides { get; set; }

    }

    public class GuildSettingModify
    {
        [JsonProperty("guild_id")]
        public string GuildId { get; set; }

        [JsonProperty("suppress_everyone")]
        public bool SuppressEveryone { get; set; }

        [JsonProperty("muted")]
        public bool Muted { get; set; }

        [JsonProperty("mobile_push")]
        public bool MobilePush { get; set; }

        [JsonProperty("message_notifications")]
        public int MessageNotifications { get; set; }

        [JsonProperty("channel_overrides")]
        public IDictionary<string, ChannelOverride> ChannelOverrides { get; set; }

    }

    public class ChannelOverride
    {
        [JsonProperty("channel_id")]
        public string Channel_Id { get; set; }

        [JsonProperty("muted")]
        public bool Muted { get; set; }

        [JsonProperty("message_notifications")]
        public int MessageNotifications { get; set; }
    }

    public class FriendSourceFlag
    {
        [JsonProperty("all")]
        public bool All { get; set; }

        [JsonProperty("mutual_friends")]
        public bool MutualFriends { get; set; }

        [JsonProperty("mutual_guilds")]
        public bool MutualGuilds { get; set; }
    }
}
