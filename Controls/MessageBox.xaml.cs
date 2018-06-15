using Discord_UWP.SharedModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Discord_UWP.LocalModels;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using Discord_UWP.Managers;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Core;
using Microsoft.Toolkit.Uwp.UI.Animations;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MessageBox : UserControl
    {
        public class OpenAdvancedArgs : EventArgs
        {
            public bool Paste { get; set; }
        }
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler<RoutedEventArgs> Send;
        public event EventHandler<RoutedEventArgs> Cancel;
        public event EventHandler<OpenAdvancedArgs> OpenAdvanced;
        public event EventHandler<RoutedEventArgs> OpenSpotify;

        public string Text
        {
            get { return MessageEditor.Text; }
            set { MessageEditor.Text = value; }
        }
        public new double FontSize
        {
            get { return MessageEditor.FontSize; }
            set { MessageEditor.FontSize = value; }
        }
        public readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(MessageBox),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public bool IsCompact
        {
            set
            {
                if (value == true)
                {
                    EmojiButton.Visibility = Visibility.Collapsed;
                    attachButton.Visibility = Visibility.Collapsed;
                    spotifyActive.Visibility = Visibility.Collapsed;
                }
            }
        }

        public new readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            nameof(IsEnabled),
            typeof(bool),
            typeof(MessageBox),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public bool IsEdit
        {
            get { return (bool)GetValue(IsEditProperty); }
            set { SetValue(IsEditProperty, value); }
        }

        public readonly DependencyProperty IsEditProperty = DependencyProperty.Register(
            nameof(IsEdit),
            typeof(bool),
            typeof(MessageBox),
            new PropertyMetadata("", OnPropertyChangedStatic));

        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as MessageBox;
            instance?.OnPropertyChanged(d, e.Property);
        }

        public MessageBox()
        {
            this.InitializeComponent();
            MessageEditor.PlaceholderText = App.GetString("/Controls/SendMessagePlaceholderText"); //TODO: Check if can be done with x:Uid
            SpotifyManager.SpotifyStateUpdated += SpotifyManager_SpotifyStateUpdated;
            Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (MessageEditor.FocusState == FocusState.Unfocused)
            {
                args.Handled = false;
                return;
            }
            else
            {
                bool HandleSuggestions = (SuggestionBlock.Items.Count != 0);
                if (args.VirtualKey == VirtualKey.Enter)
                {
                    args.Handled = true;
                    Windows.Devices.Input.KeyboardCapabilities keyboardCapabilities = new Windows.Devices.Input.KeyboardCapabilities();
                    if (keyboardCapabilities.KeyboardPresent > 0)
                    {
                        if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                        {
                            InsertNewLine();
                        }
                        else if (HandleSuggestions && SuggestionBlock.SelectedItem != null)
                            SelectSuggestion(SuggestionBlock.SelectedItem);
                        else
                            SendBox_OnClick(null, null);
                    }
                    else if (HandleSuggestions)
                        SelectSuggestion(SuggestionBlock.SelectedItem);
                    else
                        InsertNewLine();
                    return;
                }
                if (SuggestionPopup.IsOpen)
                {
                    if (args.VirtualKey == VirtualKey.Up)
                    {
                        args.Handled = true;
                        if (SuggestionBlock.SelectedIndex == -1 || SuggestionBlock.SelectedIndex == 0)
                            SuggestionBlock.SelectedIndex = SuggestionBlock.Items.Count - 1;
                        else
                            SuggestionBlock.SelectedIndex = SuggestionBlock.SelectedIndex - 1;
                        return;
                    }
                    else if (args.VirtualKey == VirtualKey.Down)
                    {
                        args.Handled = true;
                        if (SuggestionBlock.SelectedIndex == -1 || SuggestionBlock.SelectedIndex == SuggestionBlock.Items.Count - 1)
                            SuggestionBlock.SelectedIndex = 0;
                        else
                            SuggestionBlock.SelectedIndex = SuggestionBlock.SelectedIndex + 1;
                        return;
                    }
                }
            }
            args.Handled = false;
        }

        private async void SpotifyManager_SpotifyStateUpdated(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (SpotifyManager.SpotifyState != null)
                    {
                        if (SpotifyManager.SpotifyState.IsPlaying)
                        {
                            spotifyInvite.Visibility = Visibility.Visible;
                            spotifyActive.Fade(1, 200).Start();
                        }

                        else
                        {
                            spotifyActive.Fade(0, 200).Start();
                            spotifyInvite.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        spotifyActive.Opacity = 0;
                        spotifyInvite.Visibility = Visibility.Collapsed;
                    }

                });

        }

        public void Clear()
        {
            MessageEditor.Text = "";
            SendBox.IsEnabled = false;
        }

        private void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == IsEnabledProperty)
            {
                MessageEditor.IsEnabled = IsEnabled;
                SendBox.IsEnabled = IsEnabled;
                EmojiButton.IsEnabled = IsEnabled;
            }
            if(prop == TextProperty)
            {
                MessageEditor.Text = Text;
            }
            if(prop == IsEditProperty)
            {
                if(IsEdit == true)
                {
                    CancelButton.Visibility = Visibility.Visible;
                    attachButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    CancelButton.Visibility = Visibility.Collapsed;
                    attachButton.Visibility = Visibility.Visible;
                }
                
            }
        }
        public void FocusTextBox()
        {
            MessageEditor.Focus(FocusState.Keyboard);
        }
        private void SendBox_OnClick(object sender, RoutedEventArgs e)
        {
            //Text = ProcessString(Text);
            var mentions = Common.FindMentions(Text);
            foreach (var mention in mentions)
            {
                if (mention[0] == '@')
                {
                    int discIndex = mention.IndexOf('#');
                    string username = mention.Substring(1, discIndex-1);
                    string disc = mention.Substring(1 + discIndex);
                    SharedModels.User user;
                    if (App.CurrentGuildIsDM)
                    {
                        user = LocalState.DMs[App.CurrentChannelId].Users.FirstOrDefault(x => x.Username == username && x.Discriminator == disc);
                    } else
                    {
                        user = LocalState.Guilds[App.CurrentGuildId].members.FirstOrDefault(x => x.Value.User.Username == username && x.Value.User.Discriminator == disc).Value.User;
                    }
                    if (user != null)
                    {
                        Text = Text.Replace("@" + user.Username + "#" + user.Discriminator, "<@!" + user.Id + ">");
                    }
                } else if (mention[0] == '#')
                {
                    if (!App.CurrentGuildIsDM)
                    {
                        var channel = LocalState.Guilds[App.CurrentGuildId].channels.FirstOrDefault(x => x.Value.raw.Type != 4 && x.Value.raw.Name == mention.Substring(1)).Value;
                        if (channel != null)
                        {
                            Text = Text.Replace("#" + channel.raw.Name, "<#" + channel.raw.Id + ">");
                        }
                    }
                }
            }
            //if (EnableEncryption.IsChecked == true)
            //    Text = EncryptionManager.EncryptMessage(Text);
            Send?.Invoke(sender, e);
        }

        string PureText = "";
        int caretposition = 0;
        //bool EnableChanges = true;
        string mentionPrefix = "";

        private void MessageEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageEditor.Text))
                SendBox.IsEnabled = false;
            else
                SendBox.IsEnabled = true;
            TextChanged?.Invoke(sender, e);
        }
        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PopupTransform.Y = -e.NewSize.Height;
        }

        private void SelectSuggestion(object unknownitem)
        {
            string suggestion = "";

            var item = (Common.AutoComplete)unknownitem;
            if (string.IsNullOrWhiteSpace(item.namealt))
                suggestion = mentionPrefix + item.name;
            else
                suggestion = mentionPrefix + item.namealt;

            //EnableChanges = false;
            var text = MessageEditor.Text;
            text = text.Remove(caretposition - querylength, querylength);
            MessageEditor.Text = text.Insert(caretposition - querylength, suggestion);

            int afterposition = caretposition - querylength + suggestion.Length;
            int extrapadding = 0;
            if (MessageEditor.Text.Length > afterposition)
            {
                if (MessageEditor.Text[afterposition] != ' ')
                {
                    extrapadding = 1;
                    MessageEditor.Text = MessageEditor.Text.Insert(caretposition - querylength + suggestion.Length, " ");
                }
            }
            else
            {
                extrapadding = 1;
                MessageEditor.Text += ' ';
            }     
            MessageEditor.Focus(FocusState.Pointer);
            MessageEditor.SelectionStart = caretposition - querylength + suggestion.Length +extrapadding;
            SuggestionBlock.ItemsSource = null;
            SuggestionPopup.IsOpen = false;
            //EnableChanges = true;
        }
        private void InsertNewLine()
        {
            int selectionstart = MessageEditor.SelectionStart;
            MessageEditor.Text = MessageEditor.Text.Insert(selectionstart, "\n");
            MessageEditor.SelectionStart = selectionstart + 1;
        }
        private void MessageEditor_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void SuggestionBlock_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is string)
            {
                //SelectSuggestion(e.ClickedItem);
            } else
            {
                SelectSuggestion(e.ClickedItem);
            }
        }

        private void MessageEditor_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SuggestionPopup.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Flyout emojis = new Flyout();
            emojis.FlyoutPresenterStyle = (Style) App.Current.Resources["FlyoutPresenterStyle1"];
            var emojiPicker = new EmojiControl();
            emojis.Content = emojiPicker;
            emojis.ShowAt(sender as Button);
            emojis.Closed += (o, o1) =>
            {
                emojis = null;
            };
            emojiPicker.PickedEmoji += (o, args) =>
            {
                emojis.Hide();
                //if (args.names.Count > 1)
                //{
                //    int newSelectionStart = MessageEditor.SelectionStart + args.names[0].Length + args.names[1].Length + 4;
                //    MessageEditor.Text = MessageEditor.Text.Insert(MessageEditor.SelectionStart, ":" + args.names[0] + "::" + args.names[1] + ":");
                //    MessageEditor.SelectionStart = newSelectionStart;
                //} else
                //{
                //    int newSelectionStart = MessageEditor.SelectionStart + args.names[0].Length + 2;
                //    MessageEditor.Text = MessageEditor.Text.Insert(MessageEditor.SelectionStart, ":" + args.names[0] + ":");
                //    MessageEditor.SelectionStart = newSelectionStart;
                //}
                string emojiText = "";
                if (args.GetType() == typeof(EmojiControl.GuildSide))
                {
                    var emoji = (EmojiControl.GuildSide)args;
                    if (emoji.surrogates.EndsWith(".gif"))
                        emojiText = "<a:" + emoji.names.First() + ":" + emoji.id + ">";
                    else
                        emojiText = "<:" + emoji.names.First() + ":" + emoji.id + ">";
                }
                    
                else
                    emojiText = args.surrogates;

                    int newSelectionStart = MessageEditor.SelectionStart + emojiText.Length;
                    MessageEditor.Text = MessageEditor.Text.Insert(MessageEditor.SelectionStart, emojiText);
                    MessageEditor.SelectionStart = newSelectionStart;
                    MessageEditor.Focus(FocusState.Keyboard);
                
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(this, e);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(shiftisdown)
                OpenAdvanced?.Invoke(null, null);
            else
                FlyoutBase.ShowAttachedFlyout(sender as Button);
        }

        #region TextProcessing
        private string ProcessString(string input)
        {
            //Find all @ chars
            var PossibleUserMentions = AllIndexesOf('@', input);
            string output = input;
            int addedLength = 0;
            foreach (int i in PossibleUserMentions)
            {
                //remove @ sign and everything before
                string CroppedInput = input.Remove(0, i + 1);

                bool keepon = false;
                int whitespaceindex = 0;
                while (keepon && whitespaceindex < 8) //Don't waist ressources with usernames that have more than 8 whitespaces in them
                {
                    int nextWhiteSpace = AllIndexesOf(' ', CroppedInput).ElementAt(whitespaceindex);
                    if (nextWhiteSpace == 0 | nextWhiteSpace == -1)
                    {
                        keepon = true;
                        break;
                    }
                    else
                    {
                        whitespaceindex++;
                        string toVerify = CroppedInput.Remove(nextWhiteSpace);

                        //Check for role mention
                        var roleMention = LocalState.Guilds[App.CurrentGuildId].roles
                            .Where(x => x.Value.Name == toVerify)
                            .Select(e => (KeyValuePair<string, Role>?)e).FirstOrDefault();
                        if (roleMention != null)
                        {
                            //The mention is of a role
                            output = InsertMarkdown(input, "@&", roleMention.Value.Value.Id, i, toVerify.Length, addedLength);
                            addedLength = output.Length - input.Length;
                            keepon = true;
                            break;
                        }

                        //Check for nick mention
                        var nickMention = LocalState.Guilds[App.CurrentGuildId].members
                            .Where(x => x.Value.Nick == toVerify)
                            .Select(e => (KeyValuePair<string, GuildMember>?)e).FirstOrDefault();
                        if (nickMention != null)
                        {
                            //The mention is of a nick
                            output = InsertMarkdown(input, "@!", nickMention.Value.Value.User.Id, i, toVerify.Length, addedLength);
                            addedLength = output.Length - input.Length;
                            keepon = true;
                            break;
                        }

                        //Check for user mention
                        var userMention = LocalState.Guilds[App.CurrentGuildId].members
                            .Where(x => x.Value.User.Username == toVerify || x.Value.User.Username + "#" + x.Value.User.Discriminator == toVerify)
                            .Select(e => (KeyValuePair<string, GuildMember>?)e).FirstOrDefault();
                        if (userMention != null)
                        {
                            //The mention is of a user, with or without the discriminator after
                            output = InsertMarkdown(input, "@", userMention.Value.Value.User.Id, i, toVerify.Length, addedLength);
                            addedLength = output.Length - input.Length;
                            keepon = true;
                            keepon = true;
                            break;
                        }
                    }
                }
            }

            if (!App.CurrentGuildIsDM)
            {
                foreach (var chn in LocalState.Guilds[App.CurrentGuildId].Raw.Channels)
                {
                    if (chn.Type == 0)
                    {
                        string check = "<#" + chn.Id + ">";
                        StringBuilder builder = new StringBuilder();
                        builder.Append(output);
                        builder.Replace("#" + chn.Name, check);
                        output = builder.ToString();
                    }
                }

                //TODO: Improve channel mention algo
                //var possibleChannelMenetions = AllIndexesOf('#', output);
                //addedLength = 0;
                //foreach (int i in possibleChannelMenetions)
                //{

                //}
            }

            return output;
        }

        /// <summary>
        /// Insert a Discord markdown mention
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="prefix">The mention prefix (like @& for roles)</param>
        /// <param name="id">The id, main body of the mention.</param>
        /// <param name="index">The index inside the input where the mention starts</param>
        /// <param name="length">The length of the mention</param>
        /// <param name="addedlength">The length that has previously been added to the input string</param>
        /// <returns>The processed string</returns>
        private string InsertMarkdown(string input, string prefix, string id, int index, int length, int addedlength)
        {
            string output = input.Remove(index + addedlength, length+1);
            output = output.Insert(index + addedlength, "<" + prefix + id + ">");
            return output;
        }
        private List<int> AllIndexesOf(char c, string s)
        {
            var indexes = new List<int>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                    indexes.Add(i);
            }
            return indexes;
        }
        #endregion


        string previousSearch = "empty";
        private async void SearchCooldown_Tick(object sender, object e)
        {
            if (giphySearch.Text == previousSearch)
                return;
            previousSearch = giphySearch.Text;
            var service = GiphyAPI.GiphyAPI.GetGiphyService();
            
            GiphyAPI.Models.SearchResult gifs;

            if (giphySearch.Text == null || giphySearch.Text == "")
            {
                gifs = await service.Trending();
            }
            else
            {
                gifs = await service.Search(giphySearch.Text);
            }
            await (Window.Current.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progring.Visibility = Visibility.Collapsed;
                foreach (var gif in gifs.Gif)
                {
                    GiphyList.Items.Add(gif);
                }
            }));
        }

        DispatcherTimer searchCooldown = new DispatcherTimer();
        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            GiphyList.Items.Clear();
            progring.Visibility = Visibility.Visible;
            searchCooldown.Start();
            //TODO Kill any search tasks currently awaiting completion
        }

        private void GiphyList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Text += (e.ClickedItem as GiphyAPI.Models.Gif?).Value.BitlyUrl;
            GiphySelect.Visibility = Visibility.Collapsed;
            giphySearch.Text = "";
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var m = (sender as MediaElement);
            m.Position = TimeSpan.Zero;
            m.Play();
        }

        bool shiftisdown = false;
        public void ShiftDown()
        {
            if (!shiftisdown)
            {
                showAttachSymbol.Begin();
                spotifyActive.Opacity = 0;
            }
            shiftisdown = true;
        }
        public void ShiftUp()
        {
            if (shiftisdown)
            {
                showMoreSymbol.Begin();
                if (SpotifyManager.SpotifyState != null && SpotifyManager.SpotifyState.IsPlaying)
                    spotifyActive.Fade(1, 200).Start();
                else
                    spotifyActive.Opacity = 0;
            }
                
            shiftisdown = false;
        }

        private void AddGiphy_Click(object sender, RoutedEventArgs e)
        {
            GiphySelect.Visibility = GiphySelect.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            TextBox_TextChanged(null, null);
            searchCooldown.Interval = TimeSpan.FromMilliseconds(1200);
            searchCooldown.Tick += SearchCooldown_Tick;
        }

        private void AddAttachement_Click(object sender, RoutedEventArgs e)
        {
            OpenAdvanced?.Invoke(null, null);
        }

        private void AboutEncryption_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as MenuFlyoutItem);
        }

        private void ToggleEncryption_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MessageEditor_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = false;
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Bitmap) || dataPackageView.Contains(StandardDataFormats.StorageItems))
            {
                OpenAdvanced(null, new OpenAdvancedArgs() { Paste = true });
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SpotifyInvite(object sender, RoutedEventArgs e)
        {
            OpenSpotify?.Invoke(null, null);
        }

        public void Dispose()
        {
            SpotifyManager.SpotifyStateUpdated -= SpotifyManager_SpotifyStateUpdated;
            GC.Collect();
        }
        int querylength = 0;
        private void MessageEditor_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            string text = MessageEditor.Text;
            caretposition = MessageEditor.SelectionStart;
            if (text.Length > caretposition)
                text = text.Remove(caretposition);
            int loopsize = text.Length;
            int counter = 0;
            bool ranintospace = false;
            for (var i = loopsize; i > 0; i--)
            {
                counter++;
                if (counter == 32) break; //maximum username length is 32 characters, so after 32, just ignore.
                var character = text[i-1];
                if (character == '\n') break; //you systematically want to break on new lines
                else if (character == ' ') ranintospace = true;
                else if (!ranintospace && character == '#' && i != loopsize && !App.CurrentGuildIsDM)
                {
                    //This is possibly a channel
                    string query = text.Remove(0, i);
                    querylength = query.Length;
                    SearchAndDisplayChannels(query);
                    //match the channel against the last query
                    return;
                }
                else if(!ranintospace && character == ':' && i != loopsize)
                {
                    //This is possibly an emoji
                    string query = text.Remove(0, i);
                    querylength = query.Length;
                    if (App.EmojiTrie != null)
                        DisplayList(App.EmojiTrie.Retrieve(query.ToLower()));
                    return;
                }
                else if (character == '@' && i != loopsize)
                {
                    //This is possibly a user mention
                    string query = text.Remove(0, i);
                    querylength = query.Length;
                    if (App.EmojiTrie != null)
                        DisplayList(App.MemberListTrie.Retrieve(query.ToLower()));
                    return;
                }
                if(!ranintospace && loopsize>3 && i>3 && text[i-1] == '`' && text[i-2] == '`' && text[i-3] == '`')
                {
                    string query = text.Remove(0, i);
                    querylength = query.Length;
                    DisplayList(App.CodingLangsTrie.Retrieve(query.ToLower()));
                    Debug.WriteLine("Codeblock query is " + query);
                    return;
                }
            }
            //If the code reaches this far, there have been no matches
            SuggestionBlock.ItemsSource = null;
            SuggestionPopup.IsOpen = false;
        }
        
        private void SearchAndDisplayEmojis(string query)
        {
            //todo
        }
        private void SearchAndDisplayChannels(string query)
        {
            if (App.CurrentGuildId != null)
            {
                List<Common.AutoComplete> list = new List<Common.AutoComplete>();
                int counter = 0;
                foreach (var channel in LocalState.Guilds[App.CurrentGuildId].channels)
                {
                    if (counter > 12) break;
                    if (channel.Value.raw.Type == 0 && channel.Value.raw.Name.ToLower().StartsWith(query.ToLower()))
                        list.Add(new Common.AutoComplete(channel.Value.raw.Name, null, null));
                }
                DisplayList(list);
            }
        }
        private void DisplayList(IEnumerable<Common.AutoComplete> list)
        {
            int counter = 0;
            bool stopped = false;
            List<Common.AutoComplete> list2 = new List<Common.AutoComplete>();
            while (counter < 12)
            {
                var current = list.ElementAtOrDefault(counter);
                if (current == null) break;
                bool already = false;
                foreach (var element in list2)
                    if (element.name == current.name)
                    {
                        already = true;
                        break;
                    }
                if (already) continue;
                list2.Add(current);
                counter++;
            }
            if (list2.Count() == 0)
            {
                SuggestionBlock.ItemsSource = null;
                SuggestionPopup.IsOpen = false;
            }
            else
            {
                SuggestionBlock.ItemsSource = list2;
                SuggestionPopup.IsOpen = true;
            }
        }
    }

}
