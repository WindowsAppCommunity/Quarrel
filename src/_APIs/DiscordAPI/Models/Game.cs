using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models
{
    public class GameBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonIgnore]
        public bool IsCustom => Type == 4;
    }

    public class Game : GameBase
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("timestamps")]
        public TimeStamps TimeStamps{get;set; }

        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("party")]
        public party Party { get; set; }

        [JsonProperty("assets")]
        public assets Assets { get; set; }

        [JsonProperty("secrets")]
        public secrets Secrets { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("instance")]
        public bool Instance { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        public string Display => IsCustom ? State : Name;

        [JsonIgnore]
        public bool IsRich => (Details != null || State != null || Assets != null) && !IsCustom;

        [JsonIgnore]
        public bool IsXboxGame { get => ApplicationId != null && ApplicationId == "438122941302046720"; }

        [JsonIgnore]
        public string SmallImageUrl => Assets != null ? GetImageUrl(Assets.SmallImage, ApplicationId) : "";

        [JsonIgnore]
        public Uri SmallImageUri => Assets != null ? new Uri(GetImageUrl(Assets.SmallImage, ApplicationId)) : null;

        [JsonIgnore]
        public string LargeImageUrl => Assets != null ? GetImageUrl(Assets.LargeImage, ApplicationId) : "";

        [JsonIgnore]
        public Uri LargeImageUri => Assets != null ? new Uri(GetImageUrl(Assets.LargeImage, ApplicationId)) : null;

        public string GetImageUrl(string id, string gameid, bool game = false, string append = "?size=512")
        {
            // Set type in query 
            string type = "app";
            if (game) type = "game";

            // Query URL
            return "https://cdn.discordapp.com/" + type + "-assets/" + gameid + "/" + id + ".png" + append;
        }
    }
    public class TimeStamps
    {
        [JsonProperty("start")]
        public long? Start;

        [JsonProperty("end")]
        public long? End;

        [JsonIgnore]
        public DateTimeOffset StartTime => Start.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(Start.Value) : new DateTime();

        [JsonIgnore]
        public DateTimeOffset EndTime => End.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(End.Value) : new DateTime();

        [JsonIgnore]
        public TimeSpan TimeElapsed => DateTime.Now - StartTime;

        [JsonIgnore]
        public TimeSpan TimeLeft => EndTime - DateTime.Now;
    }
    public class party
    {
        [JsonProperty("size")]
        public int?[] Size { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
    public class assets
    {
        [JsonProperty("small_image")]
        public string SmallImage { get; set; }
        [JsonProperty("large_image")]
        public string LargeImage { get; set; }
        [JsonProperty("small_text")]
        public string SmallText { get; set; }
        [JsonProperty("large_text")]
        public string LargeText { get; set; }
    }
    public class secrets
    {
        [JsonProperty("join")]
        public string Join { get; set; }
        [JsonProperty("spectate")]
        public string Spectate { get; set; }
        [JsonProperty("match")]
        public string Match { get; set; }
    }
}