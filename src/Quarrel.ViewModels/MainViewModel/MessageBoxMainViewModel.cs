// Copyright (c) Quarrel. All rights reserved.

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Models.Bindables.Messages;
using Quarrel.ViewModels.Models.Bindables.Users;
using Quarrel.ViewModels.Models.Suggesitons;
using Refit;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// Handles MessageBox control data.
    /// </summary>
    public partial class MainViewModel
    {
        private RelayCommand<BindableMessage> _quote;
        private RelayCommand<char> _tiggerTyping;
        private RelayCommand _newLineCommand;
        private RelayCommand _sendMessageCommand;
        private RelayCommand _editLastMessageCommand;

        private bool _isSending;
        private string _messageText = string.Empty;
        private int _selectionStart;
        private int _selectionLength;
        private int _queryLength;

        /// <summary>
        /// Gets the command to send an API message to indicate typing state.
        /// </summary>
        public RelayCommand<char> TriggerTyping => _tiggerTyping = _tiggerTyping ?? new RelayCommand<char>(m =>
        {
            // Not control characters
            if (m > 31 && m != 127)
            {
                _discordService.ChannelService.TriggerTypingIndicator(_channelsService.CurrentChannel.Model.Id);
            }
        });

        /// <summary>
        /// Gets the command to overrides enter on MessageBox and add new line.
        /// </summary>
        public RelayCommand NewLineCommand =>
            _newLineCommand = _newLineCommand ?? new RelayCommand(() =>
            {
                string text = MessageText;
                int selectionstart = SelectionStart;

                if (SelectionLength > 0)
                {
                    // Remove selected text first
                    text = text.Remove(selectionstart, SelectionLength);
                }

                text = text.Insert(selectionstart, " \n");
                MessageText = text;
                SelectionStart = selectionstart + 2;
            });

        /// <summary>
        /// Gets the command to override enter on MessageBox and send message.
        /// </summary>
        public RelayCommand SendMessageCommand => _sendMessageCommand = _sendMessageCommand ?? new RelayCommand(async () =>
        {
            // Enters sending state
            IsSending = true;

            string text = ReplaceMessageDraftSurrogates();
            _dispatcherHelper.CheckBeginInvokeOnUi(() => { MessageText = string.Empty; });

            ulong nonce = (ulong)new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() << 22;

            if ((!string.IsNullOrEmpty(text) && (!string.IsNullOrWhiteSpace(text))) || Attachments.Count > 0)
            {
                if (Attachments.Count == 0)
                {
                    try
                    {
                        // Send message
                        await _discordService.ChannelService.CreateMessage(_channelsService.CurrentChannel.Model.Id, new DiscordAPI.API.Channel.Models.MessageUpsert()
                                { Content = text, Nonce = nonce.ToString() });
                    }
                    catch (ApiException ex)
                    {
                        if ((int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500 && ex.HasContent)
                        {
                            int? retryAfter = JObject.Parse(ex.Content).GetValue("retry_after")?.Value<int>();
                            if (retryAfter != null)
                            {
                                await Task.Delay(retryAfter.Value);
                            }

                            try
                            {
                                await _discordService.ChannelService.CreateMessage(_channelsService.CurrentChannel.Model.Id, new DiscordAPI.API.Channel.Models.MessageUpsert()
                                        { Content = text, Nonce = nonce.ToString() });
                            }
                            catch
                            {
                                // Todo: handle failed
                            }
                        }
                    }
                }
                else
                {
                    switch (Attachments.Count)
                    {
                        case 1:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0]);
                            break;
                        case 2:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1]);
                            break;
                        case 3:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2]);
                            break;
                        case 4:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3]);
                            break;
                        case 5:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4]);
                            break;
                        case 6:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4],
                                Attachments[5]);
                            break;
                        case 7:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4],
                                Attachments[5],
                                Attachments[6]);
                            break;
                        case 8:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4],
                                Attachments[5],
                                Attachments[6],
                                Attachments[7]);
                            break;
                        case 9:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4],
                                Attachments[5],
                                Attachments[6],
                                Attachments[7],
                                Attachments[8]);
                            break;
                        case 10:
                        default:
                            await _discordService.ChannelService.UploadFile(
                                _channelsService.CurrentChannel.Model.Id,
                                text,
                                Attachments[0],
                                Attachments[1],
                                Attachments[2],
                                Attachments[3],
                                Attachments[4],
                                Attachments[5],
                                Attachments[6],
                                Attachments[7],
                                Attachments[8],
                                Attachments[9]);
                            break;
                    }

                    Attachments.Clear();
                }

            }

            _analyticsService.Log(
                Constants.Analytics.Events.SentMessage,
                ("channel type", _channelsService.CurrentChannel.Model.Type.ToString()));

            // Leaves sending state
            IsSending = false;
        });

        /// <summary>
        /// Gets the command to override up arrow and edit last sent message in chat.
        /// </summary>
        public RelayCommand EditLastMessageCommand => _editLastMessageCommand = _editLastMessageCommand ?? new RelayCommand(() =>
        {
            // Only overrides if there's no draft
            if (string.IsNullOrEmpty(MessageText))
            {
                this.ScrollToAndEditLast();
            }

            // TODO: Scroll to and edit
        });

        /// <summary>
        /// Gets a command that opens the profile of the message author.
        /// </summary>
        public RelayCommand<BindableMessage> Quote => _quote = new RelayCommand<BindableMessage>((message) =>
        {
            if (!string.IsNullOrEmpty(MessageText))
            {
                MessageText += "\n";
            }

            MessageText += "> " + message.Model.Content + "\n" + $"@{message.Model.User.Username}#{message.Model.User.Discriminator}";
        });

        /// <summary>
        /// Gets or sets a value indicating whether or not a message is currently being sent.
        /// </summary>
        public bool IsSending
        {
            get => _isSending;
            set => Set(ref _isSending, value);
        }

        /// <summary>
        /// Gets or sets the message draft.
        /// </summary>
        public string MessageText
        {
            get => _messageText;
            set
            {
                Set(ref _messageText, value);
                if (!_guildsService.CurrentGuild.IsDM)
                {
                    GetMentionQueryAndShow();
                }
            }
        }

        /// <summary>
        /// Gets or sets where the curser is in the draft.
        /// </summary>
        public int SelectionStart
        {
            get => _selectionStart;
            set => Set(ref _selectionStart, value);
        }

        /// <summary>
        /// Gets or sets how many letters are selected in selection.
        /// </summary>
        public int SelectionLength
        {
            get => _selectionLength;
            set => Set(ref _selectionLength, value);
        }

        /// <summary>
        /// Gets the current Attachments lists.
        /// </summary>
        public ObservableCollection<StreamPart> Attachments { get; } = new ObservableCollection<StreamPart>();

        /// <summary>
        /// Gets the suggested mentions based on draft.
        /// </summary>
        public ObservableCollection<ISuggestion> Suggestions { get; } = new ObservableCollection<ISuggestion>();

        /// <summary>
        /// Applies a suggestion.
        /// </summary>
        /// <param name="suggestion">Suggestion to apply.</param>
        public void SelectSuggestion(ISuggestion suggestion)
        {
            string newText = MessageText.Remove(SelectionStart - _queryLength, _queryLength);
            newText = newText.Insert(SelectionStart - _queryLength, suggestion.Surrogate + " ");
            MessageText = newText;
            SelectionStart += _queryLength;
            Suggestions.Clear();
        }

        private void GetMentionQueryAndShow()
        {
            Suggestions.Clear();
            string text = MessageText;
            if (text.Length > SelectionStart)
            {
                text = text.Remove(SelectionStart);
            }

            int loopsize = text.Length;
            int counter = 0;
            bool ranintospace = false;
            for (var i = loopsize; i > 0; i--)
            {
                counter++;
                if (counter == 32)
                {
                    // maximum username length is 32 characters, so after 32, just ignore.
                }

                var character = text[i - 1];

                if (character == '\n')
                {
                    break; // Systematically want to breaks on new lines
                }
                else if (character == ' ')
                {
                    ranintospace = true;
                }
                else if (!ranintospace && character == '#' && i != loopsize && !_guildsService.CurrentGuild.IsDM)
                {
                    // This is possibly a channel
                    string query = text.Remove(0, i);
                    _queryLength = query.Length + 1;
                    ShowSuggestions(query, 1);

                    // match the channel against the last query
                    return;
                }

                /*else if (!ranintospace && character == ':' && i != loopsize)
                {
                    // This is possibly an emoji
                    string query = text.Remove(0, i);
                    ReplacePrefix = true;
                    if (App.EmojiTrie != null)
                        DisplayList(App.EmojiTrie.Retrieve(query.ToLower()));
                    return;
                }
                */
                else if (character == '@' && i != loopsize)
                {
                    // This is possibly a user mention
                    string query = text.Remove(0, i);
                    _queryLength = query.Length;
                    ShowSuggestions(query, 0);
                    return;
                }

                /*if (!ranintospace && loopsize > 3 && i > 3 && text[i - 1] == '`' && text[i - 2] == '`' && text[i - 3] == '`')
                {
                    string query = text.Remove(0, i);
                    querylength = query.Length;
                    ReplacePrefix = false;
                    DisplayList(App.CodingLangsTrie.Retrieve(query.ToLower()));
                    Debug.WriteLine("Codeblock query is " + query);
                    return;
                }*/
            }
        }

        private void ShowSuggestions(string query, int type)
        {
            switch (type)
            {
                case 0: // User/Role
                    var members = _guildsService.QueryGuildMembers(query, _guildsService.CurrentGuild.Model.Id);
                    foreach (var member in members)
                    {
                        Suggestions.Add(new UserSuggestion(member));
                    }

                    break;
                case 1: // Channels
                    var channels = _guildsService.CurrentGuild.Channels.Where(x => x.Model.Name.ToLower().StartsWith(query.ToLower()) && x.IsTextChannel);
                    foreach (var channel in channels)
                    {
                        Suggestions.Add(new ChannelSuggestion(channel));
                    }

                    break;
            }
        }

        /// <summary>
        /// Replaces surrogates with proper values for Emojis and Mentions.
        /// </summary>
        /// <returns>Reformatted message string.</returns>
        private string ReplaceMessageDraftSurrogates()
        {
            string formattedMessage = MessageText;

            // Emoji surrogates
            var emojiMatches = Regex.Matches(formattedMessage, Constants.Regex.EmojiSurrogateRegex);
            foreach (Match match in emojiMatches)
            {
                // Finds emoji by name
                DiscordAPI.Models.Emoji emoji = _guildsService.CurrentGuild.Model.Emojis.FirstOrDefault(x => x.Name == match.Groups[1].Value);

                // Replaces :emoji_name: format with <emoji_name:id> format
                if (emoji != null)
                {
                    // Different format if animated
                    string format = emoji.Animated ? "<a:{0}:{1}>" : "<:{0}:{1}>";
                    formattedMessage = formattedMessage.Replace(match.Value, string.Format(format, emoji.Name, emoji.Id));
                }
            }

            // User mentions
            var userMentionMatches = Regex.Matches(formattedMessage, Constants.Regex.UserMentionSurrogateRegex);
            foreach (Match match in userMentionMatches)
            {
                // Finds user from Username and Discriminator
                BindableGuildMember user = _guildsService.GetGuildMember(match.Groups[1].Value, match.Groups[2].Value, _guildsService.CurrentGuild.Model.Id);

                // Replaces @name#disc format with <@!ID> format
                if (user != null)
                {
                    formattedMessage = formattedMessage.Replace(match.Value, $"<@!{user.Model.User.Id}>");
                }
            }

            // Channel Mentions
            if (!_guildsService.CurrentGuild.IsDM)
            {
                var channelMentionMatches = Regex.Matches(formattedMessage, Constants.Regex.ChannelMentionSurrogateRegex);
                foreach (Match match in channelMentionMatches)
                {
                    // Finds channel by name, in current guild
                    BindableChannel channel = _guildsService.CurrentGuild.Channels.FirstOrDefault(x => x.Model.Name == match.Groups[1].Value);

                    // Replaces #channel-name
                    if (channel != null)
                    {
                        formattedMessage = formattedMessage.Replace(match.Value, $"<#{channel.Model.Id}>");
                    }
                }
            }

            return formattedMessage;
        }
    }
}
