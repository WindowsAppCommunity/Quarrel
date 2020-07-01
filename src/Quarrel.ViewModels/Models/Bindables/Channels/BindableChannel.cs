// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Services.Settings;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Bindables.Guilds;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Friends;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.Services.Settings.Enums;
using Quarrel.ViewModels.Services.Voice;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Quarrel.ViewModels.Messages.Voice;

namespace Quarrel.ViewModels.Models.Bindables.Channels
{
    /// <summary>
    /// A Bindable wrapper for the <see cref="Channel"/> model.
    /// </summary>
    public class BindableChannel : BindableModelBase<Channel>
    {
        private bool _muted;
        private Permissions _permissions = null;
        private ReadState _readState;
        private int _parentPostion;
        private bool _selected;
        private bool _collapsed;
        private RelayCommand _openProfile;
        private RelayCommand _markAsRead;
        private RelayCommand _mute;
        private RelayCommand _leaveGroup;
        private RelayCommand _copyId;
        private RelayCommand _startCallCommand;
        private IAnalyticsService _analyticsService = null;
        private ICurrentUserService _currentUsersService = null;
        private IChannelsService _channelsService = null;
        private IDiscordService _discordService = null;
        private ISettingsService _settingsService = null;
        private IFriendsService _friendsService = null;
        private IVoiceService _voiceService = null;
        private IGatewayService _gatewayService = null;
        private IGuildsService _guildsService = null;
        private IDispatcherHelper _dispatcherHelper = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableChannel"/> class.
        /// </summary>
        /// <param name="model">API Channel Model.</param>
        /// <param name="guildId">Id of Channel's guild.</param>
        /// <param name="states">List of VoiceStates for users in a voice channel.</param>
        public BindableChannel([NotNull] Channel model, [CanBeNull] IEnumerable<VoiceState> states = null) : base(model)
        {
            MessengerInstance.Register<GatewayUserGuildSettingsUpdatedMessage>(this, m =>
            {
                if ((m.Settings.GuildId ?? "DM") == GuildId)
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        // Updated channel settings
                        ChannelOverride channelOverride = ChannelsService.GetChannelSettings(Model.Id);
                        if (channelOverride != null)
                        {
                            Muted = channelOverride.Muted;
                        }
                    });
                }
            });

            MessengerInstance.Register<SettingChangedMessage<bool>>(this, m =>
            {
                if (m.Key == SettingKeys.ShowNoPermssions)
                {
                    RaisePropertyChanged(nameof(Hidden));
                }
            });

            MessengerInstance.Register<SettingChangedMessage<CollapseOverride>>(this, m =>
            {
                if (m.Key == SettingKeys.CollapseOverride)
                {
                    RaisePropertyChanged(nameof(Hidden));
                }
            });

            MessengerInstance.Register<SpeakMessage>(this, e =>
            {
                if (e.EventData.UserId != null && ConnectedUsers.ContainsKey(e.EventData.UserId))
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() => { ConnectedUsers[e.EventData.UserId].Speaking = e.EventData.Speaking > 0; });
                }
            });

            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (m.VoiceState.ChannelId == Model.Id)
                    {
                        if (ConnectedUsers.ContainsKey(m.VoiceState.UserId))
                        {
                            ConnectedUsers[m.VoiceState.UserId].Model = m.VoiceState;
                            ConnectedUsers[m.VoiceState.UserId].UpateProperties();
                        }
                        else
                        {
                            ConnectedUsers.Add(m.VoiceState.UserId, new BindableVoiceUser(m.VoiceState));
                        }
                    }
                    else if (ConnectedUsers.ContainsKey(m.VoiceState.UserId))
                    {
                        ConnectedUsers.Remove(m.VoiceState.UserId);
                    }
                });
            });

            if (states != null)
            {
                foreach (var state in states)
                {
                    if (state.ChannelId == Model.Id)
                    {
                        state.GuildId = GuildId;
                        ConnectedUsers.Add(state.UserId, new BindableVoiceUser(state));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="BindableGuild"/> the channel belongs to.
        /// </summary>
        public BindableGuild Guild
        {
            get => GuildsService.GetGuild(GuildId);
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is a standard text channel.
        /// </summary>
        public bool IsTextChannel => Model.Type == 0;

        /// <summary>
        /// Gets a value indicating whether or not the channel is a dm channel.
        /// </summary>
        public bool IsDirectChannel => Model.Type == 1;

        /// <summary>
        /// Gets a value indicating whether or not the channel is a voice channel.
        /// </summary>
        public bool IsVoiceChannel => Model.Type == 2;

        /// <summary>
        /// Gets a value indicating whether or not the channel is a group dm channel.
        /// </summary>
        public bool IsGroupChannel => Model.Type == 3;

        /// <summary>
        /// Gets a value indicating whether or not the channel is a category.
        /// </summary>
        public bool IsCategory => Model.Type == 4;

        /// <summary>
        /// Gets a value indicating whether or not the channel is a guild channel.
        /// </summary>
        public bool IsGuildChannel => !IsPrivateChannel;

        /// <summary>
        /// Gets the model as a <see cref="GuildChannel"/>.
        /// </summary>
        public GuildChannel AsGuildChannel => Model as GuildChannel;

        /// <summary>
        /// Gets a value indicating whether or not the channel is in a DM.
        /// </summary>
        public bool IsPrivateChannel => IsDirectChannel || IsGroupChannel;

        /// <summary>
        /// Gets the Model as a <see cref="DirectMessageChannel"/>.
        /// </summary>
        public DirectMessageChannel AsDMChannel => Model as DirectMessageChannel;

        /// <summary>
        /// Gets a value indicating whether or not the channel has typing contents.
        /// </summary>
        public bool IsTypingChannel => IsCategory || IsTextChannel || IsDirectChannel || IsGroupChannel;

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel is muted.
        /// </summary>
        public bool Muted
        {
            get => _muted;
            set
            {
                if (Set(ref _muted, value))
                {
                    RaisePropertyChanged(nameof(TextOpacity));
                    RaisePropertyChanged(nameof(ShowUnread));
                }
            }
        }

        /// <summary>
        /// Gets the current user's permissions in this channel.
        /// </summary>
        /// <remarks>
        /// Guild Permsissions
        ///  Denies of @everyone.
        ///  Allows of @everyone.
        ///  All Role Denies.
        ///  All Role Allows.
        ///  Member denies.
        ///  Member allows.
        /// </remarks>
        public Permissions Permissions
        {
            get
            {
                if (_permissions != null)
                {
                    return _permissions;
                }

                if (Model is GuildChannel)
                {
                    Permissions perms = Guild.Permissions.Clone();

                    if (ParentId != null && ParentId != Model.Id)
                    {
                        perms = ChannelsService.GetChannel(ParentId).Permissions.Clone();
                    }

                    var user = Guild.Model.Members.FirstOrDefault(x => x.User.Id == DiscordService.CurrentUser.Id);

                    GuildPermission roleDenies = 0;
                    GuildPermission roleAllows = 0;
                    GuildPermission memberDenies = 0;
                    GuildPermission memberAllows = 0;
                    foreach (Overwrite overwrite in (Model as GuildChannel).PermissionOverwrites)
                    {
                        // @everyone Id is equal to GuildId
                        if (overwrite.Type == "role" && overwrite.Id == GuildId)
                        {
                            perms.AddDenies((GuildPermission)overwrite.Deny);
                            perms.AddAllows((GuildPermission)overwrite.Allow);
                        }
                        else if (overwrite.Type == "role" && user.Roles.Contains(overwrite.Id))
                        {
                            roleDenies |= (GuildPermission)overwrite.Deny;
                            roleAllows |= (GuildPermission)overwrite.Allow;
                        }
                        else if (overwrite.Type == "member" && overwrite.Id == user.User.Id)
                        {
                            memberDenies |= (GuildPermission)overwrite.Deny;
                            memberAllows |= (GuildPermission)overwrite.Allow;
                        }
                    }

                    perms.AddDenies(roleDenies);
                    perms.AddAllows(roleAllows);
                    perms.AddDenies(memberDenies);
                    perms.AddAllows(memberAllows);

                    // If owner add admin
                    if (Guild.Model.OwnerId == user.User.Id)
                    {
                        perms.AddAllows(GuildPermission.Administrator);
                    }

                    _permissions = perms;
                    return perms;
                }

                return new Permissions(int.MaxValue);
            }
        }

        /// <summary>
        /// Gets or sets the position of the parent.
        /// </summary>
        public int ParentPostion
        {
            get => _parentPostion;
            set => Set(ref _parentPostion, value);
        }

        /// <summary>
        /// Gets the position to order the channel in.
        /// </summary>
        public ulong AbsolutePostion
        {
            get
            {
                if (IsCategory)
                {
                    return ((ulong)Position + 1) << 32;
                }
                else
                {
                    return
                        ((ulong)ParentPostion + 1) << 32 |
                        ((uint)(IsVoiceChannel ? 1 : 0) << 31) |
                        (uint)(Position + 1);
                }
            }
        }

        /// <summary>
        /// Gets the id of the guild parenting the channel.
        /// </summary>
        public string GuildId => AsGuildChannel?.GuildId ?? "DM";

        /// <summary>
        /// Gets the id of the parent category.
        /// </summary>
        public string ParentId => Model is GuildChannel gcModel ? (IsCategory ? gcModel.Id : gcModel.ParentId) : null;

        /// <summary>
        /// Gets the position within the category of the channel.
        /// </summary>
        public int Position => Model is GuildChannel gcModel ? gcModel.Position : 0;

        /// <summary>
        /// Gets the channel name formatted (case wise).
        /// </summary>
        public string FormattedName
        {
            get
            {
                switch (Model.Type)
                {
                    case 0:
                        return Model.Name.ToLower();
                    case 4:
                        return Model.Name.ToUpper();
                }

                return Model.Name;
            }
        }

        /// <summary>
        /// Gets the members of a private channel.
        /// </summary>
        public IEnumerable<BindableGuildMember> ChannelMembers
        {
            get
            {
                if (!IsDirectChannel && !IsGroupChannel)
                {
                    return null;
                }

                if (Model is DirectMessageChannel dmChannel)
                {
                    return dmChannel.Users.Select(x => FriendsService.DMUsers[x.Id]);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current is selected.
        /// </summary>
        public bool Selected
        {
            get => _selected;
            set
            {
                if (Set(ref _selected, value))
                {
                    RaisePropertyChanged(nameof(Hidden));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the channel is collapsed.
        /// </summary>
        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                if (Set(ref _collapsed, value))
                {
                    RaisePropertyChanged(nameof(Hidden));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel is hidden.
        /// </summary>
        public bool Hidden
        {
            get
            {
                bool hidden = false;

                if (IsCategory)
                {
                    return !SettingsService.Roaming.GetValue<bool>(SettingKeys.ShowNoPermssions) &&
                        !Guild.Channels
                        .Where(x => x.Model.Id != Model.Id && x.ParentId == Model.Id)
                        .Any(x => x.Permissions.ReadMessages);
                }

                if (_collapsed)
                {
                    hidden = true;
                    switch (SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride))
                    {
                        case CollapseOverride.Mention:
                            if (MentionCount > 0)
                            {
                                hidden = false;
                            }

                            break;
                        case CollapseOverride.Unread:
                            if (ShowUnread)
                            {
                                hidden = false;
                            }

                            break;
                    }
                }
                else if (!Permissions.ReadMessages && !SettingsService.Roaming.GetValue<bool>(SettingKeys.ShowNoPermssions))
                {
                    hidden = true;
                }

                return hidden && !Selected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel has an icon.
        /// </summary>
        public bool HasIcon
        {
            get
            {
                if (Model is DirectMessageChannel dmModel)
                {
                    if (IsDirectChannel)
                    {
                        return !string.IsNullOrEmpty(dmModel.Users[0].Avatar);
                    }
                    else if (IsGroupChannel)
                    {
                        return dmModel.IconUri(false) == null;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the channel should be shown as unread.
        /// </summary>
        public bool ShowUnread
        {
            get => IsUnread && !Muted && Permissions.ReadMessages;
        }

        /// <summary>
        /// Gets the text opacity to show the channel at.
        /// </summary>
        public double TextOpacity
        {
            get
            {
                if (!Permissions.ReadMessages)
                {
                    return 0.25;
                }
                else if (Muted)
                {
                    return 0.35;
                }
                else if (IsUnread)
                {
                    return 1;
                }
                else
                {
                    return 0.55;
                }
            }
        }

        /// <summary>
        /// Gets the icon url for the channel.
        /// </summary>
        public string ImageUrl
        {
            get
            {
                if (Model is DirectMessageChannel dmModel)
                {
                    if (IsDirectChannel)
                    {
                        return dmModel.Users[0].AvatarUrl;
                    }
                    else if (IsGroupChannel)
                    {
                        // TODO: detect theme
                        return dmModel.IconUrl(true, true);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the discriminator of the first user in a DM.
        /// </summary>
        public int FirstUserDiscriminator
        {
            get => Model is DirectMessageChannel dmModel ? (dmModel.Users.Count > 0 ? Convert.ToInt32(dmModel.Users[0].Discriminator) : 0) : 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not the backdrop icon should show.
        /// </summary>
        public bool ShowIconBackdrop
        {
            get => IsDirectChannel && !HasIcon;
        }

        /// <summary>
        /// Gets a collection of users connected to a voice channel.
        /// </summary>
        public ObservableHashedCollection<string, BindableVoiceUser> ConnectedUsers { get; private set; } = new ObservableHashedCollection<string, BindableVoiceUser>();

        /// <summary>
        /// Gets a value indicating whether or not the channel is unread.
        /// </summary>
        public bool IsUnread
        {
            get
            {
                if (Permissions.ReadMessages && (IsTextChannel || IsPrivateChannel || IsGroupChannel) && !string.IsNullOrEmpty(Model.LastMessageId))
                {
                    if (ReadState != null)
                    {
                        return ReadState.LastMessageId != Model.LastMessageId;
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the amount of mentions in the channel.
        /// </summary>
        public int MentionCount
        {
            get => ReadState?.MentionCount ?? 0;
        }

        /// <summary>
        /// Gets or sets the channel read state and updates related values.
        /// </summary>
        public ReadState ReadState
        {
            get => _readState;
            set
            {
                if (Set(ref _readState, value))
                {
                    DispatcherHelper.CheckBeginInvokeOnUi(() =>
                    {
                        RaisePropertyChanged(nameof(IsUnread));
                        RaisePropertyChanged(nameof(ShowUnread));
                        RaisePropertyChanged(nameof(TextOpacity));
                    });
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not someone is typing in the channel.
        /// </summary>
        public bool IsTyping => Typers.Count > 0;

        /// <summary>
        /// Gets a formatted string for the list of typing members.
        /// </summary>
        public string TypingText
        {
            get
            {
                var names = GetTypersNames();
                string typeText = string.Empty;
                for (int i = 0; i < names.Count; i++)
                {
                    if (i != 0)
                    {
                        typeText += ", ";
                    }
                    else if (i != 0 && i == names.Count - 1)
                    {
                        typeText += " and ";
                    }

                    typeText += names[i];
                }

                if (names.Count > 5)
                {
                    return "Several people are typing.";
                }
                else if (names.Count > 1)
                {
                    typeText += " are typing";
                }
                else
                {
                    typeText += " is typing";
                }

                return typeText;
            }
        }

        /// <summary>
        /// Gets a command that opens the user profile for a DM.
        /// </summary>
        public RelayCommand OpenProfile => _openProfile = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("UserProfilePage", ChannelMembers.FirstOrDefault());
        });

        /// <summary>
        /// Gets a command that marks all messages in the channel as read.
        /// </summary>
        public RelayCommand MarkAsRead => _markAsRead = new RelayCommand(async () =>
        {
            if (Model.LastMessageId != null)
            {
                await DiscordService.ChannelService.AckMessage(Model.Id, Model.LastMessageId);
            }
        });

        /// <summary>
        /// Gets a command that mutes the channel.
        /// </summary>
        public RelayCommand Mute => _mute = new RelayCommand(async () =>
        {
            // Build basic Settings Modifier
            GuildSettingModify guildSettingModify = new GuildSettingModify();
            guildSettingModify.GuildId = GuildId == "DM" ? "@me" : GuildId;
            guildSettingModify.ChannelOverrides = new Dictionary<string, ChannelOverride>();

            ChannelOverride channelOverride = ChannelsService.GetChannelSettings(Model.Id);
            if (channelOverride == null)
            {
                // No pre-exisitng channeloverride, create a default
                channelOverride = new ChannelOverride();
                channelOverride.ChannelId = Model.Id;
                channelOverride.Muted = false; // Will be swapped later
            }

            channelOverride.Muted = !channelOverride.Muted;

            // Finish Settings Modifer and send request
            guildSettingModify.ChannelOverrides.Add(Model.Id, channelOverride);
            await DiscordService.UserService.ModifyGuildSettings(guildSettingModify.GuildId, guildSettingModify);
        });

        /// <summary>
        /// Gets a command that removes the current user from a group DM.
        /// </summary>
        public RelayCommand LeaveGroup => _leaveGroup = new RelayCommand(async () =>
        {
            await DiscordService.ChannelService.DeleteChannel(Model.Id);
        });

        /// <summary>
        /// Gets a command that copies the channel id to clipboard.
        /// </summary>
        public RelayCommand CopyId => _copyId = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(Model.Id);
        });

        /// <summary>
        /// Gets a command that starts a call in the current channel.
        /// </summary>
        public RelayCommand StartCallCommand => _startCallCommand = new RelayCommand(async () =>
        {
            await DiscordService.ChannelService.StartCall(Model.Id);
            await GatewayService.Gateway.VoiceStatusUpdate(null, Model.Id, false, false);
            AnalyticsService.Log(Constants.Analytics.Events.StartCall);
        });

        /// <summary>
        /// Gets a <see cref="ConcurrentDictionary{TKey, TValue}"/> of people typing in the channel, hashed by user id.
        /// </summary>
        public ConcurrentDictionary<string, Timer> Typers { get; private set; } = new ConcurrentDictionary<string, Timer>();

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private ICurrentUserService CurrentUsersService => _currentUsersService ?? (_currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>());

        private IChannelsService ChannelsService => _channelsService ?? (_channelsService = SimpleIoc.Default.GetInstance<IChannelsService>());

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private ISettingsService SettingsService => _settingsService ?? (_settingsService = SimpleIoc.Default.GetInstance<ISettingsService>());

        private IFriendsService FriendsService => _friendsService ?? (_friendsService = SimpleIoc.Default.GetInstance<IFriendsService>());

        private IVoiceService VoiceService => _voiceService ?? (_voiceService = SimpleIoc.Default.GetInstance<IVoiceService>());

        private IGatewayService GatewayService => _gatewayService ?? (_gatewayService = SimpleIoc.Default.GetInstance<IGatewayService>());

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        private IDispatcherHelper DispatcherHelper => _dispatcherHelper ?? (_dispatcherHelper = SimpleIoc.Default.GetInstance<IDispatcherHelper>());

        /// <summary>
        /// Updates the Last Read Message ID in the channel's readstate.
        /// </summary>
        /// <param name="id">The new last read message id.</param>
        public void UpdateLRMID(string id)
        {
            if (ReadState == null)
            {
                ReadState = new ReadState();
            }

            ReadState.LastMessageId = id;
            ReadState.MentionCount = 0;
            DispatcherHelper.CheckBeginInvokeOnUi(() =>
            {
                RaisePropertyChanged(nameof(IsUnread));
                RaisePropertyChanged(nameof(ShowUnread));
                RaisePropertyChanged(nameof(Hidden));
                RaisePropertyChanged(nameof(TextOpacity));
                RaisePropertyChanged(nameof(MentionCount));
            });
        }

        /// <summary>
        /// Updates the Last Message Id for in the channel.
        /// </summary>
        /// <param name="id">The new last message id.</param>
        public void UpdateLMID(string id)
        {
            Model.UpdateLMID(id);
            DispatcherHelper.CheckBeginInvokeOnUi(() =>
            {
                RaisePropertyChanged(nameof(IsUnread));
                RaisePropertyChanged(nameof(ShowUnread));
                RaisePropertyChanged(nameof(Hidden));
                RaisePropertyChanged(nameof(TextOpacity));
                RaisePropertyChanged(nameof(MentionCount));
            });
        }

        /// <summary>
        /// Gets the names of the people typing in a channel.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of members typing in the channel.</returns>
        public List<string> GetTypersNames()
        {
            List<string> names = new List<string>();
            var keys = Typers.Keys.ToList();

            foreach (var id in keys)
            {
                var user = GuildsService.GetGuildMember(id, GuildId);
                if (user != null)
                {
                    names.Add(user.DisplayName);
                }
            }

            return names;
        }
    }
}
