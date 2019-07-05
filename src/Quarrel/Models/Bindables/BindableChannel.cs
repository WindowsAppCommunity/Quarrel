// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using JetBrains.Annotations;
using System;
using Quarrel.Models.Bindables.Abstract;
using GalaSoft.MvvmLight.Threading;

namespace Quarrel.Models.Bindables
{
    public class BindableChannel : BindableModelBase<Channel>
    {
        public BindableChannel([NotNull] Channel model) : base(model) { }

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
                    return (ulong)Position << 32;
                else
                    return
                        (ulong)ParentPostion << 32 |
                        ((uint)(IsVoiceChannel ? 1 : 0) << 31) |
                        (uint)(Position + 1); ;
            }
        }

        #endregion

        #region Misc
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
            get => IsCategory ? false : _Collapsed;
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
            get => IsUnread && !Muted;
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
    }
}
