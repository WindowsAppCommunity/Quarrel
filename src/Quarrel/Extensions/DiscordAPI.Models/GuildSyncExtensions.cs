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
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;

namespace DiscordAPI.Models
{
    internal static class GuildSyncExtentions
    {
        private static ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        public static void Cache(this GuildSync sync)
        {
            #region Presense

            foreach(var presense in sync.Presences)
            {
                cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Presence, presense, presense.User.Id);
            }

            #endregion
        }
    }
}
