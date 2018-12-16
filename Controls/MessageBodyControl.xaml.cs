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
using Discord_UWP.API.Channel.Models;
using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;
using Discord_UWP.SimpleClasses;
using Discord_UWP.SubPages;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Debug = System.Diagnostics.Debug;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MessageBodyControl : UserControl
    {
        private ScrollViewer _messageScrollviewer;
        private ItemsStackPanel _messageStacker;
        private bool _outofboundsNewMessage;
        private bool DisableLoadingMessages;
        private readonly DispatcherTimer typingCooldown = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };

        private string ChannelId;
        private bool CurrentGuildIsDM;
        private string CurrentGuildId;
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


        public async void Setup(object o, EventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (App.CinematicMode)
                {
                    MessageList.Padding = new Thickness(0, 84, 0, 0);
                }

            });
        }

        private void GoToLastRead_Click(object sender, RoutedEventArgs e)
        {
            LoadMessagesAround(App.LastReadMsgId);
        }

        private void IgnoreNewMessages_Click(object sender, RoutedEventArgs e)
        {
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
        }

        private void ReturnToPresent_Click(object sender, RoutedEventArgs e)
        {
            RenderMessages();
        }

        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _messageStacker = sender as ItemsStackPanel;
        }

        private void ItemStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_messageScrollviewer != null && _messageStacker != null &&
                _messageStacker.ItemsUpdatingScrollMode == ItemsUpdatingScrollMode.KeepLastItemInView)
                if (MessageList.Items.Count > 0 &&
                    _messageScrollviewer.VerticalOffset + 24 > _messageScrollviewer.ExtentHeight)
                    _messageScrollviewer.ChangeView(null, _messageScrollviewer.ExtentHeight, null, true);
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            string text = MessageBox1.Text;
            App.CreateMessage(ChannelId, text);

            MessageBox1.Text = "";
            MessageBox1.FocusTextBox();

            //Add a user activity for this channel:

            Task.Run(async () =>
            {
                if (CurrentGuildIsDM)
                    await UserActivityManager.GenerateActivityAsync("@me", LocalState.CurrentChannel.raw.Name,
                        LocalState.CurrentChannel.raw.Icon, LocalState.CurrentChannel.raw.Id, "");
                else
                    await UserActivityManager.GenerateActivityAsync(LocalState.CurrentGuild.Raw.Id,
                        LocalState.CurrentGuild.Raw.Name, LocalState.CurrentGuild.Raw.Icon,
                        LocalState.CurrentChannel.raw.Id, "#" + LocalState.CurrentChannel.raw.Name);
            });
        }


        private void TypingCooldown_Tick(object sender, object e)
        {
            typingCooldown.Stop();
        }

        private async void TypingStarted(object sender, TextChangedEventArgs e)
        {
            if (!typingCooldown.IsEnabled)
            {
                await RESTCalls.TriggerTypingIndicator(ChannelId);
                typingCooldown.Start();
            }
        }


        private void MessageBox1_OpenAdvanced(object sender, MessageBox.OpenAdvancedArgs e)
        {
            if (e == null)
                App.NavigateToMessageEditor(MessageBox1.Text, false);
            else
                App.NavigateToMessageEditor(e.Content, e.Paste);
            MessageBox1.Text = "";
        }

        private void MessageBox1_OpenSpotify(object sender, RoutedEventArgs e)
        {
            SubFrameNavigator(typeof(SpotifyShare));
        }

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


        public enum Position
        {
            Before,
            After
        }

        public void AddMessages(Position position, bool scroll, List<MessageContainer> messages,
            bool showNewMessageIndicator)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ReturnToPresentIndicator.Visibility = Visibility.Collapsed;
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            if (messages != null && messages.Count > 0 && messages[0].Message.ChannelId == ChannelId)
            {
                MessageContainer scrollTo = null;
                if (showNewMessageIndicator)
                {
                    //(MAYBE) SHOW NEW MESSAGE INDICATOR
                    DateTimeOffset firstMessageTime = Common.SnowflakeToTime(messages.First().Message.Id);
                    DateTimeOffset lastMessageTime = Common.SnowflakeToTime(messages.Last().Message.Id);
                    DateTimeOffset lastReadTime = Common.SnowflakeToTime(App.LastReadMsgId);

                    if (firstMessageTime < lastReadTime)
                    {
                        //the last read message is after the first one in the list
                        if (lastMessageTime > lastReadTime)
                        {
                            _outofboundsNewMessage = false;
                            //the last read message is before the last one in the list
                            bool canBeLastRead = true;
                            for (int i = 0; i < messages.Count(); i++)
                            {
                                if (canBeLastRead)
                                {
                                    //the first one with a larger timestamp gets the "NEW MESSAGES" header
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
                            //The last read message is after the span of currently displayed messages
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
                        //the last read message is before the first one in the list
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
                    //DO NOT SHOW NEW MESSAGE INDICATOR. Just add everything before or after
                    for (int i = 0; i < messages.Count(); i++)
                        if (position == Position.After)
                            MessageList.Items.Add(messages[i]);
                        else if (position == Position.Before)
                            MessageList.Items.Insert(i, messages[i]);
                }

                if (scroll && scrollTo != null) MessageList.ScrollIntoView(scrollTo, ScrollIntoViewAlignment.Leading);
            }

            Message last = MessageList.Items.Count > 0 ? (MessageList.Items.Last() as MessageContainer).Message : null;
            if (last != null && CurrentGuildId != null && ChannelId != null && last.Id !=
                LocalState.CurrentGuild.channels[ChannelId].raw.LastMessageId)
            {
                ReturnToPresentIndicator.Opacity = 1;
                ReturnToPresentIndicator.Visibility = Visibility.Visible;
                ReturnToPresentIndicator.Fade(1, 300).Start();
            }
            else
            {
                ReturnToPresentIndicator.Visibility = Visibility.Collapsed;
            }

            sw.Stop();
            Debug.WriteLine("Messages took " + sw.ElapsedMilliseconds + "ms to load");
        }


        public async void RenderMessages()
        {
            _outofboundsNewMessage = false;
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            MessagesLoading.Visibility = Visibility.Visible;
            MessageList.Items.Clear();

            MessageList.Items.Clear();
            IEnumerable<Message> emessages = null;
            await Task.Run(async () => { emessages = await RESTCalls.GetChannelMessages(ChannelId); });
            if (emessages != null)
            {
                List<MessageContainer> messages = await MessageManager.ConvertMessage(emessages.ToList());
                AddMessages(Position.After, true, messages, true);
            }

            MessagesLoading.Visibility = Visibility.Collapsed;
        }

        private bool LastMessageIsLoaded()
        {
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


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _messageScrollviewer = Common.GetScrollViewer(MessageList);
            if (_messageScrollviewer != null) _messageScrollviewer.ViewChanged += MessageScrollviewer_ViewChanged;
        }

        private void MessageScrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (MessageList.Items.Count > 0)
            {
                double fromTop = _messageScrollviewer.VerticalOffset;
                double fromBottom = _messageScrollviewer.ScrollableHeight - fromTop;
                if (fromTop < 100 && !DisableLoadingMessages)
                    LoadOlderMessages();
                if (fromBottom < 200 && !DisableLoadingMessages)
                    LoadNewerMessages();
            }
        }

        private async void LoadOlderMessages()
        {
            DisableLoadingMessages = true;
            MessagesLoadingTop.Visibility = Visibility.Visible;
            List<MessageContainer> messages = await MessageManager.ConvertMessage(
                (await RESTCalls.GetChannelMessagesBefore(ChannelId,
                    (MessageList.Items.FirstOrDefault(x => (x as MessageContainer).Message != null) as MessageContainer)
                    .Message.Id)).ToList());
            AddMessages(Position.Before, false, messages,
                _outofboundsNewMessage); //if there is an out of bounds new message, show the indicator. Otherwise, don't.
            MessagesLoadingTop.Visibility = Visibility.Collapsed;
            await Task.Delay(1000);
            DisableLoadingMessages = false;
        }


        private async void LoadNewerMessages()
        {
            try
            {
                Message last = (MessageList.Items.Last() as MessageContainer)?.Message;
                if (last != null && last.Id != LocalState.RPC[ChannelId].LastMessageId)
                {
                    // var offset = MessageScrollviewer.VerticalOffset;
                    MessagesLoading.Visibility = Visibility.Visible;
                    DisableLoadingMessages = true;
                    List<MessageContainer> messages = await MessageManager.ConvertMessage(
                        (await RESTCalls.GetChannelMessagesAfter(ChannelId,
                            (MessageList.Items.LastOrDefault(x => (x as MessageContainer).Message != null) as
                                MessageContainer).Message.Id)).ToList());
                    _messageStacker.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
                    AddMessages(Position.After, false, messages,
                        _outofboundsNewMessage); //if there is an out of bounds new message, show the indicator. Otherwise, don't.
                    MessagesLoading.Visibility = Visibility.Collapsed;
                    await Task.Delay(1000);
                    _messageStacker.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                    DisableLoadingMessages = false;
                }
            }
            catch
            {
                // ignored
            }
        }


        private void App_MentionHandler(object sender, App.MentionArgs e)
        {
            if (MessageBox1.Text.Trim() == "")
                MessageBox1.Text = "@" + e.Username + "#" + e.Discriminator;
            else
                MessageBox1.Text = MessageBox1.Text + " @" + e.Username + "#" + e.Discriminator;
        }

        private void App_LoggingInHandlerAsync(object sender, EventArgs e)
        {
            if (App.LoggedIn())
            {
                App.LogOutHandler += App_LogOutHandler;
                App.MentionHandler += App_MentionHandler;
                App.SaveDraftHandler += App_SaveDraft;
                App.SaveDraftHandler += App_LoadDraft;
                if (!isMypeople)
                {
                    App.CreateMessageHandler += App_CreateMessageHandler;
                }
                App.DeleteMessageHandler += App_DeleteMessageHandler;
                //UpdateUI-Messages
                App.MessageCreatedHandler += App_MessageCreatedHandler;
                App.MessageDeletedHandler += App_MessageDeletedHandler;
                App.MessageEditedHandler += App_MessageEditedHandler;
                App.NavigateToGuildChannelHandler += App_NavigateToGuildChannelHandler;
                App.NavigateToDMChannelHandler += App_NavigateToDMChannelHandler;
                App.TypingHandler += App_TypingHandler;
            }
        }

        private void ClearEvents()
        {
            App.MentionHandler -= App_MentionHandler;
            App.LogOutHandler -= App_LogOutHandler;
            //API
            App.CreateMessageHandler -= App_CreateMessageHandler;
            App.DeleteMessageHandler -= App_DeleteMessageHandler;
            //UpdateUI-Messages
            App.MessageCreatedHandler -= App_MessageCreatedHandler;
            App.MessageDeletedHandler -= App_MessageDeletedHandler;
            App.MessageEditedHandler -= App_MessageEditedHandler;
        }

        private void App_LogOutHandler(object sender, EventArgs e)
        {
            ClearEvents();
        }

        private void App_SaveDraft(object sender, EventArgs eventArgs)
        {
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

        private void App_LoadDraft(object sender, EventArgs eventArgs)
        {
            if (ChannelId != null && LocalState.Drafts.ContainsKey(ChannelId))
                MessageBox1.Text = LocalState.Drafts[ChannelId];
            else
                MessageBox1.Text = "";
        }

        private void App_NavigateToGuildChannelHandler(object sender, App.GuildChannelNavigationArgs e)
        {
            ChannelId = App.CurrentChannelId;
            CurrentGuildIsDM = App.CurrentGuildIsDM;
            CurrentGuildId = App.CurrentGuildId;
            App_SaveDraft(null, null);
            if (CurrentGuildId == e.GuildId)
            {
                RenderMessages();
                if (App.IsDesktop) MessageBox1.FocusTextBox();
            }

            MessageBox1.IsEnabled = LocalState.CurrentChannel.permissions.SendMessages || App.CurrentGuildIsDM;

            UpdateTyping();
            App_SaveDraft(null, null);
        }

        private void App_NavigateToDMChannelHandler(object sender, App.DMChannelNavigationArgs e)
        {
            ChannelId = App.CurrentChannelId;
            CurrentGuildIsDM = App.CurrentGuildIsDM;
            CurrentGuildId = App.CurrentGuildId;
            App_SaveDraft(null, null);
            if (e.ChannelId != null)
            {
                if (e.Message != null && !e.Send)
                    MessageBox1.Text = e.Message;
                else if (e.Send && e.Message != null)
                    App_CreateMessageHandler(null,
                        new App.CreateMessageArgs { ChannelId = ChannelId, Message = new MessageUpsert { Content = e.Message } });

                RenderMessages();

                App_LoadDraft(null, null);
                MessageBox1.FocusTextBox();
                UpdateTyping();
            }
        }

        private async void App_CreateMessageHandler(object sender, App.CreateMessageArgs e)
        {
            //MessageList.Items.Add(MessageManager.MakeMessage(e.ChannelId, e.Message));
            if (e.Message.Content.Length > 10000)
            {
                //To avoid spam
                MessageDialog md =
                    new MessageDialog("Sorry, but this message is way too long to be sent, even with Quarrel",
                        "Over 10 000 characters?!");
                await md.ShowAsync();
            }
            else if (e.Message.Content.Length > 2000)
            {
                MessagesLoading.Visibility = Visibility.Visible;
                //Split the message into <2000 char ones and send them individually
                IEnumerable<string> split = SplitToLines(e.Message.Content, 2000);
                int splitcount = split.Count();
                if (splitcount < 10)
                {
                    for (int i = 0; i < splitcount; i++)
                    {
                        MessageUpsert splitmessage = new MessageUpsert {Content = split.ElementAt(i)};
                        if (i == splitcount) splitmessage.file = e.Message.file;
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        await RESTCalls.CreateMessage(e.ChannelId, splitmessage);
                        sw.Stop();
                        if (sw.ElapsedMilliseconds < 500) //make sure to wait at least 500ms between each message
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

                MessagesLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                //Just send the message
                await RESTCalls.CreateMessage(e.ChannelId, e.Message);
            }
        }

        private void App_DeleteMessageHandler(object sender, App.DeleteMessageArgs e)
        {
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

        private static IEnumerable<string> SplitToLines(string stringToSplit, int maxLineLength)
        {
            string[] words = stringToSplit.Split(' ');
            StringBuilder line = new StringBuilder();
            foreach (string word in words)
                if (word.Length + line.Length <= maxLineLength)
                {
                    line.Append(word + " ");
                }
                else
                {
                    if (line.Length > 0)
                    {
                        yield return line.ToString().Trim();
                        line.Clear();
                    }

                    string overflow = word;
                    while (overflow.Length > maxLineLength)
                    {
                        yield return overflow.Substring(0, maxLineLength);
                        overflow = overflow.Substring(maxLineLength);
                    }

                    line.Append(overflow + " ");
                }

            yield return line.ToString().Trim();
        }

        private async void App_MessageCreatedHandler(object sender, App.MessageCreatedArgs e)
        {
            if (ChannelId != e.Message.ChannelId) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (e.Message.Type == 3 && e.Message.User.Id != LocalState.CurrentUser.Id)
                {
                    //TODO: Pretty up this shit (animations)
                    // AcceptCallUI.Tag = e.Message.ChannelId;
                    // AcceptCallUI.Visibility = Visibility.Visible;
                    NotificationManager.CreateCallNotification(e.Message);
                }

                //var lastMsg = MessageList.Items.LastOrDefault() as MessageContainer;
                //if (e.Message.User.Id == LocalState.CurrentUser.Id)
                //{
                //    if (lastMsg.Pending)
                //    {
                //        lastMsg.Message = lastMsg.Message.Value.MergePending(e.Message);
                //        if (lastMsg.Message.Value.User.Id == null)
                //        {
                //            lastMsg.Message.Value.SetUser(LocalModels.LocalState.CurrentUser);
                //        }
                //        lastMsg.Pending = false;
                //    }
                //} else
                //{

                bool showheader = false;
                bool nextIsUnread = false;
                string lastmessageid = LocalState.RPC[ChannelId].LastMessageId;
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

                //set the last message id

                //}
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


                if (Storage.Settings.Vibrate && e.Message.User.Id != LocalState.CurrentUser.Id)
                {
                    TimeSpan vibrationDuration = TimeSpan.FromMilliseconds(200);
                    if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification"))
                    {
                        VibrationDevice phonevibrate = VibrationDevice.GetDefault();
                        phonevibrate.Vibrate(vibrationDuration);
                    }

                    //This will be for another time, it clearly isn't working right now
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


        private async void App_MessageDeletedHandler(object sender, App.MessageDeletedArgs e)
        {
            if (ChannelId != e.ChannelId) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < MessageList.Items.Count; i++)
                {
                    MessageContainer message = (MessageContainer) MessageList.Items[i];
                    if (message.Message != null && message.Message.Id == e.MessageId)
                    {
                        MessageList.Items.Remove(message);

                        //the previous message was deleted, new item i is the old i++
                        if (i + 1 < MessageList.Items.Count)
                        {
                            (MessageList.Items[i] as MessageContainer).IsContinuation = false;
                        }

                        if (LocalState.RPC[ChannelId].LastMessageId == e.MessageId)
                        {
                            MessageContainer last = (MessageContainer) MessageList.Items.LastOrDefault();
                            if (last != null)
                            {
                                ReadState temp = LocalState.RPC[ChannelId];
                                temp.LastMessageId = ((MessageContainer) MessageList.Items.Last()).Message.Id;
                                LocalState.RPC[ChannelId] = temp;
                                if(!CurrentGuildIsDM)
                                    LocalState.Guilds[CurrentGuildId].channels[ChannelId].raw
                                        .LastMessageId = temp.LastMessageId;
                                else
                                    LocalState.DMs[ChannelId].LastMessageId = temp.LastMessageId;
                            }
                        }
                    }
                }
            });
        }

        private async void App_MessageEditedHandler(object sender, App.MessageEditedArgs e)
        {
            if (ChannelId != e.Message.ChannelId) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (MessageList.Items.Count > 0)
                    foreach (MessageContainer message in MessageList.Items)
                        if (message.Message != null && message.Message.Id == e.Message.Id)
                        {
                            message.Edit = true;
                            message.Message = e.Message;
                            message.Edit = false;
                        }
            });
        }

        private async void App_TypingHandler(object sender, App.TypingArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                UpdateTyping);
        }

        public void UpdateTyping()
        {
            string typingString = "";
            List<string> NamesTyping = new List<string>();

            if (ChannelId != null)
                if (LocalState.Typers.ContainsKey(ChannelId))
                    foreach (KeyValuePair<string, DispatcherTimer> typer in LocalState.Typers[ChannelId])
                        if (CurrentGuildIsDM)
                        {
                            NamesTyping.Add(LocalState.DMs[ChannelId].Users
                                .FirstOrDefault(m => m.Id == typer.Key).Username);
                        }
                        else
                        {
                            GuildMember member = LocalState.Guilds[CurrentGuildId].members[typer.Key];
                            string displayedName = member.User.Username;
                            if (member.Nick != null) displayedName = member.Nick;
                            NamesTyping.Add(displayedName);
                        }

            int displayedTyperCounter = NamesTyping.Count();
            for (int i = 0; i < displayedTyperCounter; i++)
                //TODO: Fix translate
                if (i == 0)
                    typingString += NamesTyping.ElementAt(i); //first element, no prefix
                else if (i == 2 && i == displayedTyperCounter)
                    typingString +=
                        " " + App.GetString("/Main/TypingAnd") + " " + " " +
                        NamesTyping.ElementAt(i); //last element out of 2, prefix = "and"
                else if (i == displayedTyperCounter)
                    typingString +=
                        ", " + App.GetString("/Main/TypingAnd") +
                        NamesTyping.ElementAt(i); //last element out of 2, prefix = "and" WITH OXFORD COMMA
                else
                    typingString += ", " + NamesTyping.ElementAt(i); //intermediary element, prefix = comma
            if (displayedTyperCounter > 1)
                typingString += " " + App.GetString("/Main/TypingPlural");
            else
                typingString += " " + App.GetString("/Main/TypingSingular");

            TypingIndicator.Text = typingString;

            if (displayedTyperCounter == 0)
            {
                TypingStackPanel.Fade(0, 300).Start();
            }
            else
            {
                TypingIndicator.Text = typingString;
                TypingStackPanel.Fade(1, 300).Start();
            }
        }

        private void SubFrameNavigator(Type page, object args = null)
        {
            App.ShowSubFrame(page, args);
        }
    }
}