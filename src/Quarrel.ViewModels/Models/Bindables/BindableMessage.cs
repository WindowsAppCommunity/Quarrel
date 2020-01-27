// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        #region Constructors

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, bool isContinuation = false, bool isLastRead = false,
            BindableGuildMember member = null) : base(model)
        {
            GuildId = guildId;
            IsContinuation = isContinuation;
            IsLastReadMessage = isLastRead;
            channel = SimpleIoc.Default.GetInstance<IChannelsService>().AllChannels[Model.ChannelId];
            _Author = member;

            ConvertReactions();

            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                if (m.GuildMembersChunk.GuildId == GuildId)
                {
                    _Author = GuildsService.GetGuildMember(Model.User.Id, GuildId);
                }
            });
        }


        #endregion

        #region Commands

        public RelayCommand OpenProfile => openProfile = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("UserProfilePage", Author);
        });
        private RelayCommand openProfile;

        public RelayCommand CopyId => copyId = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(Author.Model.User.Id);
        });
        private RelayCommand copyId;

        public RelayCommand ToggleEdit => toggleEdit = new RelayCommand(() =>
        {
            IsEditing = !IsEditing;
        });
        private RelayCommand toggleEdit;

        public RelayCommand SaveEdit => saveEdit = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.EditMessage(Model.ChannelId, Model.Id, new DiscordAPI.API.Channel.Models.EditMessage() { Content = EditedText });
            IsEditing = false;
        });
        private RelayCommand saveEdit;

        #endregion

        #region Methods

        public void Update(Message message)
        {
            Model = message;
        }

        public void ConvertReactions()
        {
            if (Model.Reactions != null)
            {
                foreach (var reaction in Model.Reactions)
                {
                    reaction.ChannelId = Model.ChannelId;
                    reaction.MessageId = Model.Id;
                    BindableReactions.Add(new BindableReaction(reaction));
                }
            }
        }

        #endregion

        #region Properties

        #region Services

        private ICurrentUserService CurrentUserService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();
        private IGuildsService GuildsService { get; } = SimpleIoc.Default.GetInstance<IGuildsService>();
        private IPresenceService PresenceService { get; } = SimpleIoc.Default.GetInstance<IPresenceService>();

        #endregion

        private BindableChannel channel;

        private string GuildId;

        #region Display

        #region Flyout
        public bool ShowPin => !Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);
        public bool ShowUnpin => Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);
        public bool ShowEdit => Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUserService>().CurrentUser.Model.Id;
        public bool ShowDelete =>
            Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUserService>().CurrentUser.Model.Id
            || (channel.Permissions.ManageMessages && !channel.IsDirectChannel);

        #endregion

        #region Author


        public BindableGuildMember Author
        {
            get => _Author;
            set => Set(ref _Author, value);
        }
        private BindableGuildMember _Author;

        public string AuthorName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        public int AuthorColor => Author?.TopRole?.Color ?? -1;

        #endregion

        private bool _IsLastReadMessage;

        public bool IsLastReadMessage
        {
            get => _IsLastReadMessage;
            set => Set(ref _IsLastReadMessage, value);
        }

        private bool _IsContinuation;
        public bool IsContinuation
        {
            get => _IsContinuation && !IsLastReadMessage;
            set => Set(ref _IsContinuation, value);
        }

        #region Editing

        private string editedText;
        public string EditedText
        {
            get => editedText;
            set => Set(ref editedText, value);
        }

        private bool isEditing;
        public bool IsEditing
        {
            get => isEditing;
            set
            {
                EditedText = Model.Content;
                Set(ref isEditing, value);
            }

        }

        #endregion

        public ObservableCollection<BindableReaction> BindableReactions { get; set; } = new ObservableCollection<BindableReaction>();

        #endregion

        #endregion
    }
}
