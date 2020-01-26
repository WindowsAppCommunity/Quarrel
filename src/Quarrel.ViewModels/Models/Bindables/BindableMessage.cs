// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        #region Constructors

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, bool isContinuation = false, bool isLastRead = false,
            GuildMember member = null) : base(model)
        {
            GuildId = guildId;
            IsContinuation = isContinuation;
            IsLastReadMessage = isLastRead;
            channel = SimpleIoc.Default.GetInstance<IChannelsService>().AllChannels[Model.ChannelId];
            author = member;

            ConvertReactions();

            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                if (m.GuildMembersChunk.GuildId == GuildId)
                {
                    GuildMember guildMember =
                        m.GuildMembersChunk.Members.FirstOrDefault(x => x.User.Id == Model.User.Id);
                    if (guildMember != null)
                        author = guildMember;
                }
            });
        }


        #endregion

        #region Properties

        #region Services

        private ICurrentUserService CurrentUserService { get; } = SimpleIoc.Default.GetInstance<ICurrentUserService>();
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

        private GuildMember author;

        public BindableGuildMember Author =>
            author != null
                ? new BindableGuildMember(author) { GuildId = GuildId, Presence = PresenceService.GetUserPrecense(Model.User.Id) }
                : new BindableGuildMember(new GuildMember { User = Model.User })
                    { Presence = new Presence { Status = "offline", User = Model.User } };

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

        #region Commands

        private RelayCommand toggleEdit;
        public RelayCommand ToggleEdit => toggleEdit = new RelayCommand(() =>
        {
            IsEditing = !IsEditing;
        });

        private RelayCommand saveEdit;
        public RelayCommand SaveEdit => saveEdit = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.EditMessage(Model.ChannelId, Model.Id, new DiscordAPI.API.Channel.Models.EditMessage() { Content = EditedText });
            IsEditing = false;
        });

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
    }
}
