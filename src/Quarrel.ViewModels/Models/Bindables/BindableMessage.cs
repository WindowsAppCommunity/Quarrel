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
using Quarrel.Services.Rest;

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        private ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private BindableChannel channel;

        private Message _previousMessage;

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, bool isLastRead = false) : base(model)
        {
            GuildId = guildId;
            IsLastReadMessage = isLastRead;
            channel = SimpleIoc.Default.GetInstance<IGuildsService>().CurrentChannels[Model.ChannelId];
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

        public IEnumerable<Reaction> Reactions {
            get =>
                Model.Reactions == null ? null :
                Model.Reactions.Select(x =>
                {
                    x.ChannelId = Model.ChannelId;
                    x.MessageId = Model.Id;
                    return x;
                });
        }

        private RelayCommand toggleEdit;
        public RelayCommand ToggleEdit => toggleEdit = new RelayCommand(() =>
        {
            IsEditing = !IsEditing;
        });

        private RelayCommand saveEdit;
        public RelayCommand SaveEdit => saveEdit = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.EditMessage(Model.ChannelId, Model.Id, new DiscordAPI.API.Channel.Models.EditMessage() { Content = EditedText});
            IsEditing = false;
        });

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
