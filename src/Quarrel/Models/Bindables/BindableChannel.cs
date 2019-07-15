// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Models.Bindables.Abstract;
using GalaSoft.MvvmLight.Threading;
using Quarrel.Messages.Gateway;
using Quarrel.Services.Users;
using Quarrel.Services.Voice;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Rest;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

namespace Quarrel.Models.Bindables
{
    public class BindableChannel : BindableModelBase<Channel>
    {
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();  
        public IVoiceService VoiceService { get; } = SimpleIoc.Default.GetInstance<IVoiceService>();

        public BindableChannel([NotNull] Channel model, [CanBeNull] IEnumerable<VoiceState> states = null) : base(model)
        {
            MessengerInstance.Register<GatewayVoiceStateUpdateMessage>(this, async e =>
            {
                await DispatcherHelper.RunAsync(() =>
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
            MessengerInstance.Register<GatewayGuildChannelUpdatedMessage>(this, async m =>
            {
                // TODO: Complete Update
                await DispatcherHelper.RunAsync(() =>
                {
                    if (Model.Id == m.Channel.Id)
                    {
                        RaisePropertyChanged(nameof(FormattedName));
                    }
                });
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
            get => MessengerInstance.Request<BindableGuildRequestMessage, BindableGuild>(new BindableGuildRequestMessage(GuildId));
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

                    var user = Guild.Model.Members.FirstOrDefault(x => x.User.Id == discordService.CurrentUser.Id);

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

        public string ParentId
        {
            get { return Model is GuildChannel gcModel ? (IsCategory ? gcModel.Id : gcModel.ParentId ) : null; }
        }

        public int Position
        {
            get { return Model is GuildChannel gcModel ? gcModel.Position : 0; }
        }

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

        private bool _Collapsed;

        public bool Hidden
        {
            get => (IsCategory ? false : _Collapsed) ||
                !Permissions.ReadMessages;
        }

        public bool Collapsed
        {
            get => _Collapsed;
            set
            {
                if (Set(ref _Collapsed, value))
                    RaisePropertyChanged(nameof(Hidden));
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
                if (Muted)
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
                        return dmModel.IconUri(true, App.Current.RequestedTheme == Windows.UI.Xaml.ApplicationTheme.Dark);
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
                    DispatcherHelper.RunAsync(() =>
                    {
                        RaisePropertyChanged(nameof(IsUnread));
                        RaisePropertyChanged(nameof(ShowUnread));
                        RaisePropertyChanged(nameof(TextOpacity));
                    });
                }
            }
        }

        public async void UpdateLRMID(string id)
        {
            if (ReadState == null)
                ReadState = new ReadState();

            ReadState.LastMessageId = id;
            ReadState.MentionCount = 0;
            await DispatcherHelper.RunAsync(() =>
            {
                RaisePropertyChanged(nameof(IsUnread));
                RaisePropertyChanged(nameof(ShowUnread));
                RaisePropertyChanged(nameof(TextOpacity));
                RaisePropertyChanged(nameof(MentionCount));
            });
        }

        public async void UpdateLMID(string id)
        {
            Model.UpdateLMID(id);
            await DispatcherHelper.RunAsync(() => 
            {
                RaisePropertyChanged(nameof(IsUnread));
                RaisePropertyChanged(nameof(ShowUnread));
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

        public List<string> Names
        {
            get
            {
                List<string> names = new List<string>();
                foreach (var id in Typers.Keys)
                {
                    names.Add(currentUsersService.Users[id].DisplayName);
                }
                return names;
            }
        }

        public string TypingText
        {
            get
            {
                string typeText = "";
                for (int i = 0; i < Names.Count; i++)
                {
                    if (i != 0)
                    {
                        typeText += ", ";
                    }
                    typeText += Names[i];
                }

                if (Names.Count > 1)
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

        public Dictionary<string, DispatcherTimer> Typers = new Dictionary<string, DispatcherTimer>();

        #endregion

    }
}
