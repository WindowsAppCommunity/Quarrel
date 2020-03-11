// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Resources;
using Quarrel.ViewModels.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="GuildMember"/> model.
    /// </summary>
    public class BindableGuildMember : BindableModelBase<GuildMember>, IEquatable<BindableGuildMember>, IComparable<BindableGuildMember>, IGuildMemberListItem
    {
        private Presence _presence;
        private int? _userAccentColor = null;
        private List<Role> _cachedRoles;
        private RelayCommand _openProfile;
        private RelayCommand _copyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableGuildMember"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="GuildMember"/> object.</param>
        /// <param name="guildId">The Guild for the member.</param>
        /// <param name="presence">The presence of the user.</param>
        public BindableGuildMember([NotNull] GuildMember model, string guildId, Presence presence = null) : base(model)
        {
            if (model == null)
            {
                return;
            }

            GuildId = guildId;

            if (presence != null)
            {
                Presence = presence;
            }
            else
            {
                Presence = new Presence()
                {
                    User = model.User,
                    Status = "offline",
                    GuildId = guildId,
                };
            }

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

        /// <summary>
        /// Gets a command that opens the guild member's user profile.
        /// </summary>
        public RelayCommand OpenProfile => _openProfile = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("UserProfilePage", this);
        });

        /// <summary>
        /// Gets a command that copies the guild member's user id to the clipboard.
        /// </summary>
        public RelayCommand CopyId => _copyId = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(Model.User.Id);
        });

        /// <summary>
        /// Gets the name to display the guild member under.
        /// </summary>
        public string DisplayName => Model.Nick ?? Model.User.Username;

        /// <summary>
        /// Gets a value indicating whether or not the guild member is a bot.
        /// </summary>
        public bool IsBot => Model.User.Bot;

        /// <summary>
        /// Gets or sets a value indicating whether or not the guild member owns the guild.
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the member has a nickname in this guild.
        /// </summary>
        public bool HasNickname => !string.IsNullOrEmpty(Model.Nick);

        /// <summary>
        /// Gets the current user's note for the member.
        /// </summary>
        public string Note => CacheService.Runtime.TryGetValue<string>(Constants.Cache.Keys.Note, Model.User.Id);

        /// <summary>
        /// Gets display color for the guild member.
        /// </summary>
        public int RoleColor => Roles?.FirstOrDefault(x => x.Color != 0)?.Color ?? -1;

        /// <summary>
        /// Gets or sets the accent color of the guild member.
        /// </summary>
        public int AccentColor
        {
            get
            {
                if (_userAccentColor.HasValue)
                {
                    return _userAccentColor.Value;
                }

                return ResourceService.GetStatusColor(Presence.Status);
            }
            set => Set(ref _userAccentColor, value);
        }

        /// <summary>
        /// Gets all roles the guild member belongs to.
        /// </summary>
        public List<Role> Roles
        {
            get
            {
                if (_cachedRoles == null)
                {
                    if (GuildId == null || Model == null || Model.Roles == null)
                    {
                        return null;
                    }

                    _cachedRoles = GuildsService.AllGuilds[GuildId].Model.Roles.Where(a => Model.Roles.Contains(a.Id)).OrderByDescending(x => x.Position).ToList();
                }

                return _cachedRoles;
            }
        }

        /// <summary>
        /// Gets the highest role the guild member belongs to.
        /// </summary>
        public Role TopRole
        {
            get
            {
                if (Roles != null)
                {
                    return Roles.FirstOrDefault() ?? Role.Everyone;
                }

                return Role.Everyone;
            }
        }

        /// <summary>
        /// Gets the highest role with hoist status that the guild member belongs to.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the Guild the guild member applies to.
        /// </summary>
        public string GuildId { get; set; }

        /// <summary>
        /// Gets or sets the discord presence of the guild member.
        /// </summary>
        public Presence Presence
        {
            get => _presence;
            set
            {
                Set(ref _presence, value);
                RaisePropertyChanged(nameof(Game));
            }
        }

        private IDiscordService DiscordService => SimpleIoc.Default.GetInstance<IDiscordService>();

        private ICacheService CacheService => SimpleIoc.Default.GetInstance<ICacheService>();

        private IGuildsService GuildsService => SimpleIoc.Default.GetInstance<IGuildsService>();

        private IDispatcherHelper DispatcherHelper => SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        private IResourceService ResourceService => SimpleIoc.Default.GetInstance<IResourceService>();

        /// <summary>
        /// Updates the accent color for the bindable guild member.
        /// </summary>
        public async void UpdateAccentColor()
        {
            if (SimpleIoc.Default.GetInstance<ISettingsService>().Roaming.GetValue<bool>(SettingKeys.DerivedColor))
            {
                AccentColor = await GetUserDerivedColor();
            }
        }

        /// <inheritdoc/>
        public bool Equals(BindableGuildMember other) =>
            Model.User.Id == other.Model.User.Id &&
            GuildId == other.GuildId;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            return obj is BindableGuildMember other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Model.User?.Id?.GetHashCode() ?? 0;
        }

        /// <inheritdoc/>
        public int CompareTo(BindableGuildMember other)
        {
            return TopHoistRole.CompareTo(other.TopHoistRole);
        }

        private async Task<int> GetUserDerivedColor()
        {
            return await ResourceService.GetUserAccentColor(Model.User);
        }
    }
}
