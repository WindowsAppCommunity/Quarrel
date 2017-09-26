using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Discord_UWP.LocalModels;
using Discord_UWP.Managers;

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
            header.Text = App.GetString("/Flyouts/EditServer").ToUpper();
            GuildName.Header = App.GetString("/Flyouts/Name");
            Roles.Header = App.GetString("/Flyouts/Roles");
            RoleName.Header = App.GetString("/Flyouts/Name").ToUpper();
            Bans.Header = App.GetString("/Flyouts/Bans");
            Invites.Visibility = Visibility.Collapsed;
            Bans.Visibility = Visibility.Collapsed;
        }

        private void SaveGuildSettings(object sender, RoutedEventArgs e)
        {
            Discord_UWP.API.Guild.Models.ModifyGuild modifyguild = new Discord_UWP.API.Guild.Models.ModifyGuild() { Name = GuildName.Text};
            Task.Run(async () =>
            {
                await RESTCalls.ModifyGuild(guildId, modifyguild); //TODO: Rig to App.Events
            });
            CloseButton_Click(null, null);
        }

        private void SaveRoleSettings()
        {
            if (!loadingRoles)
            {
                LocalState.Guilds[guildId].permissions.Administrator = Administrator.IsOn;
                LocalState.Guilds[guildId].permissions.ViewAuditLog = ViewAuditLog.IsOn;
                LocalState.Guilds[guildId].permissions.ManangeGuild = ManageServer.IsOn;
                LocalState.Guilds[guildId].permissions.ManageRoles = ManageRoles.IsOn;
                LocalState.Guilds[guildId].permissions.ManageChannels = ManageChannels.IsOn;
                LocalState.Guilds[guildId].permissions.KickMembers = KickMembers.IsOn;
                LocalState.Guilds[guildId].permissions.BanMembers = BanMembers.IsOn;
                LocalState.Guilds[guildId].permissions.CreateInstantInvite = CreateInstantInvite.IsOn;
                LocalState.Guilds[guildId].permissions.ChangeNickname = ChangeNickname.IsOn;
                LocalState.Guilds[guildId].permissions.ManageNicknames = ManageNicknames.IsOn;
                LocalState.Guilds[guildId].permissions.ManageEmojis = ManageEmojis.IsOn;
                LocalState.Guilds[guildId].permissions.ManageWebhooks = ManageWebhooks.IsOn;

                LocalState.Guilds[guildId].permissions.ReadMessages = ReadMessages.IsOn;
                LocalState.Guilds[guildId].permissions.SendMessages = SendMessages.IsOn;
                LocalState.Guilds[guildId].permissions.SendTtsMessages = SendTtsMessages.IsOn;
                LocalState.Guilds[guildId].permissions.ManageMessages = ManageMessages.IsOn;
                LocalState.Guilds[guildId].permissions.EmbedLinks = EmbedLinks.IsOn;
                LocalState.Guilds[guildId].permissions.AttachFiles = AttachFiles.IsOn;
                LocalState.Guilds[guildId].permissions.ReadMessageHistory = ReadMessageHistory.IsOn;
                LocalState.Guilds[guildId].permissions.MentionEveryone = MentionEveryone.IsOn;
                LocalState.Guilds[guildId].permissions.UseExternalEmojis = UseExternalEmojis.IsOn;
                LocalState.Guilds[guildId].permissions.AddReactions = AddReactions.IsOn;

                LocalState.Guilds[guildId].permissions.Connect = ConnectPerm.IsOn;
                LocalState.Guilds[guildId].permissions.Speak = Speak.IsOn;
                LocalState.Guilds[guildId].permissions.MuteMembers = MuteMembers.IsOn;
                LocalState.Guilds[guildId].permissions.DeafenMembers = DeafenMembers.IsOn;
                LocalState.Guilds[guildId].permissions.MoveMembers = MoveMembers.IsOn;
                LocalState.Guilds[guildId].permissions.UseVad = UseVad.IsOn;
                string roleId = (RolesView.SelectedItem as SimpleRole).Id;
                Discord_UWP.API.Guild.Models.ModifyGuildRole modifyguildrole = new Discord_UWP.API.Guild.Models.ModifyGuildRole() { Name = RoleName.Text, Color = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id].Color, Hoist = Hoist.IsOn, Permissions = LocalState.Guilds[guildId].permissions.GetPermInt(), Position = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id].Position };
                Task.Run(async () =>
                {
                    await RESTCalls.ModifyGuildRole(guildId, roleId, modifyguildrole); //TODO: Rig to App.Events
                });
            }
        }

        private string guildId = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            guildId = e.Parameter.ToString();
            var guild = LocalState.Guilds[guildId];
            GuildName.Text = guild.Raw.Name;
            header.Text = App.GetString("/Flyouts/Edit").ToUpper() + " " + guild.Raw.Name.ToUpper();
            if (!LocalState.Guilds[guildId].permissions.ManangeGuild && !LocalState.Guilds[guildId].permissions.Administrator && LocalState.Guilds[guildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                GuildName.IsEnabled = false;
                pivot.Items.Remove(Invites);
            }
            if (!LocalState.Guilds[guildId].permissions.BanMembers && !LocalState.Guilds[guildId].permissions.Administrator && LocalState.Guilds[guildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                pivot.Items.Remove(Bans);
            }
            if (true) //TODO: Set role permissions
            {
                foreach (Role role in LocalState.Guilds[guildId].roles.Values.OrderBy(x => x.Position))
                {
                    RolesView.Items.Add(new SimpleRole(role.Id, role.Name, Common.IntToColor(role.Color)));
                }
                RolesView.SelectedIndex = 0;
            }
            GatewayManager.Gateway.GuildUpdated += GuildUpdated;
            GatewayManager.Gateway.GuildBanAdded += BanAdded;
            GatewayManager.Gateway.GuildBanRemoved += BanRemoved;
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

        private async void GuildUpdated(object sender, GatewayEventArgs<SharedModels.Guild> e)
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
            GatewayManager.Gateway.GuildUpdated -= GuildUpdated;
            GatewayManager.Gateway.GuildBanAdded -= BanAdded;
            GatewayManager.Gateway.GuildBanRemoved -= BanRemoved;
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            App.SubpageClosed();
        }

        //Regex regex = new Regex("^[A-Za-z0-9_-]+$");
        private void ServerName_TextChanged(object sender, TextChangedEventArgs e)
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
        private bool loadingRoles = false;
        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((pivot.SelectedItem as PivotItem).Header.ToString() == "Invites" && !LoadingInvites)
            {
                InviteView.Items.Clear();
                NoInvites.Opacity = 0;
                LoadingInvite.Opacity = 1;
                LoadingInvites = true;
                IEnumerable<Invite> invites = null;
                await Task.Run(async () =>
                {
                    invites = await RESTCalls.GetChannelInvites(guildId); //TODO: Rig to App.Events
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
            } else if ((pivot.SelectedItem as PivotItem).Header.ToString() == "Bans" && !loadingBans)
            {
                BanView.Items.Clear();
                NoBans.Opacity = 0;
                LoadingBans.Opacity = 1;
                loadingBans = true;
                IEnumerable<Ban> bans = null;
                await Task.Run(async () =>
                {
                    bans = await RESTCalls.GetGuildBans(guildId); //TODO: Rig to App.Events
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
            await RESTCalls.DeleteInvite(code); //TODO: Rig to App.Events
            InviteView.Items.Remove(InviteView.Items.FirstOrDefault(x => ((Invite)x).String == code));
        }

        private void RolesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadingRoles = true;
            Role role = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id];
            if ((role.Position >= LocalState.Guilds[guildId].GetHighestRole(LocalState.Guilds[guildId].members[LocalState.CurrentUser.Id].Roles).Position || (!LocalState.Guilds[guildId].permissions.ManageRoles && !LocalState.Guilds[guildId].permissions.Administrator)) && LocalState.Guilds[guildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                RoleName.IsEnabled = Hoist.IsEnabled = AllowMention.IsEnabled = Administrator.IsEnabled = ViewAuditLog.IsEnabled = ManageServer.IsEnabled = ManageRoles.IsEnabled = ManageChannels.IsEnabled = KickMembers.IsEnabled = BanMembers.IsEnabled = CreateInstantInvite.IsEnabled = ChangeNickname.IsEnabled = ManageNicknames.IsEnabled = ManageEmojis.IsEnabled = ManageWebhooks.IsEnabled = ReadMessages.IsEnabled = SendMessages.IsEnabled = SendTtsMessages.IsEnabled = ManageMessages.IsEnabled = EmbedLinks.IsEnabled = AttachFiles.IsEnabled = ReadMessageHistory.IsEnabled = MentionEveryone.IsEnabled = UseExternalEmojis.IsEnabled = AddReactions.IsEnabled = ConnectPerm.IsEnabled = Speak.IsEnabled = MuteMembers.IsEnabled = DeafenMembers.IsEnabled = MoveMembers.IsEnabled = UseVad.IsEnabled = false;
            } else
            {
                RoleName.IsEnabled = Hoist.IsEnabled = AllowMention.IsEnabled = Administrator.IsEnabled = ViewAuditLog.IsEnabled = ManageServer.IsEnabled = ManageRoles.IsEnabled = ManageChannels.IsEnabled = KickMembers.IsEnabled = BanMembers.IsEnabled = CreateInstantInvite.IsEnabled = ChangeNickname.IsEnabled = ManageNicknames.IsEnabled = ManageEmojis.IsEnabled = ManageWebhooks.IsEnabled = ReadMessages.IsEnabled = SendMessages.IsEnabled = SendTtsMessages.IsEnabled = ManageMessages.IsEnabled = EmbedLinks.IsEnabled = AttachFiles.IsEnabled = ReadMessageHistory.IsEnabled = MentionEveryone.IsEnabled = UseExternalEmojis.IsEnabled = AddReactions.IsEnabled = ConnectPerm.IsEnabled = Speak.IsEnabled = MuteMembers.IsEnabled = DeafenMembers.IsEnabled = MoveMembers.IsEnabled = UseVad.IsEnabled = true;
            }

            //TODO: Permission Editor
            //Permissions perms = new Permissions();
            //perms(guildId, new List<Role>() { role }.AsEnumerable());

            //RoleName.Text = role.Name;

            //Hoist.IsOn = role.Hoist;
            //AllowMention.IsOn = role.Mentionable;

            //Administrator.IsOn = perms.Perms.Administrator;
            //ViewAuditLog.IsOn = perms.Perms.ViewAuditLog;
            //ManageServer.IsOn = perms.Perms.ManangeGuild;
            //ManageRoles.IsOn = perms.Perms.ManageRoles;
            //ManageChannels.IsOn = perms.Perms.ManageChannels;
            //KickMembers.IsOn = perms.Perms.KickMembers;
            //BanMembers.IsOn = perms.Perms.BanMembers;
            //CreateInstantInvite.IsOn = perms.Perms.CreateInstantInvite;
            //ChangeNickname.IsOn = perms.Perms.ChangeNickname;
            //ManageNicknames.IsOn = perms.Perms.ManageNicknames;
            //ManageEmojis.IsOn = perms.Perms.ManageEmojis;
            //ManageWebhooks.IsOn = perms.Perms.ManageWebhooks;

            //ReadMessages.IsOn = perms.Perms.ReadMessages;
            //SendMessages.IsOn = perms.Perms.SendMessages;
            //SendTtsMessages.IsOn = perms.Perms.SendTtsMessages;
            //ManageMessages.IsOn = perms.Perms.ManageMessages;
            //EmbedLinks.IsOn = perms.Perms.EmbedLinks;
            //AttachFiles.IsOn = perms.Perms.AttachFiles;
            //ReadMessageHistory.IsOn = perms.Perms.ReadMessageHistory;
            //MentionEveryone.IsOn = perms.Perms.MentionEveryone;
            //UseExternalEmojis.IsOn = perms.Perms.UseExternalEmojis;
            //AddReactions.IsOn = perms.Perms.AddReactions;

            //ConnectPerm.IsOn = perms.Perms.Connect;
            //Speak.IsOn = perms.Perms.Speak;
            //MuteMembers.IsOn = perms.Perms.MuteMembers;
            //DeafenMembers.IsOn = perms.Perms.DeafenMembers;
            //MoveMembers.IsOn = perms.Perms.MoveMembers;
            //UseVad.IsOn = perms.Perms.UseVad;
            //loadingRoles = false;
        }

        class SimpleRole : INotifyPropertyChanged 
        {
            public SimpleRole(string id, string name, SolidColorBrush color)
            {
                Id = id;
                Name = name;
                Color = color;
            }

            private string _id;
            public string Id
            {
                get { return _id; }
                set { if (_id == value) return; _id = value; OnPropertyChanged("Id"); }
            }

            private string _name;
            public string Name
            {
                get { return _name; }
                set { if (_name == value) return; _name = value; OnPropertyChanged("Name"); }
            }

            private SolidColorBrush _color;
            public SolidColorBrush Color
            {
                get { return _color; }
                set { if (_color == value) return; _color = value; OnPropertyChanged("Color"); }
            }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        }

        private void UpdateRole(object sender, RoutedEventArgs e)
        {
            SaveRoleSettings();
        }
    }
}
