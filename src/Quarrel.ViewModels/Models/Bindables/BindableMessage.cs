// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using Quarrel.Models.Bindables.Abstract;
using JetBrains.Annotations;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;
using Quarrel.Services.Users;

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();

        private Message _previousMessage;

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, [CanBeNull] Message previousMessage, bool isLastRead = false) : base(model)
        {
            GuildId = guildId;
            _previousMessage = previousMessage;
            IsLastReadMessage = isLastRead;
        }

        private string GuildId;

        public BindableGuildMember Author =>
            currentUsersService.Users.TryGetValue(Model.User.Id, out BindableGuildMember value) ? value :
            (currentUsersService.DMUsers.TryGetValue(Model.User.Id, out value) ? value : new BindableGuildMember(new GuildMember() { User = Model.User }) { Presence = new Presence() { Status = "offline", User = Model.User } });

        private bool _IsLastReadMessage;

        public bool IsLastReadMessage
        {
            get => _IsLastReadMessage;
            set => Set(ref _IsLastReadMessage, value);
        }

        #region Display

        public string AuthorName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        public int AuthorColor => Author?.TopRole?.Color ?? -1;

        public bool IsContinuation => !IsLastReadMessage &&
                                      _previousMessage != null &&
                                      _previousMessage.User.Id == Model.User.Id &&
                                      _previousMessage.Type == 0 &&
                                      Model.Timestamp.Subtract(_previousMessage.Timestamp).Minutes < 2;

        // TODO: Edit mode

        #endregion

        public void Update(Message message)
        {
            Model = message;
        }
    }
}
