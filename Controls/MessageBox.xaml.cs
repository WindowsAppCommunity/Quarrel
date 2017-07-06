using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public event EventHandler<AutoSuggestBoxTextChangedEventArgs> TextChanged;
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
        private void MessageEditor_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            try
            {
                SendBox.IsEnabled = !String.IsNullOrWhiteSpace(MessageEditor.Text.Trim());
                //WHY THE FUCK IS THIS FIRING EVEN WHEN THE TEXT IS CHANGED PROGRAMATICALLY
                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    MessageEditor.ItemsSource = null;
                    string str = MessageEditor.Text;
                    Debug.WriteLine("TEXTCHANGED");
                    if (UnderlyingTextBox.SelectionStart < str.Length)
                    {
                        selectionstart = UnderlyingTextBox.SelectionStart;
                        str = str.Remove(UnderlyingTextBox.SelectionStart);
                    }

                    if (!String.IsNullOrWhiteSpace(str))
                    {
                        if (str.Last() != ' ' || str.Count() < 2) //cancel if the last letter is a space or there are less than 2 chars
                        {
                            str = str.Trim();
                            string word = "";
                            //if the letter contains a space, get the last word, otherwise the string is a single word
                            if (str.Contains(' '))
                                word = str.Split(' ').Last();
                            else
                                word = str;

                            if (word.StartsWith("@"))
                            {
                                string query = word.Remove(0, 1);
                                PureText = MessageEditor.Text.Remove(selectionstart - word.Length, word.Length);
                                PureText = MessageEditor.Text.Insert(selectionstart - word.Length, "<&&DISCORD_UWP//MENTION&&>");
                                if (query != "")
                                {
                                    IEnumerable<string> userlist = App.GuildMembers.Where(x => x.Value.Raw.User.Username.StartsWith(query)).Select(x => "@" + x.Value.Raw.User.Username);
                                    IEnumerable<string> rolelist = App.CurrentGuild.Roles.Where(x => x.Value.Name.StartsWith(query) && x.Value.Mentionable).Select(x => "@" + x.Value.Name);
                                    rolelist.Concat(new List<string> { "@here", "@everyone" });
                                    MessageEditor.ItemsSource = userlist.Concat(rolelist);
                                }
                            }
                            if (word.StartsWith("#"))
                            {
                                PureText = MessageEditor.Text.Remove(selectionstart - word.Length, word.Length);
                                PureText = MessageEditor.Text.Insert(selectionstart - word.Length, "<&&DISCORD_UWP//MENTION&&>");
                                string query = word.Remove(0, 1);
                                MessageEditor.ItemsSource = App.CurrentGuild.Channels.Where(x => x.Value.Raw.Type == 0 && x.Value.Raw.Name.StartsWith(query)).Select(x => "#" + x.Value.Raw.Name);
                            }
                        }

                    }
                    else
                    {
                        MessageEditor.ItemsSource = null;
                    }

                    Text = str;
                    
                }
            }
            catch
            {
                MessageEditor.ItemsSource = null;
            }
            TextChanged?.Invoke(sender, args);
        }
        TextBox UnderlyingTextBox;
        private void MessageEditor_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            EnableChanges = false;
            var str = MessageEditor.Text;
            MessageEditor.Text = PureText.Replace("<&&DISCORD_UWP//MENTION&&>", args.SelectedItem as string);
            UnderlyingTextBox.SelectionStart = PureText.IndexOf("<&&DISCORD_UWP//MENTION&&>") + (args.SelectedItem as string).Length;
            EnableChanges = true;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            UnderlyingTextBox = sender as TextBox;
        }
    }
}
