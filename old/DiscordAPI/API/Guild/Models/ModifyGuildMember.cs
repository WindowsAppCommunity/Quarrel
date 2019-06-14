using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Guild.Models
{
    public class IModifyGuildMember
    {
        [JsonProperty("nick")]
        public string Nick { get; set; }
    }
    public class ModifyGuildMember : IModifyGuildMember
    {
        public ModifyGuildMember(string nickname)
        {
            Nick = nickname;
        }
        public ModifyGuildMember(GuildMember member)
        {
            Nick = member.Nick;
            Roles = member.Roles;
            Mute = member.Mute;
            Deaf = member.Deaf;
            ChannelId = null;
        }

        public bool TryAddRole(string roleId)
        {
            if (!Roles.Contains(roleId))
            {
                var roles = Roles.ToList();
                roles.Add(roleId);
                Roles = roles.AsEnumerable();
                return true;
            }
            return false;
        }

        public bool TryRemoveRole(string roleId)
        {
            if (Roles.Contains(roleId))
            {
                var roles = Roles.ToList();
                roles.Remove(roleId);
                Roles = roles.AsEnumerable();
                return true;
            }
            return false;
        }

        public void ToggleRole(string roleId)
        {
            var roles = Roles.ToList();
            if (Roles.Contains(roleId))
            {
                roles.Remove(roleId);
            } else
            {
                roles.Add(roleId);
            }
            Roles = roles.AsEnumerable();
        }


        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }
        [JsonProperty("mute")]
        public bool Mute { get; set; }
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}
