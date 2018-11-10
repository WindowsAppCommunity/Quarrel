using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation.Metadata;
using Windows.Media.SpeechSynthesis;
using Windows.Phone.Devices.Notification;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.API.User.Models;
using Discord_UWP.Controls;
using Discord_UWP.LocalModels;
using Discord_UWP.Managers;
using Discord_UWP.SharedModels;
using Discord_UWP.SimpleClasses;
using Microsoft.Toolkit.Uwp.UI.Animations;
using ContactManager = Discord_UWP.Managers.ContactManager;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPanePage : Page
    {
        private ScrollViewer _messageScrollviewer;
        private ItemsStackPanel _messageStacker;
        private bool _outofboundsNewMessage;
        private bool DisableLoadingMessages;
        private string DMChannelID;
        private string userID;
        private readonly DispatcherTimer typingCooldown = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
        private bool render;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            MessagesLoading.Visibility = Visibility.Visible;
            var contactManager = new ContactManager();
            ContactPanelActivatedEventArgs panelArgs = (ContactPanelActivatedEventArgs) e.Parameter;
            Contact contact = await contactManager.GetContactlocal(panelArgs.Contact.Id);
            userID = contact.RemoteId;
            DMChannelID = LocalState.DMs
                              ?.FirstOrDefault(dm =>
                                  dm.Value?.Type == 1 && dm.Value.Users.FirstOrDefault()?.Id == userID).Value?.Id ??
                          (await RESTCalls.CreateDM(new CreateDM
                              {Recipients = new List<string> {userID}.AsEnumerable()})).Id;
            if(render) RenderMessages();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            _messageScrollviewer = Common.GetScrollViewer(MessageList);
            if (_messageScrollviewer != null) _messageScrollviewer.ViewChanged += MessageScrollviewer_ViewChanged;
            MessageBox1.FocusTextBox();
        }

        public ContactPanePage()
        {
            InitializeComponent();
            typingCooldown.Tick += TypingCooldown_Tick;

            GatewayManager.Gateway.MessageCreated += MessageCreatedHandler;
            if (App.Ready)
                render = true;
            else
                App.ReadyRecievedHandler += ReadyRecievedHandler;
        }

        private void ItemStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_messageStacker == null ||
                _messageStacker.ItemsUpdatingScrollMode != ItemsUpdatingScrollMode.KeepLastItemInView) return;
            if (MessageList.Items.Count > 0 &&
                _messageScrollviewer.VerticalOffset + 24 > _messageScrollviewer.ExtentHeight)
                _messageScrollviewer.ChangeView(null, _messageScrollviewer.ExtentHeight, null, true);
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
                (await RESTCalls.GetChannelMessagesBefore(DMChannelID,
                    (MessageList.Items.FirstOrDefault(x => (x as MessageContainer).Message != null) as MessageContainer)
                    .Message.Id)).ToList());
            AddMessages(Position.Before, false, messages, _outofboundsNewMessage);
            MessagesLoadingTop.Visibility = Visibility.Collapsed;
            await Task.Delay(1000);
            DisableLoadingMessages = false;
        }

        private async void LoadNewerMessages()
        {
            try
            {
                Message last = (MessageList.Items.Last() as MessageContainer)?.Message;
                if (last != null && last.Id != LocalState.RPC[DMChannelID].LastMessageId)
                {
                    // var offset = MessageScrollviewer.VerticalOffset;
                    MessagesLoading.Visibility = Visibility.Visible;
                    DisableLoadingMessages = true;
                    List<MessageContainer> messages = await MessageManager.ConvertMessage(
                        (await RESTCalls.GetChannelMessagesAfter(DMChannelID,
                            (MessageList.Items.LastOrDefault(x => (x as MessageContainer).Message != null) as
                                MessageContainer).Message.Id)).ToList());
                    _messageStacker.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;
                    AddMessages(Position.After, false, messages, _outofboundsNewMessage);
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

        public void AddMessages(Position position, bool scroll, List<MessageContainer> messages,
            bool showNewMessageIndicator)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ReturnToPresentIndicator.Visibility = Visibility.Collapsed;
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            if (messages != null && messages.Count > 0)
            {
                MessageContainer scrollTo = null;
                if (showNewMessageIndicator)
                {
                    //(MAYBE) SHOW NEW MESSAGE INDICATOR
                    DateTimeOffset firstMessageTime = Common.SnowflakeToTime(messages.First().Message.Id);
                    DateTimeOffset lastMessageTime = Common.SnowflakeToTime(messages.Last().Message.Id);
                    DateTimeOffset lastReadTime = Common.SnowflakeToTime(LocalState.RPC[DMChannelID].LastMessageId);

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
            if (last != null && last.Id != LocalState.DMs[DMChannelID].LastMessageId)
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
            System.Diagnostics.Debug.WriteLine("Messages took " + sw.ElapsedMilliseconds + "ms to load");
        }

        public enum Position
        {
            Before,
            After
        }

        private void ItemsStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _messageStacker = sender as ItemsStackPanel;
        }

        public async void RenderMessages()
        {
            _outofboundsNewMessage = false; //assume this for the moment
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
            MessagesLoading.Visibility = Visibility.Visible;
            MessageList.Items.Clear();
            IEnumerable<Message> emessages = null;
            await Task.Run(async () => { emessages = await RESTCalls.GetChannelMessages(DMChannelID); });
            if (emessages != null)
            {
                List<MessageContainer> messages = await MessageManager.ConvertMessage(emessages.ToList());
                AddMessages(Position.After, true, messages, true);
            }

            MessagesLoading.Visibility = Visibility.Collapsed;

        }

        private void TypingCooldown_Tick(object sender, object e)
        {
            typingCooldown.Stop();
        }

        private async void TypingStarted(object sender, TextChangedEventArgs e)
        {
            if (!typingCooldown.IsEnabled)
            {
                await RESTCalls.TriggerTypingIndicator(DMChannelID);
                typingCooldown.Start();
            }
        }

        private void CreateMessage(object sender, RoutedEventArgs e)
        {
            string text = MessageBox1.Text;
            App.CreateMessage(DMChannelID, text);

            MessageBox1.Text = "";
            MessageBox1.FocusTextBox();
        }

        private void MessageBox1_OpenAdvanced(object sender, MessageBox.OpenAdvancedArgs e)
        {
            App.NavigateToMessageEditor(MessageBox1.Text, e != null && e.Paste);
            MessageBox1.Text = "";
        }

        private void MessageBox1_OpenSpotify(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private async void MessageCreatedHandler(object sender, Gateway.GatewayEventArgs<Message> e)
        {
            Message message = e.EventData;
            if (message.ChannelId != DMChannelID) return;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                bool showheader = false;
                bool nextIsUnread = false;
                string lastmessageid = LocalState.RPC[DMChannelID].LastMessageId;
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

                    MessageList.Items.Add(MessageManager.MakeMessage(message,
                        MessageManager.ShouldContinuate(message, last), showheader));
                }
                else
                {
                    MessageList.Items.Add(MessageManager.MakeMessage(message, false));
                }

                if (message.User.Id == LocalState.CurrentUser.Id)
                {
                    //do something????
                }
                else
                {
                    App.MarkMessageAsRead(message.Id, DMChannelID);
                }


                if (Storage.Settings.Vibrate && message.User.Id != LocalState.CurrentUser.Id)
                {
                    TimeSpan vibrationDuration = TimeSpan.FromMilliseconds(200);
                    if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification"))
                    {
                        VibrationDevice phonevibrate = VibrationDevice.GetDefault();
                        phonevibrate.Vibrate(vibrationDuration);
                    }
                }

                if (message.TTS)
                {
                    MediaElement mediaplayer = new MediaElement();
                    using (SpeechSynthesizer speech = new SpeechSynthesizer())
                    {
                        speech.Voice =
                            SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Male);
                        string ssml = @"<speak version='1.0' " +
                                      "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" +
                                      message.User.Username + "said" + message.Content + "</speak>";
                        SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                        mediaplayer.SetSource(stream, stream.ContentType);
                        mediaplayer.Play();
                    }
                }
            });
        }

        private void GoToLastRead_Click(object sender, RoutedEventArgs e)
        {
            LoadMessagesAround(LocalState.RPC[DMChannelID].LastMessageId);
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
                            (await RESTCalls.GetChannelMessagesAround(DMChannelID, id)).ToList());
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

        private bool LastMessageIsLoaded()
        {
            for (int i = MessageList.Items.Count; i < 0; i--)
                if (((MessageContainer) MessageList.Items[i]).Message != null)
                {
                    if (((MessageContainer) MessageList.Items[i]).Message.Id ==
                        LocalState.DMs[DMChannelID].LastMessageId)
                        return true;
                    return false;
                }

            return false;
        }

        private void IgnoreNewMessages_Click(object sender, RoutedEventArgs e)
        {
            MoreNewMessageIndicator.Visibility = Visibility.Collapsed;
        }

        private void ReturnToPresent_Click(object sender, RoutedEventArgs e)
        {
            RenderMessages();
        }

        private void ReadyRecievedHandler(object sender, EventArgs e)
        {

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RenderMessages);
        }
    }
}
