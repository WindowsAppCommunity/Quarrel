using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion
    }
}
