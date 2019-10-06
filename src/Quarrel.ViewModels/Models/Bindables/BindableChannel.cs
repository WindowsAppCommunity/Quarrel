// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Models.Bindables.Abstract;
using Quarrel.Messages.Gateway;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Services.Rest;
using Quarrel.Services.Guild;
using Quarrel.Services.Settings;
using Quarrel.Services.Settings.Enums;
using Quarrel.Messages.Services.Settings;
using Quarrel.Messages.Navigation;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System.Collections.Concurrent;

namespace Quarrel.Models.Bindables
{
    public class BindableChannel : BindableModelBase<Channel>
    {
        private ICurrentUsersService _CurrentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private IDiscordService _DiscordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        private ISettingsService _SettingsService = SimpleIoc.Default.GetInstance<ISettingsService>();
        public IVoiceService VoiceService { get; } = SimpleIoc.Default.GetInstance<IVoiceService>();
        public IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();
        public IDispatcherHelper DispatcherHelper { get; } = SimpleIoc.Default.GetInstance<IDispatcherHelper>();

        public BindableChannel([NotNull] Channel model, [CanBeNull] IEnumerable<VoiceState> states = null) : base(model)
        {
            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, async e =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    if (e.VoiceState.ChannelId == Model.Id)
                    {
                        if (!ConnectedUsers.ContainsKey(e.VoiceState.UserId))
                        {
                            ConnectedUsers.Add(e.VoiceState.UserId, new BindableVoiceUser(e.VoiceState));
                        }
                    }
                    else if (ConnectedUsers.ContainsKey(e.VoiceState.UserId))
                    {
                        ConnectedUsers.Remove(e.VoiceState.UserId);
                    }
                });
            });

            MessengerInstance.Register<ChannelNavigateMessage>(this, async m =>
            {
                DispatcherHelper.CheckBeginInvokeOnUi(() =>
                {
                    Selected = m.Channel == this;
                });
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

            if (states != null)
            {
                foreach (var state in states)
                {
                    if (state.ChannelId == Model.Id)
                    {
                        ConnectedUsers.Add(state.UserId, new BindableVoiceUser(state));
                    }
                }
            }
        }

        public BindableGuild Guild
        {
            get => GuildsService.Guilds[GuildId];
        }

        // Order:
        //  Guild Permsissions
        //  Denies of @everyone
        //  Allows of @everyone
        //  All Role Denies
        //  All Role Allows
        //  Member denies
        //  Member allows
        public Permissions Permissions
        {
            get
            {
                if (Model is GuildChannel)
                {

                    // TODO: Calculate once and store
                    Permissions perms = Guild.Permissions.Clone();

                    var user = Guild.Model.Members.FirstOrDefault(x => x.User.Id == _DiscordService.CurrentUser.Id);

                    GuildPermission roleDenies = 0;
                    GuildPermission roleAllows = 0;
                    GuildPermission memberDenies = 0;
                    GuildPermission memberAllows = 0;
                    foreach (Overwrite overwrite in (Model as GuildChannel).PermissionOverwrites)
                        if (overwrite.Id == GuildId)
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

                    perms.AddDenies(roleDenies);
                    perms.AddAllows(roleAllows);
                    perms.AddDenies(memberDenies);
                    perms.AddAllows(memberAllows);

                    // If owner add admin
                    if (Guild.Model.OwnerId == user.User.Id)
                    {
                        perms.AddAllows(GuildPermission.Administrator);
                    }

                    return perms;
                }
                return new Permissions(int.MaxValue);
            }
        }

        #region ChannelType

        public bool IsTextChannel { get { return Model.Type == 0; } }

        public bool IsDirectChannel { get { return Model.Type == 1; } }

        public bool IsVoiceChannel { get { return Model.Type == 2; } }

        public bool IsGroupChannel { get { return Model.Type == 3; } }

        public bool IsCategory { get { return Model.Type == 4; } }

        public bool IsPrivateChannel { get { return IsDirectChannel || IsGroupChannel; } }

        #endregion

        #region Settings

        private bool _Muted;

        public bool Muted
        {
            get => _Muted;
            set => Set(ref _Muted, value);
        }

        #endregion

        #region Sorting

        private int _ParentPostion;

        public int ParentPostion
        {
            get => _ParentPostion;
            set => Set(ref _ParentPostion, value);
        }

        public ulong AbsolutePostion
        {
            get
            {
                if (IsCategory)
                    return ((ulong)Position + 1) << 32;
                else
                    return
                        ((ulong)ParentPostion + 1) << 32 |
                        ((uint)(IsVoiceChannel ? 1 : 0) << 31) |
                        (uint)(Position + 1); ;
            }
        }

        #endregion

        #region Misc

        public string GuildId;

        public string ParentId => Model is GuildChannel gcModel ? (IsCategory ? gcModel.Id : gcModel.ParentId) : null;

        public int Position => Model is GuildChannel gcModel ? gcModel.Position : 0;

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

        #endregion

        #region Display

        private bool _Selected;

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (Set(ref _Selected, value))
                    RaisePropertyChanged(nameof(Hidden));
            }
        }

        private bool _Collapsed;

        public bool Collapsed
        {
            get => _Collapsed;
            set
            {
                if (Set(ref _Collapsed, value))
                    RaisePropertyChanged(nameof(Hidden));
            }
        }

        public bool Hidden
        {
            get
            {
                bool hidden = false;
                if (_Collapsed && !IsCategory)
                {
                    hidden = true;
                    switch (_SettingsService.Roaming.GetValue<CollapseOverride>(SettingKeys.CollapseOverride))
                    {
                        case CollapseOverride.Mention:
                            if (MentionCount > 0)
                                hidden = false;
                            break;
                        case CollapseOverride.Unread:
                            if (ShowUnread)
                                hidden = false;
                            break;
                    }
                } else if (!Permissions.ReadMessages && !_SettingsService.Roaming.GetValue<bool>(SettingKeys.ShowNoPermssions))
                {
                    hidden = true;
                }
                return hidden && !Selected;
            }
        }
            

        public bool HasIcon
        {
            get
            {
                if (Model is DirectMessageChannel dmModel)
                {
                    if (IsDirectChannel)
                    {
                        return dmModel.Users[0].AvatarUri(false) != null;
                    }
                    else if (IsGroupChannel)
                    {
                        return dmModel.IconUri(false) == null;
                    }
                }

                return false;
            }
        }

        public bool ShowUnread
        {
            get => IsUnread && !Muted && Permissions.ReadMessages;
        }

        public double TextOpacity
        {
            get
            {
                if (!Permissions.ReadMessages)
                    return 0.25;
                else if (Muted)
                    return 0.35;
                else if (IsUnread)
                    return 1;
                else
                    return 0.55;
            }
        }

        public Uri ImageUri
        {
            get
            {
                if (Model is DirectMessageChannel dmModel)
                {
                    if (IsDirectChannel)
                    {
                        return dmModel.Users[0].AvatarUri();
                    }
                    else if (IsGroupChannel)
                    {
                        //TODO: detect theme
                        return dmModel.IconUri(true, true);
                    }
                }

                return null;
            }
        }

        public int FirstUserDiscriminator
        {
            get => Model is DirectMessageChannel dmModel ? (dmModel.Users.Count > 0 ? Convert.ToInt32(dmModel.Users[0].Discriminator) : 0) : 0;
        }

        public bool ShowIconBackdrop
        {
            get => IsDirectChannel && !HasIcon;
        }

        public ObservableHashedCollection<string, BindableVoiceUser> ConnectedUsers = new ObservableHashedCollection<string, BindableVoiceUser>();

        #endregion

        #region ReadState

        public bool IsUnread
        {
            get
            {
                if ((IsTextChannel || IsPrivateChannel || IsGroupChannel) && !string.IsNullOrEmpty(Model.LastMessageId))
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

        public int MentionCount
        {
            get => ReadState == null ? 0 : ReadState.MentionCount;
        }

        private ReadState _ReadState;

        public ReadState ReadState
        {
            get => _ReadState;
            set
            {
                if (Set(ref _ReadState, value))
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

        public void UpdateLRMID(string id)
        {
            if (ReadState == null)
                ReadState = new ReadState();

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
        #endregion

        #region Typing

        public bool IsTyping
        {
            get => Typers.Count > 0;
        }

        public List<string> GetNames()
        {
            List<string> names = new List<string>();
            var keys = Typers.Keys.ToList();

            foreach (var id in keys)
            {
                var typer = _CurrentUsersService.Users.TryGetValue(id, out var user) ? user.DisplayName : null;

                if (typer != null)
                {
                    names.Add(typer);
                }
            }

            return names;
        }

        public string TypingText
        {
            get
            {
                var names = GetNames();
                string typeText = "";
                for (int i = 0; i < names.Count; i++)
                {
                    if (i != 0)
                    {
                        typeText += ", ";
                    }
                    else if (i != 0 && i == names.Count-1)
                    {
                        typeText += " and ";
                    }
                    typeText += names[i];
                }

                if (names.Count > 1)
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


        public ConcurrentDictionary<string, Timer> Typers = new ConcurrentDictionary<string, Timer>();

        #endregion

    }
}
