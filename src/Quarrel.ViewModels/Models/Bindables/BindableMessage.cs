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

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private BindableChannel channel;

        private Message _previousMessage;

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, bool isLastRead = false,
            GuildMember member = null) : base(model)
        {
            GuildId = guildId;
            IsLastReadMessage = isLastRead;
            channel = SimpleIoc.Default.GetInstance<IGuildsService>().CurrentChannels[Model.ChannelId];
            author = member;

            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, async m =>
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

        private string GuildId;

        private GuildMember author;

        public BindableGuildMember Author =>
            author != null
                ? new BindableGuildMember(author) {GuildId = GuildId}
                : new BindableGuildMember(new GuildMember {User = Model.User})
                    {Presence = new Presence {Status = "offline", User = Model.User}};

        private bool _IsLastReadMessage;

        public bool IsLastReadMessage
        {
            get => _IsLastReadMessage;
            set => Set(ref _IsLastReadMessage, value);
        }

        #region Display

        public string AuthorName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        public int AuthorColor => Author?.TopRole?.Color ?? -1;

        public IEnumerable<Reaction> Reactions =>
            Model.Reactions?.Select(x =>
            {
                x.ChannelId = Model.ChannelId;
                x.MessageId = Model.Id;
                return x;
            });

        // TODO: Edit mode

        #endregion

        public void Update(Message message)
        {
            Model = message;
        }


        public bool ShowPin => !Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);

        public bool ShowUnpin => Model.Pinned && (channel.Permissions.ManageMessages || channel.IsDirectChannel);

        public bool ShowEdit => Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id;

        public bool ShowDelete =>
            Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUsersService>().CurrentUser.Model.Id
            || (channel.Permissions.ManageMessages && !channel.IsDirectChannel);
    }
}
