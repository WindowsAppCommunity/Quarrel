// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableGuildMember : BindableModelBase<GuildMember>, IEquatable<BindableGuildMember>, IComparable<BindableGuildMember>, IGuildMemberListItem
    {
        #region Constructors

        public BindableGuildMember([NotNull] GuildMember model, string guildId, Presence presence = null) : base(model)
        {
            GuildId = guildId;

            if (presence != null)
                Presence = presence;
            else
                Presence = new Presence()
                {
                    User = model.User,
                    Status = "offline",
                    GuildId = guildId
                };

            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (m.UserId == Model.User.Id)
                    {
                        Presence = m.Presence;
                    }
                });
            });
        }

        #endregion

        #region Properties

        #region Services

        private readonly IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private readonly ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private readonly IGuildsService GuildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        private readonly IDispatcherHelper DispatcherHelper = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        #endregion

        #region Display 

        public Game Game => Presence?.Game;
        
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
                    if (GuildId == null || Model == null || Model.Roles == null)
                        return null;

                    cachedRoles = GuildsService.AllGuilds[GuildId].Model.Roles.Where(a => Model.Roles.Contains(a.Id)).OrderByDescending(x => x.Position).ToList();
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

        private Presence _Presence;
        public Presence Presence
        {
            get => _Presence;
            set
            {
                Set(ref _Presence, value);
                RaisePropertyChanged(nameof(Game));
            }
        }

        #endregion

        #region Interfaces

        /// <inheritdoc/>
        public bool Equals(BindableGuildMember other) =>
            Model.User.Id == other.Model.User.Id &&
            GuildId == other.GuildId;

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
