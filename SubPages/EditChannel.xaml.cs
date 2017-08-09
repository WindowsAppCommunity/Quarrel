using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Discord_UWP.Gateway;
using Discord_UWP.SharedModels;
using Microsoft.Toolkit.Uwp.UI.Animations;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class EditChannel : Page
    {
        public EditChannel()
        {
            this.InitializeComponent();
            header.Text = App.Translate("EditChannel").ToUpper();
            OverviewItem.Header = App.Translate("Overview");
            ChannelName.Header = App.Translate("Name");
            ChannelTopic.Header = App.Translate("Topic");
            NsfwSwitch.OnContent = App.Translate("NSFW " + App.Translate("Channel"));
            InvitesItem.Header = App.Translate("Invites");
            button.Content = App.Translate("Cancel");
            SaveButton.Content = App.Translate("Save");
        }

        private void SaveChannelSettings(object sender, RoutedEventArgs e)
        {
            Discord_UWP.API.Channel.Models.ModifyChannel modifychannel = new Discord_UWP.API.Channel.Models.ModifyChannel() { Name = ChannelName.Text, Topic = ChannelTopic.Text, Bitrate = 64000, Position = Storage.Cache.Guilds[App.CurrentGuildId].Channels[channelId].Raw.Position, NSFW = NsfwSwitch.IsOn };
            Task.Run(() =>
            {
                Session.ModifyGuildChannel(channelId, modifychannel);
            });
            CloseButton_Click(null,null);
        }

        private string channelId = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            channelId = e.Parameter.ToString();
            var channel = Storage.Cache.Guilds.FirstOrDefault(x => x.Value.Channels.ContainsKey(channelId))
                .Value.Channels[channelId];
            ChannelName.Text = channel.Raw.Name;
            header.Text = App.GetString("EDIT") + " " + channel.Raw.Name.ToUpper();
            if(channel.Raw.Topic != null)
                ChannelTopic.Text = channel.Raw.Topic;
            Session.Gateway.GuildChannelUpdated += ChannelUpdated;
            ChannelName_TextChanged(null, null);
            ChannelTopic_OnTextChanged(null,null);
        }

        private async void ChannelUpdated(object sender, GatewayEventArgs<GuildChannel> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (channelId == e.EventData.Id)
                    {
                        ChannelName.Text = e.EventData.Name;
                        if (e.EventData.Topic == null)
                            ChannelTopic.Text = "";
                        else
                            ChannelTopic.Text = e.EventData.Topic;
                    }
                });
        }

        private void NavAway_Completed(object sender, object e)
        {
            Frame.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            CloseButton_Click(null, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Session.Gateway.GuildChannelUpdated -= ChannelUpdated;
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //the IsOn appears to fire *after* you change the toggle state, so this needs to be inverted
            //if (NsfwSwitch.IsOn && IgnoreToggle == false)
            //{
            //    if (!ChannelName.Text.StartsWith("nsfw-") || ChannelName.Text != "nsfw")
            //        if (ChannelName.Text == "")
            //            ChannelName.Text = "nsfw";
            //        else
            //            ChannelName.Text = "nsfw-" + ChannelName.Text;
            //}
            //else if(IgnoreToggle == false)
            //{
            //    if (ChannelName.Text.StartsWith("nsfw-"))
            //        ChannelName.Text = ChannelName.Text.Remove(0,5);
            //    else if (ChannelName.Text == "nsfw")
            //        ChannelName.Text = "";
            //}
            //IgnoreToggle = false;
        }
        Regex regex = new Regex("^[A-Za-z0-9_-]+$");
        private bool IgnoreToggle = false;
        private void ChannelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //{
            //    if (ChannelName.Text.StartsWith("nsfw-") || ChannelName.Text == "nsfw")
            //    {
            //        if (!NsfwSwitch.IsOn) IgnoreToggle = true;
            //        NsfwSwitch.IsOn = true;
            //    }
            //    else if (NsfwSwitch.IsOn)
            //        NsfwSwitch.IsOn = false;
            //}


            if (!regex.IsMatch(ChannelName.Text))
            {
                if(ChannelName.Text != "")
                    CharacterWarning.Visibility = Visibility.Visible;
                else
                    CharacterWarning.Visibility = Visibility.Collapsed;
                NameContent = false;
            }
            else
            {
                CharacterWarning.Visibility = Visibility.Collapsed;
                NameContent = true;
            }

            int charCounter = ChannelName.Text.Length;
            NameCharCounter.Text = (100 - charCounter).ToString();

            if (charCounter < 2 || charCounter >= 100)
            {
                NameCharCounter.Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71));
                if (charCounter > 100)
                {
                    ChannelName.Text = ChannelName.Text.Remove(100);
                    ChannelName.SelectionStart = 100;
                }
                if (charCounter < 2)
                    NameLength = false;
            }
            else
            {
                NameLength = true;
                NameCharCounter.Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"];
            }
            CheckForSave();
        }

        private bool NameLength = true;
        private bool NameContent = true;
        private void CheckForSave()
        {
            if (NameLength && NameContent)
                SaveButton.IsEnabled = true;
            else
                SaveButton.IsEnabled = false;
        }
        private void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            string filtered = RemoveDiacritics(ChannelName.Text.Replace(' ', '_').Replace("@","-at-"));
            string filtered2 = new Regex(@"[(){}|~=+:;.,?!%&[\]]").Replace(filtered, "-");
            ChannelName.Text = new Regex("[^a-zA-Z0-9%_]").Replace(filtered2, "");
        }

        public string RemoveDiacritics(string input)
        {
            string stFormD = input.Normalize(NormalizationForm.FormD);
            int len = stFormD.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private void ChannelTopic_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int charCounter = ChannelTopic.Text.Length;
            CharCounter.Text = (1024 - charCounter).ToString();
            if (charCounter >= 1024)
            {
                CharCounter.Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71));
                if (charCounter > 1024)
                {
                    ChannelTopic.Text = ChannelTopic.Text.Remove(1024);
                    ChannelTopic.SelectionStart = 1024;
                }
            }
                
            else
                CharCounter.Foreground = (SolidColorBrush)App.Current.Resources["InvertedBG"];
        }

        private bool LoadingInvites = false;
        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivot.SelectedIndex == 1 && !LoadingInvites)
            {
                InviteView.Items.Clear();
                NoInvites.Opacity = 0;
                LoadingInvite.Opacity = 1;
                LoadingInvites = true;
                IEnumerable<Invite> invites = null;
                await Task.Run(async () =>
                 {
                     invites = await Session.GetChannelInvites(channelId);
                 });
                if (invites != null)
                {
                    foreach (var invite in invites)
                    {
                        InviteView.Items.Add(invite);
                    }
                    LoadingInvite.Fade(0, 200).Start();
                }
                if (InviteView.Items.Count == 0)
                {
                    NoInvites.Fade(0.2f, 200).Start();
                    LoadingInvite.Fade(0, 200).Start();
                }
            }
        }

        private async void InviteControl_OnDeleteInvite(object sender, EventArgs e)
        {
            string code = ((Invite)sender).String;
            await Session.DeleteInvite(code);
            InviteView.Items.Remove(InviteView.Items.FirstOrDefault(x => ((Invite)x).String == code));
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
