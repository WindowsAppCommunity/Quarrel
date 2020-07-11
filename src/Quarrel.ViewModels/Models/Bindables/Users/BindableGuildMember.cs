// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.API.User.Models;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
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

namespace Quarrel.ViewModels.Models.Bindables.Users
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="GuildMember"/> model.
    /// </summary>
    public class BindableGuildMember : BindableModelBase<GuildMember>,
        IEquatable<BindableGuildMember>,
        IComparable<BindableGuildMember>,
        IGuildMemberListItem,
        IBindableUser
    {
        private Presence _presence;
        private int? _userAccentColor = null;
        private List<Role> _cachedRoles;
        private RelayCommand _openProfile;
        private RelayCommand _copyId;
        private RelayCommand _messageCommand;
        private RelayCommand _changeNicknameCommand;
        private RelayCommand _sendFriendRequestCommand;
        private RelayCommand _acceptFriendRequestCommand;
        private RelayCommand _removeFriendRequestCommand;
        private RelayCommand _blockRequestCommand;
        private RelayCommand _unblockRequestCommand;
        private IAnalyticsService _analyticsService = null;
        private ICacheService _cacheService = null;
        private IChannelsService _channelsService = null;
        private ICurrentUserService _currentUserService = null;
        private IDiscordService _discordService = null;
        private IDispatcherHelper _dispatcherHelper = null;
        private IResourceService _resourceService = null;
        private IGuildsService _guildsService = null;

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
        /// Gets a command that creates and navigates to a DM channel with the member.
        /// </summary>
        public RelayCommand MessageCommand => _messageCommand = new RelayCommand(async () =>
        {
            CreateDM createDM = new CreateDM()
            {
                Recipients = new string[] { Model.User.Id },
            };

            var channel = await DiscordService.UserService.CreateDirectMessageChannelForCurrentUser(createDM);

            BindableChannel bChannel;
            bChannel = ChannelsService.GetChannel(channel.Id);
            if (bChannel == null)
            {
                bChannel = new BindableChannel(channel);
                ChannelsService.AddOrUpdateChannel(channel.Id, bChannel);
            }

            MessengerInstance.Send(new ChannelNavigateMessage(bChannel));
        });

        /// <summary>
        /// Gets a command that opens the change nickname prompt for the memeber.
        /// </summary>
        public RelayCommand ChangeNicknameCommand => _changeNicknameCommand ?? new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("ChangeNicknamePage", new Tuple<string, GuildMember>(GuildId, Model));
        });

        /// <summary>
        /// Gets a command that sends a friend request.
        /// </summary>
        public RelayCommand SendFriendRequestCommand => _sendFriendRequestCommand ?? new RelayCommand(async () =>
        {
            await DiscordService.UserService.SendFriendRequest(Model.User.Id);
        });

        /// <summary>
        /// Gets a command that accepts a friend request.
        /// </summary>
        public RelayCommand AcceptFriendRequestCommand => _acceptFriendRequestCommand ?? new RelayCommand(async () =>
        {
            await DiscordService.UserService.SendFriendRequest(Model.User.Id);
        });

        /// <summary>
        /// Gets a command that removes a friend.
        /// </summary>
        public RelayCommand RemoveFriendRequestCommand => _removeFriendRequestCommand ?? new RelayCommand(async () =>
        {
            await DiscordService.UserService.RemoveFriend(Model.User.Id);
        });

        /// <summary>
        /// Gets a command that blocks the user.
        /// </summary>
        public RelayCommand BlockRequestCommand => _blockRequestCommand ?? new RelayCommand(async () =>
        {
            await DiscordService.UserService.BlockUser(Model.User.Id);
        });

        /// <summary>
        /// Gets a command that unblocks the user.
        /// </summary>
        public RelayCommand UnblockRequestCommand => _unblockRequestCommand ?? new RelayCommand(async () =>
        {
            await DiscordService.UserService.RemoveFriend(Model.User.Id);
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
        /// Gets a value indicating whether or not the user can edit this user's nickname.
        /// </summary>
        // TODO: Handle other users
        public bool CanEditNickname => Model.User.Id == CurrentUserService.CurrentUser.Model.Id && GuildsService.CurrentGuild.Permissions.ChangeNickname;

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

                    _cachedRoles = GuildsService.GetGuild(GuildId).Model.Roles.Where(a => Model.Roles.Contains(a.Id)).OrderByDescending(x => x.Position).ToList();
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

        /// <inheritdoc/>
        public User RawModel => Model.User;

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private ICacheService CacheService => _cacheService ?? (_cacheService = SimpleIoc.Default.GetInstance<ICacheService>());

        private IChannelsService ChannelsService => _channelsService ?? (_channelsService = SimpleIoc.Default.GetInstance<IChannelsService>());

        private ICurrentUserService CurrentUserService => _currentUserService ?? (_currentUserService = SimpleIoc.Default.GetInstance<ICurrentUserService>());

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        private IDispatcherHelper DispatcherHelper => _dispatcherHelper ?? (_dispatcherHelper = SimpleIoc.Default.GetInstance<IDispatcherHelper>());

        private IResourceService ResourceService => _resourceService ?? (_resourceService = SimpleIoc.Default.GetInstance<IResourceService>());

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
