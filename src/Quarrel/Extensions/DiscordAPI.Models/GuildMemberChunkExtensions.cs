using CommonServiceLocator;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using DiscordAPI.Models;

namespace DiscordAPI.Models
{
    internal static class GuildMemberChunkExtentions
    {
        public static void Cache(this GuildMemberChunk chunk)
        {
            foreach (var user in chunk.Members)
            {
                BindableGuildMember bgMember = new BindableGuildMember(user);
                bgMember.GuildId = chunk.GuildId;
                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildMember, bgMember, chunk.GuildId + user.User.Id);
            }
        }
    }
}
