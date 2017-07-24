using Discord_UWP.CacheModels;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord_UWP.Controls
{
    public sealed partial class MessageBox : UserControl
    {
        public event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler<RoutedEventArgs> Send;
        public event EventHandler<RoutedEventArgs> Cancel;

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(MessageBox),
            new PropertyMetadata("", OnPropertyChangedStatic));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
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
        }

        public void Clear()
        {
            MessageEditor.Text = "";
            SendBox.IsEnabled = false;
        }

        private async void OnPropertyChanged(DependencyObject d, DependencyProperty prop)
        {
            if (prop == IsEnabledProperty)
            {
                MessageEditor.IsEnabled = IsEnabled;
                SendBox.IsEnabled = IsEnabled;
            }
            if(prop == TextProperty)
            {
                MessageEditor.Text = Text;
            }
            if(prop == IsEditProperty)
            {
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void SendBox_OnClick(object sender, RoutedEventArgs e)
        {
            Text = ProcessString(Text);
            Send?.Invoke(sender, e);
        }

        string PureText = "";
        int selectionstart = 0;
        bool EnableChanges = true;

        private void MessageEditor_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SendBox.IsEnabled = !String.IsNullOrWhiteSpace(MessageEditor.Text.Trim());
                //WHY THE FUCK IS THIS FIRING EVEN WHEN THE TEXT IS CHANGED PROGRAMATICALLY
                SuggestionBlock.ItemsSource = null;
                string str = MessageEditor.Text;
                Debug.WriteLine("TEXTCHANGED");
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
                    if (str.Last() != ' ' || str.Count() < 2
                    ) //cancel if the last letter is a space or there are less than 2 chars
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
                            string query = word.Remove(0, 1);
                            selectionstart = selectionstart - word.Length;
                            PureText = MessageEditor.Text.Remove(selectionstart, word.Length);
                            if (query != "")
                            {
                                //Something in this block is crashing the UI, but I dunno what
                                IEnumerable<string> userlist =
                                    App.GuildMembers.Where(x => x.Value.Raw.User.Username.StartsWith(query))
                                        .Select(x => "@" + x.Value.Raw.User.Username + "#" + x.Value.Raw.User.Discriminator);
                                IEnumerable<string> rolelist =
                                    App.CurrentGuild.Roles
                                        .Where(x => x.Value.Name.StartsWith(query) && x.Value.Mentionable)
                                        .Select(x => "@" + x.Value.Name);
                                rolelist.Concat(new List<string> {"@here", "@everyone"});
                                SuggestionBlock.ItemsSource = userlist.Concat(rolelist);
                                SuggestionPopup.IsOpen = true;
                            }
                        }
                        else if (word.StartsWith("#"))
                        {
                            string query = word.Remove(0, 1);
                            selectionstart = selectionstart - word.Length;
                            PureText = MessageEditor.Text.Remove(selectionstart, word.Length);
                            
                            SuggestionBlock.ItemsSource = App.CurrentGuild.Channels
                                .Where(x => x.Value.Raw.Type == 0 && x.Value.Raw.Name.StartsWith(query))
                                .Select(x => "#" + x.Value.Raw.Name);
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
                Text = str;
            }
            catch
            {
                SuggestionBlock.ItemsSource = null;
                SuggestionPopup.IsOpen = false;
            }
            TextChanged?.Invoke(sender, e);
        }

        private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PopupTransform.Y = -e.NewSize.Height;
        }

        private void SelectSuggestion(string suggestion)
        {
            EnableChanges = false;
            var str = MessageEditor.Text;
            MessageEditor.Text = PureText.Insert(selectionstart, suggestion);
            MessageEditor.Focus(FocusState.Pointer);
            MessageEditor.SelectionStart = selectionstart + suggestion.Length;
            SuggestionBlock.ItemsSource = null;
            SuggestionPopup.IsOpen = false;
            EnableChanges = true;
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
                        SelectSuggestion(SuggestionBlock.SelectedItem as string);
                    else
                        SendBox_OnClick(null, null);
                }
                else if (HandleSuggestions)
                    SelectSuggestion(SuggestionBlock.SelectedItem as string);
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
            SelectSuggestion(e.ClickedItem as string);
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
                int newSelectionStart = MessageEditor.SelectionStart + args.surrogates.Length;
                MessageEditor.Text = MessageEditor.Text.Insert(MessageEditor.SelectionStart, args.surrogates);
                MessageEditor.SelectionStart = newSelectionStart;
                MessageEditor.Focus(FocusState.Keyboard);
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(this, e);
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
                        var roleMention = Storage.Cache.Guilds[App.CurrentGuildId].Roles
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
                        var nickMention = Storage.Cache.Guilds[App.CurrentGuildId].Members
                            .Where(x => x.Value.Raw.Nick == toVerify)
                            .Select(e => (KeyValuePair<string, CacheModels.Member>?)e).FirstOrDefault();
                        if (nickMention != null)
                        {
                            //The mention is of a nick
                            output = InsertMarkdown(input, "@!", nickMention.Value.Value.Raw.User.Id, i, toVerify.Length, addedLength);
                            addedLength = output.Length - input.Length;
                            keepon = true;
                            break;
                        }

                        //Check for user mention
                        var userMention = Storage.Cache.Guilds[App.CurrentGuildId].Members
                            .Where(x => x.Value.Raw.User.Username == toVerify || x.Value.Raw.User.Username + "#" + x.Value.Raw.User.Discriminator == toVerify)
                            .Select(e => (KeyValuePair<string, Member>?)e).FirstOrDefault();
                        if (userMention != null)
                        {
                            //The mention is of a user, with or without the discriminator after
                            output = InsertMarkdown(input, "@", userMention.Value.Value.Raw.User.Id, i, toVerify.Length, addedLength);
                            addedLength = output.Length - input.Length;
                            keepon = true;
                            keepon = true;
                            break;
                        }
                    }
                }
            }

            foreach (var chn in Storage.Cache.Guilds[App.CurrentGuildId].RawGuild.Channels)
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
    }
}
