using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.Devices.Notification;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using DiscordAPI.API.Channel.Models;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using Quarrel.SimpleClasses;
using Quarrel.SubPages;
using Debug = System.Diagnostics.Debug;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Quarrel.Controls
{
    public sealed partial class MessageBodyControl : UserControl
    {
        /// <summary>
        /// ScrollView for MessageList
        /// </summary>
        private ScrollViewer _messageScrollviewer;

        /// <summary>
        /// ItemStack for MessageList
        /// </summary>
        private ItemsStackPanel _messageStacker;

        /// <summary>
        /// True if the oldest unread message is out of view
        /// </summary>
        private bool _outofboundsNewMessage;

        /// <summary>
        /// True while loading new message to avoid loading twice
        /// </summary>
        private bool DisableLoadingMessages;

        /// <summary>
        /// Timer for tracking typing status
        /// </summary>
        private readonly DispatcherTimer typingCooldown = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };


        /// <summary>
        /// Id of open channel
        /// </summary>
        private string ChannelId;

        /// <summary>
        /// True if the open channel is from DMs
        /// </summary>
        private bool CurrentGuildIsDM;

        /// <summary>
        /// Id of guild open channel is from
        /// </summary>
        private string CurrentGuildId;

        /// <summary>
        /// True if UIElement is in People Taskbar view
        /// </summary>
        private bool isMypeople;

        public MessageBodyControl()
        {
            InitializeComponent();
            typingCooldown.Tick += TypingCooldown_Tick;
            App.SetupMainPage += Setup;
            App.LoggingInHandler += App_LoggingInHandlerAsync;
        }

        public string MyPeopleChannelId
        {
            set
            {
                // Setup UI
                isMypeople = true;
                ChannelId = value;
                CurrentGuildIsDM = true;
                App_LoggingInHandlerAsync(null, null);
                RenderMessages();

                App_LoadDraft(null, null);
                MessageBox1.FocusTextBox();
                UpdateTyping();
            }
        }

        /// <summary>
        /// Setup cinematic padding
        /// </summary>
        public async void Setup(object o, EventArgs args)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Add padding if on Xbox
                if (App.CinematicMode)
                {
                    MessageList.Padding = new Thickness(0, 84, 0, 0);
                }
            });
        }

        /// <summary>
        /// Jump ListView to last read message
        /// </summary>
        private void GoToLastRead_Click(object sender, RoutedEventArgs e)
        {
            LoadMessagesAround(App.LastReadMsgId);
        }

        /// <summary>
        /// Hide new message indicator
        /// </summary>
        private void IgnoreNewMessages_Click(object sender, RoutedEventArgs e)
        {
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Scroll to bottom
        /// </summary>
        private void ReturnToPresent_Click(object sender, RoutedEventArgs e)
        {
            RenderMessages();
        }

        /// <summary>
        /// UI Initialized
        /// </summary>
        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _messageScrollviewer = Common.GetScrollViewer(MessageList);
            if (_messageScrollviewer != null) _messageScrollviewer.ViewChanged += MessageScrollviewer_ViewChanged;
            _messageStacker = sender as ItemsStackPanel;
        }

        /// <summary>
        /// Rescale UI and maintain scroll
        /// </summary>
        private void ItemStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_messageScrollviewer != null && _messageStacker != null &&
                _messageStacker.ItemsUpdatingScrollMode == ItemsUpdatingScrollMode.KeepLastItemInView)
                if (MessageList.Items.Count > 0 &&
                    _messageScrollviewer.VerticalOffset + 24 > _messageScrollviewer.ExtentHeight)
                    _messageScrollviewer.ChangeView(null, _messageScrollviewer.ExtentHeight, null, true);
        }

        /// <summary>
        /// Send message
        /// </summary>
        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            // Create message
            string text = MessageBox1.Text;
            App.CreateMessage(ChannelId, text);

            // Clear draft
            MessageBox1.Text = "";
            MessageBox1.FocusTextBox();

            //Add a user activity for this channel:
            Task.Run(async () =>
            {
                if (CurrentGuildIsDM)
                    await UserActivityManager.GenerateActivityAsync(LocalState.CurrentDMChannel.Id, LocalState.CurrentDMChannel.Name,
                        Common.GetChannelIconUriString(LocalState.CurrentDMChannel.Id, LocalState.CurrentDMChannel.Icon));
                else
                    await UserActivityManager.GenerateActivityAsync(LocalState.CurrentGuild.Raw.Id,
                        LocalState.CurrentGuild.Raw.Name, Common.GetGuildIconUriString(LocalState.CurrentGuild.Raw.Id, LocalState.CurrentGuild.Raw.Icon),
                        LocalState.CurrentGuildChannel.raw.Id, "#" + LocalState.CurrentGuildChannel.raw.Name);
            });
        }

        /// <summary>
        /// Timer end reached
        /// </summary>
        private void TypingCooldown_Tick(object sender, object e)
        {
            typingCooldown.Stop();
        }

        /// <summary>
        /// Trigger typing indicator
        /// </summary>
        private async void TypingStarted(object sender, TextChangedEventArgs e)
        {
            if (!typingCooldown.IsEnabled)
            {
                // Send indication
                await RESTCalls.TriggerTypingIndicator(ChannelId);

                // Track indicator
                typingCooldown.Start();
            }
        }

        /// <summary>
        /// Open in advanced editor
        /// </summary>
        private void MessageBox1_OpenAdvanced(object sender, MessageBox.OpenAdvancedArgs e)
        {
            // Nav to Advanced editor
            if (e == null)
            {
                App.NavigateToMessageEditor(MessageBox1.Text, false);
            }
            else
            {
                App.NavigateToMessageEditor(e.Content, e.Paste);
            }

            // Clear local draft
            MessageBox1.Text = "";
        }

        /// <summary>
        /// Open spotify share UI
        /// </summary>
        private void MessageBox1_OpenSpotify(object sender, RoutedEventArgs e)
        {
            // Nav to Spotify sharing UI
            SubFrameNavigator(typeof(SpotifyShare));
        }

        /// <summary>
        /// Load the messages around a message (by id)
        /// </summary>
        /// <param name="id">Middle message</param>
        private async void LoadMessagesAround(string id)
        {
            try
            {
                if (!LastMessageIsLoaded())
                {
                    MessagesLoadingTop.Visibility = Visibility.Visible;
                    MessageList.Items.Clear();
                    DisableLoadingMessages = true;
                    List<MessageContainer> messages =
                        await MessageManager.ConvertMessage(
                            (await RESTCalls.GetChannelMessagesAround(ChannelId, id)).ToList());
                    AddMessages(Position.After, true, messages, true);
                    MessagesLoadingTop.Visibility = Visibility.Collapsed;
                    await Task.Delay(1000);
                    DisableLoadingMessages = false;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Relative addition positions
        /// </summary>
        public enum Position
        {
            Before,
            After
        }

        /// <summary>
        /// Add message to ListView
        /// </summary>
        /// <param name="position">Position relative to current objects</param>
        /// <param name="scroll">Ture if it shold be scrolled to</param>
        /// <param name="messages">List of messages to add</param>
        /// <param name="showNewMessageIndicator">True if the New Message Indicator should be displayed</param>
        public void AddMessages(Position position, bool scroll, List<MessageContainer> messages,
            bool showNewMessageIndicator)
        {
            // Debug assist
            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Clear overlay indicators (for now)
            ReturnToPresentIndicator.Visibility = Visibility.Collapsed;
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;

            if (messages != null && messages.Count > 0 && messages[0].Message.ChannelId == ChannelId)
            {
                // The Message to scroll to once loaded
                MessageContainer scrollTo = null;

                if (showNewMessageIndicator)
                {
                    // (MAYBE) SHOW NEW MESSAGE INDICATOR
                    DateTimeOffset firstMessageTime = Common.SnowflakeToTime(messages.First().Message.Id);
                    DateTimeOffset lastMessageTime = Common.SnowflakeToTime(messages.Last().Message.Id);
                    DateTimeOffset lastReadTime = Common.SnowflakeToTime(App.LastReadMsgId);

                    if (firstMessageTime < lastReadTime)
                    {
                        // The last read message is after the first one in the list
                        if (lastMessageTime > lastReadTime)
                        {
                            _outofboundsNewMessage = false;
                            // The last read message is before the last one in the list
                            bool canBeLastRead = true;
                            for (int i = 0; i < messages.Count(); i++)
                            {
                                if (canBeLastRead)
                                {
                                    // The first one with a larger timestamp gets the "NEW MESSAGES" header
                                    DateTimeOffset currentMessageTime = Common.SnowflakeToTime(messages[i].Message.Id);
                                    if (currentMessageTime > lastReadTime)
                                    {
                                        messages[i].LastRead = true;
                                        scrollTo = messages[i];
                                        canBeLastRead = false;
                                    }
                                }

                                if (position == Position.After)
                                    MessageList.Items.Add(messages[i]);
                                else if (position == Position.Before)
                                    MessageList.Items.Insert(i, messages[i]);
                            }
                        }
                        else
                        {
                            // The last read message is after the span of currently displayed messages
                            _outofboundsNewMessage = true;
                            for (int i = 0; i < messages.Count(); i++)
                                if (position == Position.After)
                                    MessageList.Items.Add(messages[i]);
                                else if (position == Position.Before)
                                    MessageList.Items.Insert(i, messages[i]);
                        }
                    }
                    else
                    {
                        // The last read message is before the first one in the list
                        _outofboundsNewMessage = true;
                        for (int i = 0; i < messages.Count(); i++)
                            if (position == Position.After)
                                MessageList.Items.Add(messages[i]);
                            else if (position == Position.Before)
                                MessageList.Items.Insert(i, messages[i]);
                        scrollTo = messages.First();
                        MoreNewMessageIndicator.Opacity = 0;
                        MoreNewMessageIndicator.Visibility = Visibility.Visible;
                        MoreNewMessageIndicator.Fade(1, 300).Start();
                    }
                }
                else
                {
                    // DO NOT SHOW NEW MESSAGE INDICATOR. Just add everything before or after
                    for (int i = 0; i < messages.Count(); i++)
                        if (position == Position.After)
                            MessageList.Items.Add(messages[i]);
                        else if (position == Position.Before)
                            MessageList.Items.Insert(i, messages[i]);
                }

                if (scroll && scrollTo != null) MessageList.ScrollIntoView(scrollTo, ScrollIntoViewAlignment.Leading);
            }

            // Determine visibility of overlay indicators
            Message last = MessageList.Items.Count > 0 ? (MessageList.Items.Last() as MessageContainer).Message : null;
            if (last != null && CurrentGuildId != null && ChannelId != null && LocalState.CurrentGuild.channels.ContainsKey(ChannelId) && LocalState.CurrentGuild.channels[ChannelId].raw.LastMessageId != null && Convert.ToInt64(last.Id) >
                Convert.ToInt64(LocalState.CurrentGuild.channels[ChannelId].raw.LastMessageId))
            {
                ReturnToPresentIndicator.Opacity = 1;
                ReturnToPresentIndicator.Visibility = Visibility.Visible;
                ReturnToPresentIndicator.Fade(1, 300).Start();
            }
            else
            {
                ReturnToPresentIndicator.Visibility = Visibility.Collapsed;
            }

            // Debug assit
            sw.Stop();
            Debug.WriteLine("Messages took " + sw.ElapsedMilliseconds + "ms to load");
        }

        /// <summary>
        /// Render base message list
        /// </summary>
        public async void RenderMessages()
        {
            // Show loading indicator
            MessagesLoading.Visibility = Visibility.Visible;

            // Clear view
            _outofboundsNewMessage = false;
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            MessageList.Items.Clear();
            
            // Retrieve message list
            IEnumerable<Message> emessages = null;
            await Task.Run(async () => { emessages = await RESTCalls.GetChannelMessages(ChannelId); });

            // Display messages
            if (emessages != null)
            {
                List<MessageContainer> messages = await MessageManager.ConvertMessage(emessages.ToList());
                AddMessages(Position.After, true, messages, true);
            }

            // Hide loading indicator
            MessagesLoading.Visibility = Visibility.Collapsed;
        }

        
        /// <summary>
        /// Determines if the Last message is still loaded
        /// </summary>
        /// <returns>True if the Last message is still loaded</returns>
        private bool LastMessageIsLoaded()
        {
            // TODO: Improve this code

            if (CurrentGuildIsDM)
            {
                for (int i = MessageList.Items.Count; i < 0; i--)
                    if (((MessageContainer) MessageList.Items[i]).Message != null)
                    {
                        if (((MessageContainer) MessageList.Items[i]).Message.Id ==
                            LocalState.DMs[ChannelId].LastMessageId)
                            return true;
                        return false;
                    }

                return false;
            }
            
            for (int i = MessageList.Items.Count; i < 0; i--)
                if (((MessageContainer) MessageList.Items[i]).Message != null)
                {
                    if (((MessageContainer) MessageList.Items[i]).Message.Id == LocalState.Guilds[CurrentGuildId]
                            .channels[ChannelId].raw.LastMessageId)
                        return true;
                    return false;
                }

            return false;
        }
        
        /// <summary>
        /// Determine if new messages should be loaded
        /// </summary>
        private void MessageScrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (MessageList.Items.Count > 0)
            {
                // Distance from top
                double fromTop = _messageScrollviewer.VerticalOffset;
                
                //Distance from bottom
                double fromBottom = _messageScrollviewer.ScrollableHeight - fromTop;

                // Load messages
                if (fromTop < 100 && !DisableLoadingMessages)
                    LoadOlderMessages();
                if (fromBottom < 200 && !DisableLoadingMessages)
                    LoadNewerMessages();
            }
        }

        /// <summary>
        /// Load older messages
        /// </summary>
        private async void LoadOlderMessages()
        {
            // Prevent a second call 
            DisableLoadingMessages = true;

            // Show loading indicator
            MessagesLoadingTop.Visibility = Visibility.Visible;

            // Get messages
            List<MessageContainer> messages = await MessageManager.ConvertMessage(
                (await RESTCalls.GetChannelMessagesBefore(ChannelId,
                    (MessageList.Items.FirstOrDefault(x => (x as MessageContainer).Message != null) as MessageContainer)
                    .Message.Id)).ToList());

            // Display Messages
            AddMessages(Position.Before, false, messages,
                _outofboundsNewMessage); //if there is an out of bounds new message, show the indicator. Otherwise, don't.

            // Hide loaidng indicator
            MessagesLoadingTop.Visibility = Visibility.Collapsed;

            // Buffer perioud for loading more messages
            await Task.Delay(1000);
            DisableLoadingMessages = false;
        }

        /// <summary>
        /// Load newer messages
        /// </summary>
        private async void LoadNewerMessages()
        {
            try
            {
                // Check if there are newer messages
                Message last = (MessageList.Items.Last() as MessageContainer)?.Message;
                if (last != null && LocalState.RPC.ContainsKey(ChannelId) && last.Id != LocalState.RPC[ChannelId].LastMessageId)
                {
                    // Show loading indicator
                    MessagesLoading.Visibility = Visibility.Visible;

                    // Prevent a second call
                    DisableLoadingMessages = true;

                    // Get Messages
                    List<MessageContainer> messages = await MessageManager.ConvertMessage(
                        (await RESTCalls.GetChannelMessagesAfter(ChannelId,
                            (MessageList.Items.LastOrDefault(x => (x as MessageContainer).Message != null) as
                                MessageContainer).Message.Id)).ToList());
                    _messageStacker.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;

                    // Display messages
                    AddMessages(Position.After, false, messages,
                        _outofboundsNewMessage); //if there is an out of bounds new message, show the indicator. Otherwise, don't.
                    _messageStacker.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;


                    // Hide loading indicator 
                    MessagesLoading.Visibility = Visibility.Collapsed;

                    // Buffer perioud for loading more messages
                    await Task.Delay(1000);
                    DisableLoadingMessages = false;
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Add mention to message
        /// </summary>
        private void App_MentionHandler(object sender, App.MentionArgs e)
        {
            if (!Char.IsWhiteSpace(MessageBox1.Text.Last()))
            {
                // Add white space if the last character is no white space
                MessageBox1.Text += " ";
            }

            // Add mention
            MessageBox1.Text += "@" + e.Username + "#" + e.Discriminator;
        }

        /// <summary>
        /// Handle logging in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_LoggingInHandlerAsync(object sender, EventArgs e)
        {
            if (App.LoggedIn())
            {
                // App events
                App.LogOutHandler += App_LogOutHandler;
                App.MentionHandler += App_MentionHandler;
                App.SaveDraftHandler += App_SaveDraft;
                App.SaveDraftHandler += App_LoadDraft;
                if (!isMypeople)
                {
                    App.CreateMessageHandler += App_CreateMessageHandler;
                }
                App.DeleteMessageHandler += App_DeleteMessageHandler;

                // API events
                App.MessageCreatedHandler += App_MessageCreatedHandler;
                App.MessageDeletedHandler += App_MessageDeletedHandler;
                //UpdateUI-Messages
                App.MessageEditedHandler += App_MessageEditedHandler;
                App.NavigateToGuildChannelHandler += App_NavigateToGuildChannelHandler;
                App.NavigateToDMChannelHandler += App_NavigateToDMChannelHandler;
                App.TypingHandler += App_TypingHandler;
            }
        }

        /// <summary>
        /// Unsubscribe from all events
        /// </summary>
        private void ClearEvents()
        {
            // App events
            App.LogOutHandler -= App_LogOutHandler;
            App.MentionHandler -= App_MentionHandler;
            App.SaveDraftHandler -= App_SaveDraft;
            App.SaveDraftHandler -= App_LoadDraft;

            App.CreateMessageHandler -= App_CreateMessageHandler;
            App.DeleteMessageHandler -= App_DeleteMessageHandler;

            // API events
            App.MessageCreatedHandler -= App_MessageCreatedHandler;
            App.MessageDeletedHandler -= App_MessageDeletedHandler;
            //UpdateUI-Messages
            App.MessageEditedHandler -= App_MessageEditedHandler;
            App.NavigateToGuildChannelHandler -= App_NavigateToGuildChannelHandler;
            App.NavigateToDMChannelHandler -= App_NavigateToDMChannelHandler;
            App.TypingHandler -= App_TypingHandler;
        }

        /// <summary>
        /// When the app is logging out
        /// </summary>
        private void App_LogOutHandler(object sender, EventArgs e)
        {
            ClearEvents();
        }

        /// <summary>
        /// Save a messagfe draft
        /// </summary>
        private void App_SaveDraft(object sender, EventArgs eventArgs)
        {
            // Save draft by channel Id
            if (ChannelId != null)
            {
                if (MessageBox1.Text == "")
                {
                    if (LocalState.Drafts.ContainsKey(ChannelId))
                        LocalState.Drafts.Remove(ChannelId);
                }
                else
                {
                    if (LocalState.Drafts.ContainsKey(ChannelId))
                        LocalState.Drafts[ChannelId] = MessageBox1.Text;
                    else
                        LocalState.Drafts.Add(ChannelId, MessageBox1.Text);
                }
            }
        }

        /// <summary>
        /// Load a draft (if it exists)
        /// </summary>
        private void App_LoadDraft(object sender, EventArgs eventArgs)
        {
            if (ChannelId != null && LocalState.Drafts.ContainsKey(ChannelId))
                MessageBox1.Text = LocalState.Drafts[ChannelId];
            else
                MessageBox1.Text = "";
        }

        /// <summary>
        /// When the user navigates to a different channel, from a guild
        /// </summary>
        private void App_NavigateToGuildChannelHandler(object sender, App.GuildChannelNavigationArgs e)
        {
            // Save draft 
            App_SaveDraft(null, null);

            // Set new Ids
            ChannelId = App.CurrentChannelId;
            CurrentGuildIsDM = App.CurrentGuildIsDM;
            CurrentGuildId = App.CurrentGuildId;

            // Render messages
            if (CurrentGuildId == e.GuildId)
            {
                RenderMessages();
                if (App.IsDesktop) MessageBox1.FocusTextBox();
            }

            // Update channel permissions for new channel
            bool? oldstate = MessageBox1?.IsEnabled;
            bool? newstate = LocalState.CurrentGuildChannel.permissions.SendMessages || App.CurrentGuildIsDM;
            MessageBox1.IsEnabled = newstate.Value;
            if(oldstate != newstate)
            {
                if (MessageBox1.IsEnabled)
                    messageShadow.Fade(Convert.ToSingle((Double)Application.Current.Resources["ShadowOpacity"]), 300).Start();
                else
                    messageShadow.Fade(Convert.ToSingle((Double)Application.Current.Resources["ShadowOpacity"])/2, 300).Start();
            }
            
            // Update typing indicator for new channel
            UpdateTyping();

            if (e.Message != null && !e.Send)
            {
                // Load request draft
                MessageBox1.Text = e.Message;
            } else
            {
                // Load old draft for new channel
                App_LoadDraft(null, null);
            }

            // Send requested draft message
            if (e.Message != null && e.Send)
            {
                App.CreateMessage(ChannelId, e.Message);
            }
        }

        /// <summary>
        /// When the user navigates to a different channel, from a DM
        /// </summary>
        private void App_NavigateToDMChannelHandler(object sender, App.DMChannelNavigationArgs e)
        {
            // Save draft
            App_SaveDraft(null, null);

            // Set new Ids
            ChannelId = App.CurrentChannelId;
            CurrentGuildIsDM = App.CurrentGuildIsDM;
            CurrentGuildId = App.CurrentGuildId;

            if (e.ChannelId != null)
            {
                if (e.Message != null && !e.Send)
                {
                    // Load requested draft
                    MessageBox1.Text = e.Message;
                }
                else
                {
                    // Load old draft
                    App_LoadDraft(null, null);
                }

                // Send request draft
                if (e.Send && e.Message != null)
                {
                    App.CreateMessage(ChannelId, e.Message);
                }

                // Render messages
                RenderMessages();
                if (App.IsDesktop) MessageBox1.FocusTextBox();

                // Update typing indicator for new channel
                UpdateTyping();
            }
        }

        /// <summary>
        /// Send message post request
        /// </summary>
        private async void App_CreateMessageHandler(object sender, App.CreateMessageArgs e)
        {
            // TODO: Show pending message item till sent
            
            if (e.Message.Content.Length > 10000) // Too long
            {
                
                MessageDialog md =
                    new MessageDialog("Sorry, but this message is way too long to be sent, even with Quarrel",
                        "Over 10 000 characters?!");
                await md.ShowAsync();
            }
            else if (e.Message.Content.Length > 2000) // Too long for one message
            {
                // Show loading indicator
                MessagesLoading.Visibility = Visibility.Visible;

                //Split the message into <2000 char ones and send them individually
                IEnumerable<string> split = SplitToLines(e.Message.Content, 2000);
                int splitcount = split.Count();
                if (splitcount < 10)
                {
                    for (int i = 0; i < splitcount; i++)
                    {
                        // Get split message
                        MessageUpsert splitmessage = new MessageUpsert {Content = split.ElementAt(i)};
                        if (i == splitcount) splitmessage.file = e.Message.file;

                        // Send message
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        await RESTCalls.CreateMessage(e.ChannelId, splitmessage);
                        sw.Stop();

                        //make sure to wait at least 500ms between each message
                        if (sw.ElapsedMilliseconds < 500) 
                            await Task.Delay(Convert.ToInt32(500 - sw.ElapsedMilliseconds));
                    }
                }
                else
                {
                    MessageDialog md =
                        new MessageDialog("Sorry, but this message is way too long to be sent, even with Quarrel",
                            "Wait, what?!");
                    await md.ShowAsync();
                    return;
                }

                // Hide loading indicator
                MessagesLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                //Just send the message
                await RESTCalls.CreateMessage(e.ChannelId, e.Message);
            }
        }

        /// <summary>
        /// Prompt message deletion
        /// </summary>
        private void App_DeleteMessageHandler(object sender, App.DeleteMessageArgs e)
        {
            // Navigate to dynamic subpage recommending message deletion
            SubFrameNavigator(typeof(DynamicSubPage), new SubPageData
            {
                Message = App.GetString("/Dialogs/DeleteMessageConfirm"),
                ConfirmMessage = App.GetString("/Dialogs/Delete"),
                SubMessage = "", //TODO: Make this the message
                StartText = "",
                PlaceHolderText = null,
                ConfirmRed = true,
                args = new Tuple<string, string>(e.ChannelId, e.MessageId),
                function = RESTCalls.DeleteMessage
            });
        }

        /// <summary>
        /// Splits string <paramref name="stringToSplit"/> into strings of length <paramref name="maxLineLength"/>
        /// </summary>
        /// <param name="stringToSplit">String to split</param>
        /// <param name="maxLineLength">Max character length of each line</param>
        /// <returns>A string array of orginal string split as neccesary</returns>
        private static IEnumerable<string> SplitToLines(string stringToSplit, int maxLineLength)
        {
            // Split into each word
            string[] words = stringToSplit.Split(' ');
            StringBuilder line = new StringBuilder();
            foreach (string word in words)
                if (word.Length + line.Length <= maxLineLength)
                {
                    // Add word to line
                    line.Append(word + " ");
                }
                else
                {
                    if (line.Length > 0)
                    {
                        // Add line to return value
                        yield return line.ToString().Trim();
                        line.Clear();
                    }

                    // Add overflow word(s) to new line and add current line to return value
                    string overflow = word;
                    while (overflow.Length > maxLineLength)
                    {
                        yield return overflow.Substring(0, maxLineLength);
                        overflow = overflow.Substring(maxLineLength);
                    }

                    line.Append(overflow + " ");
                }

            // return final line
            yield return line.ToString().Trim();
        }

        /// <summary>
        /// Handle a new message being created
        /// </summary>
        private async void App_MessageCreatedHandler(object sender, App.MessageCreatedArgs e)
        {
            if (ChannelId != e.Message.ChannelId) return;

            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (e.Message.Type == 3 && e.Message.User.Id != LocalState.CurrentUser.Id)
                {
                    // Show call notification
                    NotificationManager.CreateCallNotification(e.Message);
                }

                // True if new message should show header
                bool showheader = false;

                // True if the next message is unread
                bool nextIsUnread = false;

                // The id of the last message read by user
                string lastmessageid = LocalState.RPC[ChannelId].LastMessageId;

                // Add message list
                if (MessageList.Items.Count > 0)
                {
                    Message last = null;
                    for (int i = 0; i < MessageList.Items.Count; i++)
                    {
                        MessageContainer container = (MessageContainer) MessageList.Items[i];
                        if (!App.IsFocused)
                        {
                            if (nextIsUnread)
                            {
                                container.LastRead = true;
                                nextIsUnread = false;
                            }
                            else
                            {
                                container.LastRead = false;
                                if (container.Message.Id == lastmessageid)
                                {
                                    nextIsUnread = true;
                                    if (i == MessageList.Items.Count - 1) showheader = true;
                                }
                            }
                        }

                        if (i == MessageList.Items.Count - 1) last = container.Message;
                    }

                    MessageList.Items.Add(MessageManager.MakeMessage(e.Message,
                        MessageManager.ShouldContinuate(e.Message, last), showheader));
                }
                else
                {
                    MessageList.Items.Add(MessageManager.MakeMessage(e.Message));
                }

                // RPC data
                if (e.Message.User.Id == LocalState.CurrentUser.Id)
                {
                    //do something????
                }
                else
                {
                    if (App.IsFocused)
                        App.MarkMessageAsRead(e.Message.Id, ChannelId);
                    else
                        App.ReadWhenFocused(e.Message.Id, ChannelId, CurrentGuildId);
                }

                // Vibrate
                if (Storage.Settings.Vibrate && e.Message.User.Id != LocalState.CurrentUser.Id)
                {
                    TimeSpan vibrationDuration = TimeSpan.FromMilliseconds(200);
                    if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification"))
                    {
                        VibrationDevice phonevibrate = VibrationDevice.GetDefault();
                        phonevibrate.Vibrate(vibrationDuration);
                    }

                    //This will be for another time, it isn't working right now
                    /* var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault();
                     if(gamepad!=null)
                     {
                         GamepadVibration vibration = new GamepadVibration();
                         await Task.Run(async () =>
                         {
                             vibration.RightMotor = 0.5;
                             gamepad.Vibration = vibration;
                             await Task.Delay(vibrationDuration);
                             vibration.RightMotor = 0;
                         });
                     }*/
                }

                // Text to speech
                if (e.Message.TTS)
                {
                    MediaElement mediaplayer = new MediaElement();
                    using (SpeechSynthesizer speech = new SpeechSynthesizer())
                    {
                        speech.Voice =
                            SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Male);
                        string ssml = @"<speak version='1.0' " +
                                      "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" +
                                      e.Message.User.Username + "said" + e.Message.Content + "</speak>";
                        SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                        mediaplayer.SetSource(stream, stream.ContentType);
                        mediaplayer.Play();
                    }
                }
            });
        }

        /// <summary>
        /// Handle deleting a message
        /// </summary>
        private async void App_MessageDeletedHandler(object sender, App.MessageDeletedArgs e)
        {
            // If it's not from this channel, ignore it
            if (ChannelId != e.ChannelId) return;

            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < MessageList.Items.Count; i++)
                {
                    MessageContainer message = (MessageContainer) MessageList.Items[i];
                    if (message.Message != null && message.Message.Id == e.MessageId)
                    {
                        // Remove the item
                        MessageList.Items.Remove(message);

                        // The previous message was deleted, new item i is the old i++
                        if (i + 1 < MessageList.Items.Count)
                        {
                            (MessageList.Items[i] as MessageContainer).IsContinuation = false;
                        }

                        // Update latest unread message
                        if (LocalState.RPC[ChannelId].LastMessageId == e.MessageId)
                        {
                            // Get last message
                            MessageContainer last = (MessageContainer) MessageList.Items.LastOrDefault();
                            if (last != null)
                            {
                                // Update in cache
                                ReadState temp = LocalState.RPC[ChannelId];
                                temp.LastMessageId = ((MessageContainer) MessageList.Items.Last()).Message.Id;
                                LocalState.RPC[ChannelId] = temp;
                                if (!CurrentGuildIsDM)
                                {
                                    LocalState.Guilds[CurrentGuildId].channels[ChannelId].raw
                                        .LastMessageId = temp.LastMessageId;
                                }
                                else
                                {
                                    LocalState.DMs[ChannelId].LastMessageId = temp.LastMessageId;
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Handle a message being edited
        /// </summary>
        private async void App_MessageEditedHandler(object sender, App.MessageEditedArgs e)
        {
            // If it's not from this channel, ignore it
            if (ChannelId != e.Message.ChannelId) return;

            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageList.Items.Count > 0)
                    // Find message to update
                    foreach (MessageContainer message in MessageList.Items)
                        if (message.Message != null && message.Message.Id == e.Message.Id)
                        {
                            // TODO: Check if this is neccesary
                            message.Edit = true;

                            // Set new message content
                            message.Message = e.Message;

                            // TODO: Check if this is neccesary
                            message.Edit = false;
                        }
            });
        }

        /// <summary>
        /// Handle typing event
        /// </summary>
        private async void App_TypingHandler(object sender, App.TypingArgs e)
        {
            // Run on UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                UpdateTyping);
        }

        /// <summary>
        /// Update typing list
        /// </summary>
        public async void UpdateTyping()
        {
            string typingString = "";
            List<string> NamesTyping = new List<string>();

            // If the channel exists
            if (ChannelId != null)
            {
                // Check if typer list includes this channel
                if (LocalState.Typers.ContainsKey(ChannelId))
                {
                    // Get all typers in this channel
                    foreach (KeyValuePair<string, DispatcherTimer> typer in LocalState.Typers[ChannelId])
                    {
                        if (CurrentGuildIsDM)
                        {
                            // Add name to list
                            NamesTyping.Add(LocalState.DMs[ChannelId].Users
                                .FirstOrDefault(m => m.Id == typer.Key).Username);
                        }
                        else
                        {
                            // Get member
                            GuildMember member;
                            if (LocalState.Guilds[CurrentGuildId].members.ContainsKey(typer.Key))
                            {
                                member = LocalState.Guilds[CurrentGuildId].members[typer.Key];
                            }
                            else
                            {
                                member = new GuildMember() { User = await RESTCalls.GetUser(typer.Key) };
                            }

                            // Get name or nickname
                            string displayedName = member.User.Username;
                            if (member.Nick != null) displayedName = member.Nick;

                            // Add name/nickname to list
                            NamesTyping.Add(displayedName);
                        }
                    }
                }
            }

            int displayedTyperCounter = NamesTyping.Count();
            for (int i = 0; i < displayedTyperCounter; i++)
            {
                //TODO: Fix translate
                if (i == 0)
                {
                    typingString += NamesTyping.ElementAt(i); //first element, no prefix
                }
                else if (i == 2 && i == displayedTyperCounter)
                {
                    typingString +=
                        " " + App.GetString("/Main/TypingAnd") + " " + " " +
                        NamesTyping.ElementAt(i); //last element out of 2, prefix = "and"
                }
                else if (i == displayedTyperCounter)
                {
                    typingString +=
                        ", " + App.GetString("/Main/TypingAnd") +
                        NamesTyping.ElementAt(i); //last element out of 2, prefix = "and" WITH OXFORD COMMA
                }
                else
                {
                    typingString += ", " + NamesTyping.ElementAt(i); //intermediary element, prefix = comma
                }
            }

            // Add "is typing" or "are typing"
            if (displayedTyperCounter > 1)
            {
                typingString += " " + App.GetString("/Main/TypingPlural");
            }
            else
            {
                typingString += " " + App.GetString("/Main/TypingSingular");
            }

            // Update UI text
            TypingIndicator.Text = typingString;

            // Show or hide UI
            if (displayedTyperCounter == 0)
            {
                // Hide
                TypingStackPanel.Fade(0, 300).Start();
            }
            else
            {
                // Show
                TypingStackPanel.Fade(1, 300).Start();
            }
        }

        /// <summary>
        /// SubFrameNavigator for MessageBodyControl
        /// </summary>
        private void SubFrameNavigator(Type page, object args = null)
        {
            App.ShowSubFrame(page, args);
        }
    }
}