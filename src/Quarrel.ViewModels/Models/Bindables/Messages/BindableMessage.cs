// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using DiscordAPI.Models.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Messages.Embeds;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Models.Emojis;
using Quarrel.ViewModels.Models.Interfaces;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.Clipboard;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Presence;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.Gateway;
using Quarrel.ViewModels.Services.Navigation;
using Quarrel.ViewModels.Services.Settings;
using Quarrel.ViewModels.ViewModels.Messages.Gateway;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Emoji = Quarrel.ViewModels.Models.Emojis.Emoji;

namespace Quarrel.ViewModels.Models.Bindables.Messages
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
        private IDictionary<string, (string, int)> _usersMentioned;
        private IDictionary<string, (string, int)> _rolesMentioned;
        private IDictionary<string, string> _channelsMentioned;
        private BindableGuildMember _author;
        private RelayCommand _openProfile;
        private RelayCommand _copyId;
        private RelayCommand _toggleEdit;
        private RelayCommand _saveEdit;
        private RelayCommand _joinCallCommand;
        private IAnalyticsService _analyticsService = null;
        private ICurrentUserService _currentUsersService = null;
        private IChannelsService _channelsService = null;
        private IDiscordService _discordService = null;
        private ISettingsService _settingsService = null;
        private IGatewayService _gatewayService = null;
        private IGuildsService _guildsService = null;
        private IPresenceService _presenceService = null;

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
            ConvertEmbeds();
            ConvertReactions();
            FindInvites();
            CalculateMentions();

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
        public RelayCommand JoinCallCommand => _joinCallCommand = new RelayCommand(async () =>
        {
            if (Model.Call != null && Model.Call.EndedTimestamp == null)
            {
                await GatewayService.Gateway.VoiceStatusUpdate(null, Model.ChannelId, false, false);

                AnalyticsService.Log(Constants.Analytics.Events.JoinCall);
            }
        });

        /// <summary>
        /// Gets a value indicating whether or not the current user is mentioned in the message.
        /// </summary>
        public bool MentionsMe => Model.MentionEveryone ||
            (Model.Mentions != null &&
            Model.Mentions.Any(x => x.Id == CurrentUsersService.CurrentUser.Model.Id));

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
        public bool ShowEdit => Model.User?.Id == CurrentUsersService?.CurrentUser?.Model.Id;

        /// <summary>
        /// Gets a value indicating whether or not the delete button should be shown in the flyout.
        /// </summary>
        public bool ShowDelete =>
            Model.User?.Id == CurrentUsersService?.CurrentUser?.Model.Id
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
        /// Gets or sets the users mentioned dictionary.
        /// </summary>
        public IDictionary<string, (string, int)> UsersMentioned
        {
            get => _usersMentioned;
            set => Set(ref _usersMentioned, value);
        }

        /// <summary>
        /// Gets or sets the roles mentioned dictionary.
        /// </summary>
        public IDictionary<string, (string, int)> RolesMentioned
        {
            get => _rolesMentioned;
            set => Set(ref _rolesMentioned, value);
        }

        /// <summary>
        /// Gets or sets the channels mentioned dictionary.
        /// </summary>
        public IDictionary<string, string> ChannelsMentioned
        {
            get => _channelsMentioned;
            set => Set(ref _channelsMentioned, value);
        }

        /// <summary>
        /// Gets or sets the UI Bindable reactions on the message.
        /// </summary>
        public ObservableCollection<BindableReaction> BindableReactions { get; set; } = new ObservableCollection<BindableReaction>();

        /// <summary>
        /// Gets or sets the UI Bindable attachments on the message.
        /// </summary>
        public ObservableCollection<BindableAttachment> BindableAttachments { get; set; } = new ObservableCollection<BindableAttachment>();

        /// <summary>
        /// Gets or sets the UI Bindable embeds on the message.
        /// </summary>
        public ObservableCollection<IEmbed> BindableEmbeds { get; set; } = new ObservableCollection<IEmbed>();

        private BindableChannel Channel => ChannelsService.GetChannel(Model.ChannelId);

        private IAnalyticsService AnalyticsService => _analyticsService ?? (_analyticsService = SimpleIoc.Default.GetInstance<IAnalyticsService>());

        private IChannelsService ChannelsService => _channelsService ?? (_channelsService = SimpleIoc.Default.GetInstance<IChannelsService>());

        private ICurrentUserService CurrentUsersService => _currentUsersService ?? (_currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>());

        private IDiscordService DiscordService => _discordService ?? (_discordService = SimpleIoc.Default.GetInstance<IDiscordService>());

        private IGatewayService GatewayService => _gatewayService ?? (_gatewayService = SimpleIoc.Default.GetInstance<IGatewayService>());

        private IGuildsService GuildsService => _guildsService ?? (_guildsService = SimpleIoc.Default.GetInstance<IGuildsService>());

        private IPresenceService PresenceService => _presenceService ?? (_presenceService = SimpleIoc.Default.GetInstance<IPresenceService>());

        private ISettingsService SettingsService => _settingsService ?? (_settingsService = SimpleIoc.Default.GetInstance<ISettingsService>());

        /// <summary>
        /// Update the message contents to <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The new base message.</param>
        public void Update(Message message)
        {
            Model = message;
            CalculateMentions();
        }

        /// <summary>
        /// Converts the <see cref="Attachment"/> to <see cref="BindableAttachment"/>.
        /// </summary>
        private void ConvertAttachments()
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
        /// Converts the <see cref="Embed"/> to <see cref="BindableEmbed"/>.
        /// </summary>
        private void ConvertEmbeds()
        {
            if (Model.Embeds != null)
            {
                foreach (var embed in Model.Embeds)
                {
                    BindableEmbeds.Add(new BindableEmbed(embed));
                }
            }
        }

        /// <summary>
        /// Converts the <see cref="Reaction"/> to <see cref="BindableReaction"/>.
        /// </summary>
        private void ConvertReactions()
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

        private async void FindInvites()
        {
            MatchCollection matches = Regex.Matches(Model.Content, Helpers.Constants.Regex.InviteRegex);
            foreach (Match match in matches)
            {
                try
                {
                    Invite invite = await DiscordService.InviteService.GetInvite(match.Groups[1].Value);
                    BindableEmbeds.Add(new BindableInvite(invite));
                }
                catch
                {
                }
            }
        }

        private void CalculateMentions()
        {
            UsersMentioned = Model.Mentions?.ToDictionary(
                x => x.Id,
                x => (x.Username, GuildsService.GetGuildMember(x.Id, GuildsService.CurrentGuild.Model.Id)?.TopRole?.Color ?? 0x18363));

            IDictionary<string, (string, int)> rolesMentionedDict = new Dictionary<string, (string, int)>();
            if (Model.MentionRoles != null)
            {
                foreach (string roleId in Model.MentionRoles)
                {
                    var role = GuildsService.CurrentGuild.Model.Roles.FirstOrDefault(x => x.Id == roleId);
                    if (role != null)
                    {
                        rolesMentionedDict.Add(roleId, (role.Name, role.Color));
                    }
                }
            }

            RolesMentioned = rolesMentionedDict;

            IDictionary<string, string> channelsMentionedDict = new Dictionary<string, string>();

            foreach (var channel in GuildsService.CurrentGuild.Channels)
            {
                channelsMentionedDict[channel.Model.Id] = channel.Model.Name;
            }

            if (Model.MentionChannels != null)
            {
                foreach (var channel in Model.MentionChannels)
                {
                    channelsMentionedDict[channel.Id] = channel.Name;
                }
            }

            ChannelsMentioned = channelsMentionedDict;
        }
    }
}
