using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.User.Models
{
    public class ModifyUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
    }

    public class ModifyUserSettings
    {
        public ModifyUserSettings(UserSettings settings)
        {
            Theme = settings.Theme;
            Status = settings.Status;
            ShowCurrentGame = settings.ShowCurrentGame;
            RestrictedGuilds = settings.RestrictedGuilds;
            RenderReactions = settings.RenderReactions;
            RenderEmbeds = settings.RenderEmbeds;
            DevMode = settings.DevMode;
            MessageDisplayCompact = settings.MessageDisplayCompact;
            Locale = settings.Locale;
            InlineEmbedMedia = settings.InlineEmbedMedia;
            InlineAttachementMedia = settings.InlineAttachementMedia;
            GuildOrder = settings.GuildOrder;
            FriendSourceFlag = settings.FriendSourceFlag;
            ExplicitContentFilter = settings.ExplicitContentFilter;
            EnableTtsCommand = settings.EnableTtsCommand;
            ConvertEmoticons = settings.ConvertEmoticons;
            AfkTimeout = settings.AfkTimeout;
        }

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

        [JsonProperty("friend_source_flag")] /* Dafuq is this? */
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

    public class ModifyUserAndAvatar : ModifyUser
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
