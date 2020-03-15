// Copyright (c) Quarrel. All rights reserved.

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.ViewModels.Models.Bindables
{
    /// <summary>
    /// A Bindable wrapper of the <see cref="Message"/> model.
    /// </summary>
    public class BindableMessage : BindableModelBase<Message>
    {
        private bool _isContinuation;
        private bool _isOldestUnreadMessage;
        private bool _isEditing;
        private string _editedText;
        private BindableGuildMember _author;
        private RelayCommand _openProfile;
        private RelayCommand _copyId;
        private RelayCommand _toggleEdit;
        private RelayCommand _saveEdit;
        private RelayCommand<List<Controls.Emoji>> _addReaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        /// <param name="model">The base <see cref="Message"/> object.</param>
        /// <param name="guildId">The id of the guild the message is in.</param>
        /// <param name="isContinuation">A value indicating whether or not the message is a continuation of the previous message.</param>
        /// <param name="isLastRead">A value indicating if the message is the first new message from the loaded.</param>
        /// <param name="member">The <see cref="BindableGuildMember"/> object of the member that posted the message.</param>
        public BindableMessage([NotNull] Message model, bool isContinuation = false, bool isLastRead = false, BindableGuildMember member = null) : base(model)
        {
            IsContinuation = isContinuation;
            IsOldestUnreadMessage = isLastRead;
            _author = member;

            ConvertAttachments();
            ConvertReactions();

            Messenger.Default.Register<GatewayGuildMembersChunkMessage>(this, m =>
            {
                if (m.GuildMembersChunk.GuildId == Channel.GuildId)
                {
                    _author = GuildsService.GetGuildMember(Model.User.Id, Channel.GuildId);
                }
            });
        }

        /// <summary>
        /// Gets a command that opens the profile of the message author.
        /// </summary>
        public RelayCommand OpenProfile => _openProfile = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<ISubFrameNavigationService>().NavigateTo("UserProfilePage", Author);
        });

        /// <summary>
        /// Gets a command that copies the message id to the clipboard.
        /// </summary>
        public RelayCommand CopyId => _copyId = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IClipboardService>().CopyToClipboard(Author.Model.User.Id);
        });

        /// <summary>
        /// Gets a command that puts the message in edit mode.
        /// </summary>
        public RelayCommand ToggleEdit => _toggleEdit = new RelayCommand(() =>
        {
            IsEditing = !IsEditing;
        });

        /// <summary>
        /// Gets a command that saves the message edits and leaves edit mode.
        /// </summary>
        public RelayCommand SaveEdit => _saveEdit = new RelayCommand(() =>
        {
            SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.EditMessage(Model.ChannelId, Model.Id, new DiscordAPI.API.Channel.Models.EditMessage() { Content = EditedText });
            IsEditing = false;
        });

        /// <summary>
        /// Gets a command that adds reactions to the message.
        /// </summary>
        public RelayCommand<List<Controls.Emoji>> AddReaction => _addReaction = new RelayCommand<List<Controls.Emoji>>((emojis) =>
        {
            foreach (Controls.Emoji emoji in emojis)
            {
                SimpleIoc.Default.GetInstance<IDiscordService>().ChannelService.CreateReaction(Model.ChannelId, Model.Id, emoji.CustomEmoji ? $"{emoji.Names[0]}:{emoji.Id}" : emoji.Surrogate);
            }
        });

        /// <summary>
        /// Gets a value indicating whether or not the current user is mentioned in the message.
        /// </summary>
        public bool MentionsMe => Model.MentionEveryone ||
            Model.Mentions.Any(x => x.Id == CurrentUserService.CurrentUser.Model.Id);

        /// <summary>
        /// Gets a value indicating whether or not the pin button should be shown in the flyout.
        /// </summary>
        public bool ShowPin => !Model.Pinned && (Channel.Permissions.ManageMessages || Channel.IsDirectChannel);

        /// <summary>
        /// Gets a value indicating whether or not the unpin button should be shown in the flyout.
        /// </summary>
        public bool ShowUnpin => Model.Pinned && (Channel.Permissions.ManageMessages || Channel.IsDirectChannel);

        /// <summary>
        /// Gets a value indicating whether or not the edit button should be shown in the flyout.
        /// </summary>
        public bool ShowEdit => Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUserService>().CurrentUser.Model.Id;

        /// <summary>
        /// Gets a value indicating whether or not the delete button should be shown in the flyout.
        /// </summary>
        public bool ShowDelete =>
            Model.User.Id == SimpleIoc.Default.GetInstance<ICurrentUserService>().CurrentUser.Model.Id
            || (Channel.Permissions.ManageMessages && !Channel.IsDirectChannel);

        /// <summary>
        /// Gets or sets the <see cref="BindableGuildMember"/> of the author.
        /// </summary>
        public BindableGuildMember Author
        {
            get => _author ?? new BindableGuildMember(new GuildMember() { User = Model.User }, "DM", PresenceService.GetUserPrecense(Model.User.Id));
            set => Set(ref _author, value);
        }

        /// <summary>
        /// Gets the display text for the author's name.
        /// </summary>
        public string AuthorDisplayName => Author != null ? Author.Model.Nick ?? Author.Model.User.Username : Model.User.Username;

        /// <summary>
        /// Gets the author's display color as an int.
        /// </summary>
        public int AuthorColor => Author?.RoleColor ?? -1;

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is the oldest unread message.
        /// </summary>
        public bool IsOldestUnreadMessage
        {
            get => _isOldestUnreadMessage;
            set => Set(ref _isOldestUnreadMessage, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is a continuation on the previous message.
        /// </summary>
        public bool IsContinuation
        {
            get => _isContinuation && !IsOldestUnreadMessage;
            set => Set(ref _isContinuation, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the message is being edited.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                EditedText = Model.Content;
                Set(ref _isEditing, value);
            }
        }

        /// <summary>
        /// Gets or sets the editing draft for the message.
        /// </summary>
        public string EditedText
        {
            get => _editedText;
            set => Set(ref _editedText, value);
        }

        /// <summary>
        /// Gets or sets the UI Bindable attachments on the message.
        /// </summary>
        public ObservableCollection<BindableAttachment> BindableAttachments { get; set; } = new ObservableCollection<BindableAttachment>();

        /// <summary>
        /// Gets or sets the UI Bindable reactions on the message.
        /// </summary>
        public ObservableCollection<BindableReaction> BindableReactions { get; set; } = new ObservableCollection<BindableReaction>();

        private BindableChannel Channel => SimpleIoc.Default.GetInstance<IChannelsService>().AllChannels[Model.ChannelId];

        private ICurrentUserService CurrentUserService => SimpleIoc.Default.GetInstance<ICurrentUserService>();

        private IGuildsService GuildsService => SimpleIoc.Default.GetInstance<IGuildsService>();

        private IPresenceService PresenceService => SimpleIoc.Default.GetInstance<IPresenceService>();

        /// <summary>
        /// Update the message contents to <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The new base message.</param>
        public void Update(Message message)
        {
            Model = message;
        }

        /// <summary>
        /// Converts the attachments to bindable attachments.
        /// </summary>
        public void ConvertAttachments()
        {
            if (Model.Attachments != null)
            {
                foreach (var attachment in Model.Attachments)
                {
                    BindableAttachments.Add(new BindableAttachment(attachment));
                }
            }
        }

        /// <summary>
        /// Converts the reactions to bindable reactions.
        /// </summary>
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
    }
}
