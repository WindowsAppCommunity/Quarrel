// Special thanks to Sergio Pedri for the basis of this design

using DiscordAPI.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Quarrel.Models.Bindables.Abstract;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using Quarrel.Services;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Posts.Requests;

namespace Quarrel.Models.Bindables
{
    public class BindableMessage : BindableModelBase<Message>
    {
        private Message _previousMessage;

        public BindableMessage([NotNull] Message model, [CanBeNull] string guildId, [CanBeNull] Message previousMessage) : base(model)
        {
            GuildId = guildId;
            _previousMessage = previousMessage;
        }

        private string GuildId;

        public BindableUser Author
        {
            get => Messenger.Default.Request<BindableUserRequestMessage, BindableUser>(new BindableUserRequestMessage(Model.User.Id)) ??
                new BindableUser(new GuildMember() { User = Model.User });
        }

        #region Display

        public string AuthorName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        public int AuthorColor => Author?.TopRole?.Color ?? -1;

        public bool IsContinuation => _previousMessage != null &&
                                      _previousMessage.User.Id == Model.User.Id &&
                                      _previousMessage.Type == 0 &&
                                      Model.Timestamp.Subtract(_previousMessage.Timestamp).Minutes < 2;

        private bool showFlyout;
        public bool ShowFlyout
        {
            get => showFlyout;
            set => Set(ref showFlyout, value);
        }

        private RelayCommand showFlyoutCommand;

        public RelayCommand ShowFlyoutCommand => showFlyoutCommand ?? (showFlyoutCommand = new RelayCommand(() =>
                                                     {
                                                         ShowFlyout = true;
                                                     }));

        #endregion
    }
}
