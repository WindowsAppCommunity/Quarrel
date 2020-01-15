// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using Quarrel.Models.Bindables.Abstract;
using JetBrains.Annotations;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Guild;
using Quarrel.Services.Users;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.Services.Rest;

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        #region Constructors

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, bool isLastRead = false,
            GuildMember member = null) : base(model)
        {
            GuildId = guildId;
            IsLastReadMessage = isLastRead;
            channel = SimpleIoc.Default.GetInstance<IGuildsService>().CurrentChannels[Model.ChannelId];
            author = member;

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

        private ICurrentUsersService CurrentUsersService { get; } = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        #endregion

        private BindableChannel channel;

        private string GuildId;

        #region Display

        #region Flyout
        public bool ShowPin => !Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);
        public bool ShowUnpin => Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);
        public bool ShowEdit => Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id;
        public bool ShowDelete =>
            Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id
            || (channel.Permissions.ManageMessages && !channel.IsDirectChannel);

        #endregion

        #region Author

        private GuildMember author;

        public BindableGuildMember Author =>
            author != null
                ? new BindableGuildMember(author) { GuildId = GuildId, Presence = CurrentUsersService.GetUserPrecense(Model.User.Id) }
                : new BindableGuildMember(new GuildMember { User = Model.User })
                    { Presence = new Presence { Status = "offline", User = Model.User } };

        public string AuthorName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        public int AuthorColor => Author?.TopRole?.Color ?? -1;

        public IEnumerable<Reaction> Reactions =>
            Model.Reactions?.Select(x =>
            {
                x.ChannelId = Model.ChannelId;
                x.MessageId = Model.Id;
                return x;
            });
            
        #endregion

        private bool _IsLastReadMessage;

        public bool IsLastReadMessage
        {
            get => _IsLastReadMessage;
            set => Set(ref _IsLastReadMessage, value);
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

        #endregion
    }
}
