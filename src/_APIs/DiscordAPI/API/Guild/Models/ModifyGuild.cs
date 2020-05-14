using Newtonsoft.Json;

namespace DiscordAPI.API.Guild.Models
{
    public class ModifyGuild
    {
        public ModifyGuild(DiscordAPI.Models.Guilds.Guild guild)
        {
            Name = guild.Name;
            Region = guild.Region;
            VerificationLevel = guild.VerificationLevel;
            AfkChannelId = guild.AfkChannelId;
            AfkTimeout = guild.AfkTimeout;
            Splash = guild.Splash;
        }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
        [JsonProperty("explicit_content_filter")]
        public int ExplicitContentFilter { get; set; }
        [JsonProperty("afk_channel_id")]
        public string AfkChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AfkTimeout { get; set; }
        //  [JsonProperty("owner_id")]
        //  public string OwnerId { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
    }

    public class ModifyGuildIcon : ModifyGuild
    {
        public ModifyGuildIcon(DiscordAPI.Models.Guilds.Guild guild) : base(guild)
        {

        }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
