using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quarrel.ViewModels.Controls.Shell
{
    public class MessageBoxViewModel : ViewModelBase
    {
        #region Commands

        /// <summary>
        /// Sends API message to indicate typing state
        /// </summary>
        public RelayCommand TriggerTyping => tiggerTyping ??= new RelayCommand(() =>
        {
            DiscordService.ChannelService.TriggerTypingIndicator(ChannelsService.CurrentChannel.Model.Id);
        });
        private RelayCommand tiggerTyping;

        /// <summary>
        /// Handles enter override on MessageBox to add new line
        /// </summary>
        public RelayCommand NewLineCommand =>
            newLineCommand ??= new RelayCommand(() =>
            {
                string text = MessageText;
                int selectionstart = SelectionStart;

                if (SelectionLength > 0)
                    // Remove selected text first
                    text = text.Remove(selectionstart, SelectionLength);

                text = text.Insert(selectionstart, " \n");
                MessageText = text;
                SelectionStart = selectionstart + 2;
            });
        private RelayCommand newLineCommand;

        /// <summary>
        /// Handles enter override on MessageBox to send message
        /// </summary>
        public RelayCommand SendMessageCommand => sendMessageCommand ??= new RelayCommand(async () =>
        {
            string text = ReplaceMessageDraftSurrogates();

            await DiscordService.ChannelService.CreateMessage(ChannelsService.CurrentChannel.Model.Id,
                new DiscordAPI.API.Channel.Models.MessageUpsert() { Content = text });
            DispatcherHelper.CheckBeginInvokeOnUi(() => { MessageText = ""; });
        });
        private RelayCommand sendMessageCommand;

        // TODO: Scroll to and edit
        #endregion

        #region Methods

        /// <summary>
        /// Replaces surrogates with proper values for Emojis and Mentions
        /// </summary>
        /// <returns>Reformatted message string</returns>
        private string ReplaceMessageDraftSurrogates()
        {
            string formattedMessage = MessageText;

            // Emoji surrogates
            var emojiMatches = Regex.Matches(formattedMessage, Constants.Regex.EmojiSurrogateRegex);
            foreach (Match match in emojiMatches)
            {
                // Finds emoji by name
                DiscordAPI.Models.Emoji emoji = GuildsService.CurrentGuild.Model.Emojis.FirstOrDefault(x => x.Name == match.Groups[1].Value);

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
                BindableGuildMember user = GuildsService.GetGuildMember(match.Groups[1].Value, match.Groups[2].Value, GuildsService.CurrentGuild.Model.Id);

                // Replaces @name#disc format with <@!ID> format
                if (user != null)
                {
                    formattedMessage = formattedMessage.Replace(match.Value, string.Format("<@!{0}>", user.Model.User.Id));
                }
            }

            // Channel Mentions
            if (!GuildsService.CurrentGuild.IsDM)
            {
                var channelMentionMatches = Regex.Matches(formattedMessage, Constants.Regex.ChannelMentionSurrogateRegex);
                foreach (Match match in channelMentionMatches)
                {
                    // Finds channel by name, in current guild
                    BindableChannel channel = GuildsService.CurrentGuild.Channels.FirstOrDefault(x => x.Model.Name == match.Groups[1].Value);

                    // Replaces #channel-name
                    if (channel != null)
                    {
                        formattedMessage = formattedMessage.Replace(match.Value, string.Format("<#{0}>", channel.Model.Id));
                    }
                }
            }


            return formattedMessage;
        }


        #endregion

        #region Properties

        #region Services
        IChannelsService ChannelsService => SimpleIoc.Default.GetInstance<IChannelsService>();
        IDiscordService DiscordService => SimpleIoc.Default.GetInstance<IDiscordService>();
        IDispatcherHelper DispatcherHelper => SimpleIoc.Default.GetInstance<IDispatcherHelper>();
        IGuildsService GuildsService => SimpleIoc.Default.GetInstance<IGuildsService>();

        #endregion

        public string MessageText
        {
            get => _MessageText;
            set => Set(ref _MessageText, value);
        }
        private string _MessageText = "";

        private int _SelectionStart;

        public int SelectionStart
        {
            get => _SelectionStart;
            set => Set(ref _SelectionStart, value);
        }

        private int _SelectionLength;

        public int SelectionLength
        {
            get => _SelectionLength;
            set => Set(ref _SelectionLength, value);
        }


        #endregion
    }
}
