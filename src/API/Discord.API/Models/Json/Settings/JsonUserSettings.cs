// Adam Dernis © 2022

using System.Text.Json.Serialization;

namespace Discord.API.Models.Json.Settings
{
    internal class JsonUserSettings
    {
        [JsonPropertyName("theme")]
        public string Theme { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("show_current_game")]
        public bool ShowCurrentGame { get; set; }

        [JsonPropertyName("restricted_guilds"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] RestrictedGuilds { get; set; }

        [JsonPropertyName("render_reactions")]
        public bool RenderReactions { get; set; }

        [JsonPropertyName("render_embeds")]
        public bool RenderEmbeds { get; set; }

        [JsonPropertyName("developer_mode")]
        public bool DeveloperMode { get; set; }

        [JsonPropertyName("message_display_compact")]
        public bool MessageDisplayCompact { get; set; }

        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonPropertyName("inline_embed_media")]
        public bool InlineEmbedMedia { get; set; }

        [JsonPropertyName("inline_attachment_media")]
        public bool InlineAttachementMedia { get; set; }

        [JsonPropertyName("guild_positions"), JsonNumberHandling(Constants.ReadWriteAsString)]
        public ulong[] GuildOrder { get; set; }

        [JsonPropertyName("guild_folders")]
        public JsonGuildFolder[] GuildFolders { get; set; }

    }
}
