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
    internal static class GuildSyncExtentions
    {
        public static void Cache(this GuildSync sync)
        {
            #region Members
            var memberList = ServicesManager.Cache.Runtime.TryGetValue<List<BindableGuildMember>>(Quarrel.Helpers.Constants.Cache.Keys.GuildMemberList, sync.GuildId);

            if (memberList == null)
            {
                memberList = new List<BindableGuildMember>();
            }

            foreach (var user in sync.Members)
            {
                BindableGuildMember bgMember = new BindableGuildMember(user);
                bgMember.GuildId = sync.GuildId;

                if (!memberList.Contains(bgMember))
                {
                    memberList.Add(bgMember);
                }

                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildMember, bgMember, sync.GuildId + user.User.Id);
            }

            ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildMemberList, memberList, sync.GuildId);

            #endregion

            #region Presense

            foreach(var presense in sync.Presences)
            {
                ServicesManager.Cache.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, presense, presense.User.Id);
            }

            #endregion
        }
    }
}
