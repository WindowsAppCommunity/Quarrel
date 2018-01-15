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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MessageBox : UserControl
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler<RoutedEventArgs> Send;
        public event EventHandler<RoutedEventArgs> Cancel;
        public event EventHandler<RoutedEventArgs> OpenAdvanced;

        public string Text
        {
            get { return MessageEditor.Text; }
            set { MessageEditor.Text = value; }
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
                    GiphyButton.Visibility = Visibility.Collapsed;
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
            Text = ProcessString(Text);
            Send?.Invoke(sender, e);
        }

        string PureText = "";
        int selectionstart = 0;
        //bool EnableChanges = true;
        string mentionPrefix = "";
        private void MessageEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO Optimize the hell out of this
            SendBox.IsEnabled = !String.IsNullOrWhiteSpace(MessageEditor.Text.Trim());
            //WHY THE FUCK IS THIS FIRING EVEN WHEN THE TEXT IS CHANGED PROGRAMATICALLY
            SuggestionBlock.ItemsSource = null;
            string str = MessageEditor.Text;
            if (MessageEditor.SelectionStart < str.Length)
            {
                selectionstart = MessageEditor.SelectionStart;
                str = str.Remove(MessageEditor.SelectionStart);
            }
            else
            {
                selectionstart = MessageEditor.Text.Length;
            }

            if (!String.IsNullOrWhiteSpace(str))
            {
                if (str.Last() != ' ' || str.Count() < 2) 
                //cancel if the last letter is a space or there are less than 2 chars
                {
                    str = str.Trim();
                    string word = "";
                    //if the letter contains a space, get the last word, otherwise the string is a single word
                    if (str.Contains(' '))
                        word = str.Split(' ').Last();
                    else if (str.Contains('\r'))
                        word = str.Split('\r').Last();
                    else word = str;

                    if (word.StartsWith("@"))
                    {
                        mentionPrefix = "@";
                        string query = word.Remove(0, 1);
                        selectionstart = selectionstart - word.Length;
                        PureText = MessageEditor.Text.Remove(selectionstart, word.Length);
                        if (query != "")
                        {
                            if (App.MemberListDawg == null)
                            {
                                SuggestionBlock.ItemsSource = null;
                                SuggestionPopup.IsOpen = false;
                            }
                            else
                            {
                                var list = App.MemberListDawg.MatchPrefix(query).Take(12);
                                if(list.Count() == 0)
                                {
                                    SuggestionBlock.ItemsSource = null;
                                    SuggestionPopup.IsOpen = false;
                                }
                                else
                                {
                                    SuggestionBlock.ItemsSource = list;
                                    SuggestionPopup.IsOpen = true;
                                }
                                
                            }
                        }
                    }
                    else if (word.StartsWith("#"))
                    {
                        mentionPrefix = "";
                        string query = word.Remove(0, 1);
                        selectionstart = selectionstart - word.Length;
                        PureText = MessageEditor.Text.Remove(selectionstart, word.Length);

                        SuggestionBlock.ItemsSource = LocalState.Guilds[App.CurrentGuildId].channels
                            .Where(x => x.Value.raw.Type == 0 && x.Value.raw.Name.StartsWith(query))
                            .Select(x => "#" + x.Value.raw.Name);
                        SuggestionPopup.IsOpen = true;
                    }
                    else
                    {
                        SuggestionBlock.ItemsSource = null;
                        SuggestionPopup.IsOpen = false;
                    }
                }
                else
                {
                    SuggestionBlock.ItemsSource = null;
                    SuggestionPopup.IsOpen = false;
                }
            }
            else
            {
                SuggestionBlock.ItemsSource = null;
                SuggestionPopup.IsOpen = false;
            }
        }
        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PopupTransform.Y = -e.NewSize.Height;
        }

        private void SelectSuggestion(KeyValuePair<string, DawgSharp.DawgItem> item)
        {
          
            string suggestion = mentionPrefix + item.Value.InsertText;
            if (suggestion == "")
                suggestion = mentionPrefix + item.Key;
            //EnableChanges = false;
            var str = MessageEditor.Text;
            MessageEditor.Text = PureText.Insert(selectionstart, suggestion);
            MessageEditor.Focus(FocusState.Pointer);
            MessageEditor.SelectionStart = selectionstart + suggestion.Length;
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
            bool HandleSuggestions = (SuggestionBlock.Items.Count != 0);
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                
                Windows.Devices.Input.KeyboardCapabilities keyboardCapabilities = new Windows.Devices.Input.KeyboardCapabilities();
                if (keyboardCapabilities.KeyboardPresent > 0)
                {
                    if (CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down))
                    {
                        InsertNewLine();
                    }
                    else if (HandleSuggestions)
                        SelectSuggestion((KeyValuePair<string, DawgSharp.DawgItem>)SuggestionBlock.SelectedItem);
                    else
                        SendBox_OnClick(null, null);
                }
                else if (HandleSuggestions)
                    SelectSuggestion((KeyValuePair<string, DawgSharp.DawgItem>)SuggestionBlock.SelectedItem);
                else
                    InsertNewLine();
            }

            if (e.Key == VirtualKey.Up)
            {
                e.Handled = true;
                if (SuggestionBlock.SelectedIndex == -1 || SuggestionBlock.SelectedIndex == 0)                
                    SuggestionBlock.SelectedIndex = SuggestionBlock.Items.Count-1;
                else
                    SuggestionBlock.SelectedIndex = SuggestionBlock.SelectedIndex - 1;
            }
            else if (e.Key == VirtualKey.Down)
            {
                e.Handled = true;

                if (SuggestionBlock.SelectedIndex == -1 || SuggestionBlock.SelectedIndex == SuggestionBlock.Items.Count-1)
                    SuggestionBlock.SelectedIndex = 0;
                else
                    SuggestionBlock.SelectedIndex = SuggestionBlock.SelectedIndex + 1;
            }
        }

        private void SuggestionBlock_OnItemClick(object sender, ItemClickEventArgs e)
        {
            SelectSuggestion((KeyValuePair<string, DawgSharp.DawgItem>)e.ClickedItem);
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
                if (args.CustomEmoji)
                    emojiText = ":" + args.names.First() + ":";
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
            OpenAdvanced?.Invoke(null, null);
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
                while (keepon == false && whitespaceindex < 8) //Don't waist ressources with usernames that have more than 8 whitespaces in them
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

        private void GiphyButton_Click(object sender, RoutedEventArgs e)
        {
            GiphySelect.Visibility = GiphySelect.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            TextBox_TextChanged(null, null);
            searchCooldown.Interval = TimeSpan.FromMilliseconds(1200);
            searchCooldown.Tick += SearchCooldown_Tick;
        }

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
    }
}
