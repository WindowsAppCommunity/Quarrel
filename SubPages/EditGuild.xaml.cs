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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Discord_UWP.SubPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditGuild : Page
    {
        public EditGuild()
        {
            this.InitializeComponent();
        }

        private void SaveGuildSettings(object sender, RoutedEventArgs e)
        {
            Discord_UWP.API.Guild.Models.ModifyGuild modifyguild = new Discord_UWP.API.Guild.Models.ModifyGuild() { Name = GuildName.Text};
            Task.Run(() =>
            {
                Session.ModifyGuild(guildId, modifyguild);
            });
            CloseButton_Click(null, null);
        }

        private string guildId = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            guildId = e.Parameter.ToString();
            var guild = Storage.Cache.Guilds[guildId];
            GuildName.Text = guild.RawGuild.Name;
            Session.Gateway.GuildUpdated += GuildUpdated;
            Session.Gateway.GuildBanAdded += BanAdded;
            Session.Gateway.GuildBanRemoved += BanRemoved;
        }

        private async void BanRemoved(object sender, GatewayEventArgs<Gateway.DownstreamEvents.GuildBanUpdate> e)
        {
            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        for (int x = 0; x < BanView.Items.Count; x++)
                        {
                            if ((BanView.Items[x] as Ban?).Value.User.Id == e.EventData.User.Id)
                            {
                                BanView.Items.RemoveAt(x);
                            }
                        }

                        if (BanView.Items.Count == 0)
                        {
                            NoBans.Fade(0.2f, 200).Start();
                        }
                    });
            }
        }

        private async void BanAdded(object sender, GatewayEventArgs<Gateway.DownstreamEvents.GuildBanUpdate> e)
        {
            if (e.EventData.GuildId == App.CurrentGuildId)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                       () =>
                       {
                           if (BanView.Items.Count == 0)
                           {
                               NoBans.Fade(0.0f, 200).Start();
                           }
                           BanView.Items.Add(new Ban() { User = e.EventData.User });
                       });
            }
        }

        private async void GuildUpdated(object sender, GatewayEventArgs<Guild> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    if (guildId == e.EventData.Id)
                    {
                        GuildName.Text = e.EventData.Name;
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
            Session.Gateway.GuildUpdated -= GuildUpdated;
            Session.Gateway.GuildBanAdded -= BanAdded;
            Session.Gateway.GuildBanRemoved -= BanRemoved;
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        //Regex regex = new Regex("^[A-Za-z0-9_-]+$");
        private void ChannelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int charCounter = GuildName.Text.Length;
            NameCharCounter.Text = (100 - charCounter).ToString();

            if (charCounter < 2 || charCounter >= 100)
            {
                NameCharCounter.Foreground = new SolidColorBrush(Color.FromArgb(255, 240, 71, 71));
                if (charCounter > 100)
                {
                    GuildName.Text = GuildName.Text.Remove(100);
                    GuildName.SelectionStart = 100;
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

        private bool LoadingInvites = false;
        private bool loadingBans = false;
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
                    invites = await Session.GetChannelInvites(guildId);
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
            } else if (pivot.SelectedIndex == 2 && !loadingBans)
            {
                BanView.Items.Clear();
                NoBans.Opacity = 0;
                LoadingBans.Opacity = 1;
                loadingBans = true;
                IEnumerable<Ban> bans = null;
                await Task.Run(async () =>
                {
                    bans = await Session.GetGuildBans(guildId);
                });
                if (bans != null)
                {
                    foreach (var ban in bans)
                    {
                        BanView.Items.Add(ban);
                    }
                    LoadingBans.Fade(0, 200).Start();
                }
                if (BanView.Items.Count == 0)
                {
                    NoBans.Fade(0.2f, 200).Start();
                    LoadingBans.Fade(0, 200).Start();
                }
            }
        }

        private async void InviteControl_OnDeleteInvite(object sender, EventArgs e)
        {
            string code = ((Invite)sender).String;
            await Session.DeleteInvite(code);
            InviteView.Items.Remove(InviteView.Items.FirstOrDefault(x => ((Invite)x).String == code));
        }
    }
}
