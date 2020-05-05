using Newtonsoft.Json;
using System.ComponentModel;

namespace DiscordAPI.Models
{
    public class SimpleInvite : INotifyPropertyChanged
    {
        private Invite _invite;
        public Invite Invite
        {
            get { return _invite; }
            set { if (_invite.Equals(value)) return; _invite = value; OnPropertyChanged("Invite"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    }

    public class Invite
    {
        /// <summary>
        /// Current use count
        /// </summary>
        [JsonProperty("uses")]
        public int Uses { get; set; }
        /// <summary>
        /// Invite lifetime in seconds
        /// </summary>
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }
        /// <summary>
        /// Maximum amount of uses in seconds
        /// </summary>
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        /// <summary>
        /// An approximation of the member count
        /// </summary>
        [JsonProperty("approximate_member_count")]
        public int MemberCount { get; set; }
        /// <summary>
        /// An approximation of the presence count
        /// </summary>
        [JsonProperty("approximate_presence_count")]
        public int OnlineCount { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        /// <summary>
        /// The user who created the invite (only username, discrimnator, avatar and id values)
        /// </summary>
        [JsonProperty("inviter")]
        public User Inviter { get; set; }
        /// <summary>
        /// The invite code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("guild")]
        public InviteGuild Guild { get; set; }
        [JsonProperty("channel")]
        public InviteChannel Channel { get; set; }
    }

    public class InviteGuild
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("splash_hash")]
        public string SplashHash { get; set; }
    }

    public class CreateInvite
    {
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("unique")]
        public bool Unique { get; set; }
    }

    public class InviteChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
