// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Services;
using Quarrel.Services.Cache;
using Quarrel.Services.Guild;
using Quarrel.Services.Rest;
using Quarrel.ViewModels.Helpers;

namespace Quarrel.Models.Bindables
{
    public class BindableGuildMember : BindableModelBase<GuildMember>, IEquatable<BindableGuildMember>, IComparable<BindableGuildMember>
    {
        #region Constructors

        public BindableGuildMember([NotNull] GuildMember model) : base(model) { }

        #endregion

        #region Properties

        #region Services

        private IDiscordService discordService { get; } = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ICacheService cacheService { get; } = SimpleIoc.Default.GetInstance<ICacheService>();
        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();

        #endregion

        #region Display 

        public string DisplayName => Model.Nick ?? Model.User.Username;

        public bool IsBot => Model.User.Bot;

        public bool IsOwner { get; set; }

        public bool HasNickname => !string.IsNullOrEmpty(Model.Nick);

        public string Note => cacheService.Runtime.TryGetValue<string>(Constants.Cache.Keys.Note, Model.User.Id);

        #endregion

        #region Roles

        private List<Role> cachedRoles;
        public List<Role> Roles
        {
            get
            {
                if (cachedRoles == null)
                {
                    if (Model.Roles == null)
                        return null;

                    cachedRoles = GuildsService.Guilds[GuildId].Model.Roles.Where(a => Model.Roles.Contains(a.Id)).OrderByDescending(x => x.Position).ToList();
                }

                return cachedRoles;
            }
        }

        public Role TopRole
        {
            get
            {
                if (Roles != null)
                {
                    return Roles.OrderByDescending(x => x.Position).FirstOrDefault() ?? Role.Everyone;
                }
                return Role.Everyone;
            }
        }

        public Role TopHoistRole
        {
            get
            {
                if (Presence.Status == "offline")
                {
                    return Role.Offline;
                }

                if (Roles != null)
                {
                    return Roles.Where(x => x.Hoist).OrderByDescending(x => x.Position).FirstOrDefault() ?? Role.Everyone;
                }

                return Role.Everyone;
            }
        }

        #endregion

        public string GuildId { get; set; }

        private Presence presence;
        public Presence Presence
        {
            get => presence;
            set => Set(ref presence, value);
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

        public override int GetHashCode()
        {
            return Model.User?.Id?.GetHashCode() ?? 0;
        }

        /// <inheritdoc/>
        public int CompareTo(BindableGuildMember other)
        {
            return TopHoistRole.CompareTo(other.TopHoistRole);
        }

        #endregion
    }
}
