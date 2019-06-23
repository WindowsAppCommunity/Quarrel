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
using Windows.UI;
using Windows.UI.Xaml.Media;
using Quarrel.Converters.UI.Discord;

namespace Quarrel.Models.Bindables
{
    public class BindableGuildMember : BindableModelBase<GuildMember>, IEquatable<BindableGuildMember>, IComparable<BindableGuildMember>
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
            get => Roles.FirstOrDefault() ?? new Role() { Name = "Everyone" };
        }

        public Role TopHoistRole
        {
            get => Roles.FirstOrDefault(x => x.Hoist) ?? new Role() { Name = "Everyone" };
        }

        public Presence Presence
        {
            get => ServicesManager.Cache.Runtime.TryGetValue<Presence>(Quarrel.Helpers.Constants.Cache.Keys.Presence, Model.User.Id) ?? new Presence() { Status = "offline", User = Model.User };
        }

        #region Display 

        public string DisplayName
        {
            get
            {
                return Model.Nick ?? Model.User.Username;
            }
        }

        public SolidColorBrush PresenceBrush
        {
            get
            {
                if (Presence.Status == "offline" || Presence.Status == "invisible")
                    return new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
                else
                    return (SolidColorBrush)new PresenseToBrushConverter().Convert(Presence.Status, null, null, null);
            }
        }

        public bool HasNickname
        {
            get => !string.IsNullOrEmpty(Model.Nick);
        }

        #endregion

        #region Interfaces

        /// <inheritdoc/>
        public bool Equals(BindableGuildMember other) => Model.User.Id.Equals(other.Model.User.Id);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            return obj is BindableGuildMember other && Equals(other);

        }

        /// <inheritdoc/>
        public int CompareTo(BindableGuildMember other)
        {
            return TopHoistRole.CompareTo(other.TopHoistRole);
        }

        #endregion
    }
}
