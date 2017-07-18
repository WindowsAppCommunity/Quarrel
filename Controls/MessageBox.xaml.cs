using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
        }

        private void SendBox_OnClick(object sender, RoutedEventArgs e)
        {
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
                        if (word.StartsWith("#"))
                        {
                            string query = word.Remove(0, 1);
                            selectionstart = selectionstart - word.Length;
                            PureText = MessageEditor.Text.Remove(selectionstart, word.Length);
                            
                            SuggestionBlock.ItemsSource = App.CurrentGuild.Channels
                                .Where(x => x.Value.Raw.Type == 0 && x.Value.Raw.Name.StartsWith(query))
                                .Select(x => "#" + x.Value.Raw.Name);
                            SuggestionPopup.IsOpen = true;
                        }
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
        private void MessageEditor_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (SuggestionBlock.Items.Count == 0) return;

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
            else if (e.Key == VirtualKey.Enter)
            {
                SelectSuggestion(SuggestionBlock.SelectedItem as string);
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
                int newSelectionStart = MessageEditor.SelectionStart + (o as string).Length;
                MessageEditor.Text = MessageEditor.Text.Insert(MessageEditor.SelectionStart, (o as string));
                MessageEditor.SelectionStart = newSelectionStart;
                MessageEditor.Focus(FocusState.Keyboard);
            };
        }
    }
}
