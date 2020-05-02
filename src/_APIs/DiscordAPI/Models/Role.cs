using Newtonsoft.Json;
using System;

namespace DiscordAPI.Models
{
    public class Role : IEquatable<Role>, IComparable<Role>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("hoist")]
        public bool Hoist { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("permissions")]
        public int Permissions { get; set; }
        [JsonProperty("managed")]
        public bool Managed { get; set; }
        [JsonProperty("mentionable")]
        public bool Mentionable { get; set; }

        [JsonIgnore]
        public int MemberCount { get; set; }


        [JsonIgnore]
        public static Role Offline => new Role() { Name = "Offline", Position = int.MinValue + 1, Id = "offline" };

        [JsonIgnore]
        public static Role Everyone => new Role() { Name = "Everyone", Position = int.MinValue + 2, Id = "everyone" };

        #region Interfaces

        /// <inheritdoc/>
        public bool Equals(Role other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj != null && obj is Role other && Equals(other);
        }

        /// <inheritdoc/>
        public int CompareTo(Role other)
        {
            return Position.CompareTo(other.Position);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


        #endregion
    }
}
