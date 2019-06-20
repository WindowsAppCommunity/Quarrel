// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Services;
using System.ComponentModel;
using System.Collections;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

namespace Quarrel.Models.Bindables
{
    public class BindableGuildMember : BindableModelBase<GuildMember>
    {
        public BindableGuildMember([NotNull] GuildMember model) : base(model) { }

        public string GuildId { get; set; }

        // TODO: Store, as variable, not property
        public IEnumerable<Role> Roles
        {
            get
            {
                List<Role> Roles = new List<Role>();
                foreach (var role in Model.Roles)
                {
                    Roles.Add(ServicesManager.Cache.Runtime.TryGetValue<Role>(Quarrel.Helpers.Constants.Cache.Keys.GuildRole, GuildId + role));
                }
                return Roles.OrderByDescending(x => x.Position).AsEnumerable();
            }
        }

        public Role TopRole
        {
            get => Roles.FirstOrDefault();
        }
    }
}
