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
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.UI.Popups;
using DiscordAPI.API.Guild.Models;
using DiscordAPI.API.Gateway;
using Quarrel.LocalModels;
using Quarrel.Managers;
using DiscordAPI.SharedModels;
using DiscordAPI.API.Gateway;
using DiscordAPI.API.Gateway.DownstreamEvents;

using Guild = DiscordAPI.SharedModels.Guild;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditGuild : Page
    {
        public EditGuild()
        {
            this.InitializeComponent();
            Invites.Visibility = Visibility.Visible;
            Bans.Visibility = Visibility.Visible;
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private int GetVfLvl()
        {
            if (vfLvl0.IsChecked == true)
            {
                return 0;
            } else if (vfLvl1.IsChecked == true)
            {
                return 1;
            } else if (vfLvl2.IsChecked == true)
            {
                return 2;
            } else if (vfLvl3.IsChecked == true)
            {
                return 3;
            } else
            {
                return 4;
            }
        }

        private int GetECFLvl()
        {
            if (ecfLvl0.IsChecked == true)
            {
                return 0;
            }
            else if (ecfLvl1.IsChecked == true)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private async void SaveGuildSettings(object sender, RoutedEventArgs e)
        {
            saveBTNtext.Opacity = 0;
            SaveButton.IsEnabled = false;
            saveBTNprog.Visibility = Visibility.Visible;
            try
            {
                DiscordAPI.API.Guild.Models.ModifyGuild modifyguild;
                if (string.IsNullOrEmpty(base64img))
                    modifyguild = new DiscordAPI.API.Guild.Models.ModifyGuild() { Name = GuildName.Text, AfkTimeout = LocalState.Guilds[guildId].Raw.AfkTimeout, VerificationLevel = GetVfLvl(), ExplicitContentFilter = GetECFLvl() };
                else
                    modifyguild = new DiscordAPI.API.Guild.Models.ModifyGuildIcon() { Name = GuildName.Text, Icon = base64img, AfkTimeout = LocalState.Guilds[guildId].Raw.AfkTimeout, VerificationLevel = GetVfLvl(), ExplicitContentFilter = GetECFLvl() };
                if (DeletedImage)
                    modifyguild = new DiscordAPI.API.Guild.Models.ModifyGuildIcon() { Name = GuildName.Text, Icon = null, AfkTimeout = LocalState.Guilds[guildId].Raw.AfkTimeout, VerificationLevel = GetVfLvl(), ExplicitContentFilter = GetECFLvl() };
                await Task.Run(async () =>
                {
                    await RESTCalls.ModifyGuild(guildId, modifyguild);
                });

                CloseButton_Click(null, null);
            }
            catch (Exception)
            {
                MessageDialog md = new MessageDialog("Something went wrong, and we weren't able to delete the invite.",
                    "Sorry :/");
                saveBTNtext.Opacity = 1;
                SaveButton.IsEnabled = true;
                saveBTNprog.Visibility = Visibility.Collapsed;
            }

            var settings = LocalState.Settings;
            var modify = new DiscordAPI.API.User.Models.ModifyUserSettings(LocalState.Settings);
            if (AllowDMs.IsChecked == true && modify.RestrictedGuilds != null && modify.RestrictedGuilds.Contains(guildId))
            {
                var list = modify.RestrictedGuilds.ToList();
                list.Remove(guildId);
                modify.RestrictedGuilds = list.ToArray();
            } else if (modify.RestrictedGuilds != null && !modify.RestrictedGuilds.Contains(guildId))
            {
                var list = modify.RestrictedGuilds.ToList();
                list.Add(guildId);
                modify.RestrictedGuilds = list.ToArray();
            } else
            {
                modify.RestrictedGuilds = new string[] { guildId };
            }

            LocalState.Settings = await RESTCalls.ModifyUserSettings(modify);
        }

        private void SaveRoleSettings()
        {
            try
            {
                if (!loadingRoles)
                {
                    Permissions perms = new Permissions(LocalState.Guilds[guildId]
                        .roles[(RolesView.SelectedItem as SimpleRole).Id].Permissions);
                    perms.Administrator = Administrator.IsOn;
                    perms.ViewAuditLog = ViewAuditLog.IsOn;
                    perms.ManangeGuild = ManageServer.IsOn;
                    perms.ManageRoles = ManageRoles.IsOn;
                    perms.ManageChannels = ManageChannels.IsOn;
                    perms.KickMembers = KickMembers.IsOn;
                    perms.BanMembers = BanMembers.IsOn;
                    perms.CreateInstantInvite = CreateInstantInvite.IsOn;
                    perms.ChangeNickname = ChangeNickname.IsOn;
                    perms.ManageNicknames = ManageNicknames.IsOn;
                    perms.ManageEmojis = ManageEmojis.IsOn;
                    perms.ManageWebhooks = ManageWebhooks.IsOn;

                    perms.ReadMessages = ReadMessages.IsOn;
                    perms.SendMessages = SendMessages.IsOn;
                    perms.SendTtsMessages = SendTtsMessages.IsOn;
                    perms.ManageMessages = ManageMessages.IsOn;
                    perms.EmbedLinks = EmbedLinks.IsOn;
                    perms.AttachFiles = AttachFiles.IsOn;
                    perms.ReadMessageHistory = ReadMessageHistory.IsOn;
                    perms.MentionEveryone = MentionEveryone.IsOn;
                    perms.UseExternalEmojis = UseExternalEmojis.IsOn;
                    perms.AddReactions = AddReactions.IsOn;

                    perms.Connect = ConnectPerm.IsOn;
                    perms.Speak = Speak.IsOn;
                    perms.MuteMembers = MuteMembers.IsOn;
                    perms.DeafenMembers = DeafenMembers.IsOn;
                    perms.MoveMembers = MoveMembers.IsOn;
                    perms.UseVad = UseVad.IsOn;
                    string roleId = (RolesView.SelectedItem as SimpleRole).Id;
                    ModifyGuildRole modifyguildrole =
                        new ModifyGuildRole()
                        {
                            Name = RoleName.Text,
                            Color = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id].Color,
                            Hoist = Hoist.IsOn,
                            Permissions = perms.GetPermInt(),
                            Position = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id]
                                .Position
                        };

                    Task.Run(async () =>
                    {
                        await RESTCalls.ModifyGuildRole(guildId, roleId, modifyguildrole);
                    });
                }
            }
            catch (Exception)
            {
                MessageDialog md = new MessageDialog("Something went wrong, and we weren't able to modify the role.",
                    "Sorry :/");
            }

        }

        string initialiconURL = null;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            guildId = e.Parameter.ToString();
            
            var guild = LocalState.Guilds[guildId];
            GuildName.Text = guild.Raw.Name;
            if (string.IsNullOrEmpty(guild.Raw.Icon))
                deleteImage.Visibility = Visibility.Collapsed;
            else
                GuildIcon.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/icons/" + guild.Raw.Id + "/" + guild.Raw.Icon + ".png"));

            header.Text = App.GetString("/Dialogs/EDIT") + " " + guild.Raw.Name.ToUpper();

            if (!LocalState.Guilds[guildId].permissions.ViewAuditLog)
            {
                AuditLogButton.IsEnabled = false;
            }

            if (!LocalState.Guilds[guildId].permissions.ManangeGuild && !LocalState.Guilds[guildId].permissions.Administrator && LocalState.Guilds[guildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                deleteImage.IsEnabled = false;
                uploadImage.IsEnabled = false;
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

            AllowDMs.IsChecked = LocalState.Settings.RestrictedGuilds == null || !LocalState.Settings.RestrictedGuilds.Contains(guildId);

            switch (guild.Raw.VerificationLevel)
            {
                case 0:
                    vfLvl0.IsChecked = true;
                    break;
                case 1:
                    vfLvl1.IsChecked = true;
                    break;
                case 2:
                    vfLvl2.IsChecked = true;
                    break;
                case 3:
                    vfLvl3.IsChecked = true;
                    break;
                case 4:
                    vfLvl4.IsChecked = true;
                    break;
            }

            switch (guild.Raw.ExplicitContentFilter)
            {
                case 0:
                    ecfLvl0.IsChecked = true;
                    break;
                case 1:
                    ecfLvl1.IsChecked = true;
                    break;
                case 2:
                    ecfLvl2.IsChecked = true;
                    break;
            }


            if (!guild.permissions.ManangeGuild)
            {
                ServerManagementSettings.Opacity = 0.5;
                ServerManagementSettings.IsHitTestVisible = false;
            }

            GatewayManager.Gateway.GuildUpdated += GuildUpdated;
            GatewayManager.Gateway.GuildBanAdded += BanAdded;
            GatewayManager.Gateway.GuildBanRemoved += BanRemoved;
        }

        private async void BanRemoved(object sender, GatewayEventArgs<GuildBanUpdate> e)
        {
            if (e.EventData.GuildId == guildId)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        for (int x = 0; x < BanView.Items.Count; x++)
                        {
                            if ((BanView.Items[x] as Ban).User.Id == e.EventData.User.Id)
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

        private async void BanAdded(object sender, GatewayEventArgs<GuildBanUpdate> e)
        {
            if (e.EventData.GuildId == guildId)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                       () =>
                       {
                           if (BanView.Items.Count == 0)
                           {
                               NoBans.Fade(0.0f, 200).Start();
                           }
                           BanView.Items.Insert(0, new Ban() { User = e.EventData.User, Reason = e.EventData.Reason, GuildId = guildId });
                       });
            }
        }

        private async void GuildUpdated(object sender, GatewayEventArgs<Guild> e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
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
            if ((pivot.SelectedItem as PivotItem) == Invites && !LoadingInvites)
            {
                InviteView.Items.Clear();
                NoInvites.Opacity = 0;
                LoadingInvite.Opacity = 1;
                LoadingInvites = true;
                IEnumerable<Invite> invites = null;
                await Task.Run(async () =>
                {
                    invites = await RESTCalls.GetChannelInvites(guildId);
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
            else if ((pivot.SelectedItem as PivotItem) == Bans && !loadingBans)
            {
                BanView.Items.Clear();
                NoBans.Opacity = 0;
                LoadingBans.Opacity = 1;
                loadingBans = true;
                IEnumerable<Ban> bans = null;
                await Task.Run(async () =>
                {
                    bans = await RESTCalls.GetGuildBans(guildId);
                });
                if (bans != null)
                {
                    foreach (var ban in bans)
                    {
                        ban.GuildId = guildId;
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
            try
            {
                await RESTCalls.DeleteInvite(code);
                InviteView.Items.Remove(InviteView.Items.FirstOrDefault(x => ((Invite) x).String == code));
            }
            catch (Exception)
            {
                MessageDialog md = new MessageDialog("Something went wrong, and we weren't able to delete the invite.",
                    "Sorry :/");
            }
        }

        private void RolesView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            loadingRoles = true;
            Role role = LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id];
            if ((role.Position >= LocalState.Guilds[guildId].GetHighestRole(LocalState.Guilds[guildId].members[LocalState.CurrentUser.Id].Roles).Position || (!LocalState.Guilds[guildId].permissions.ManageRoles && !LocalState.Guilds[guildId].permissions.Administrator)) && LocalState.Guilds[guildId].Raw.OwnerId != LocalState.CurrentUser.Id)
            {
                RoleName.IsEnabled = Hoist.IsEnabled = AllowMention.IsEnabled = Administrator.IsEnabled = ViewAuditLog.IsEnabled = ManageServer.IsEnabled = ManageRoles.IsEnabled = ManageChannels.IsEnabled = KickMembers.IsEnabled = BanMembers.IsEnabled = CreateInstantInvite.IsEnabled = ChangeNickname.IsEnabled = ManageNicknames.IsEnabled = ManageEmojis.IsEnabled = ManageWebhooks.IsEnabled = ReadMessages.IsEnabled = SendMessages.IsEnabled = SendTtsMessages.IsEnabled = ManageMessages.IsEnabled = EmbedLinks.IsEnabled = AttachFiles.IsEnabled = ReadMessageHistory.IsEnabled = MentionEveryone.IsEnabled = UseExternalEmojis.IsEnabled = AddReactions.IsEnabled = ConnectPerm.IsEnabled = Speak.IsEnabled = MuteMembers.IsEnabled = DeafenMembers.IsEnabled = MoveMembers.IsEnabled = UseVad.IsEnabled = false;
            }
            else
            {
                RoleName.IsEnabled = Hoist.IsEnabled = AllowMention.IsEnabled = Administrator.IsEnabled = ViewAuditLog.IsEnabled = ManageServer.IsEnabled = ManageRoles.IsEnabled = ManageChannels.IsEnabled = KickMembers.IsEnabled = BanMembers.IsEnabled = CreateInstantInvite.IsEnabled = ChangeNickname.IsEnabled = ManageNicknames.IsEnabled = ManageEmojis.IsEnabled = ManageWebhooks.IsEnabled = ReadMessages.IsEnabled = SendMessages.IsEnabled = SendTtsMessages.IsEnabled = ManageMessages.IsEnabled = EmbedLinks.IsEnabled = AttachFiles.IsEnabled = ReadMessageHistory.IsEnabled = MentionEveryone.IsEnabled = UseExternalEmojis.IsEnabled = AddReactions.IsEnabled = ConnectPerm.IsEnabled = Speak.IsEnabled = MuteMembers.IsEnabled = DeafenMembers.IsEnabled = MoveMembers.IsEnabled = UseVad.IsEnabled = true;
            }

            Permissions perms = new Permissions(Convert.ToInt32(LocalState.Guilds[guildId].roles[(RolesView.SelectedItem as SimpleRole).Id].Permissions));

            RoleName.Text = role.Name;

            Hoist.IsOn = role.Hoist;
            AllowMention.IsOn = role.Mentionable;

            Administrator.IsOn = perms.Administrator;
            ViewAuditLog.IsOn = perms.ViewAuditLog;
            ManageServer.IsOn = perms.ManangeGuild;
            ManageRoles.IsOn = perms.ManageRoles;
            ManageChannels.IsOn = perms.ManageChannels;
            KickMembers.IsOn = perms.KickMembers;
            BanMembers.IsOn = perms.BanMembers;
            CreateInstantInvite.IsOn = perms.CreateInstantInvite;
            ChangeNickname.IsOn = perms.ChangeNickname;
            ManageNicknames.IsOn = perms.ManageNicknames;
            ManageEmojis.IsOn = perms.ManageEmojis;
            ManageWebhooks.IsOn = perms.ManageWebhooks;

            ReadMessages.IsOn = perms.ReadMessages;
            SendMessages.IsOn = perms.SendMessages;
            SendTtsMessages.IsOn = perms.SendTtsMessages;
            ManageMessages.IsOn = perms.ManageMessages;
            EmbedLinks.IsOn = perms.EmbedLinks;
            AttachFiles.IsOn = perms.AttachFiles;
            ReadMessageHistory.IsOn = perms.ReadMessageHistory;
            MentionEveryone.IsOn = perms.MentionEveryone;
            UseExternalEmojis.IsOn = perms.UseExternalEmojis;
            AddReactions.IsOn = perms.AddReactions;

            ConnectPerm.IsOn = perms.Connect;
            Speak.IsOn = perms.Speak;
            MuteMembers.IsOn = perms.MuteMembers;
            DeafenMembers.IsOn = perms.DeafenMembers;
            MoveMembers.IsOn = perms.MoveMembers;
            UseVad.IsOn = perms.UseVad;
            loadingRoles = false;
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

        private string guildId = "";

        private void GuildIcon_ImageOpened(object sender, RoutedEventArgs e)
        {
            GuildIconRect.Opacity = 0;
            GuildIconRect.Fade(1, 300).Start();
        }
        string base64img = "";
        bool DeletedImage = false;
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {

                    string uri = file.Path;

                    base64img = "data:" + file.ContentType + ";base64,";
                    // var tempfile = await RescaleImage(file, 128, 128);
                    base64img += Convert.ToBase64String(await filetobytes(file));
                    BitmapImage img = new BitmapImage();
                    GuildIconRect.Opacity = 0;
                    using (var fileStream = await file.OpenStreamForReadAsync())
                    {
                        await img.SetSourceAsync(fileStream.AsRandomAccessStream());
                    }

                    GuildIcon.ImageSource = img;
                    GuildIconRect.Fade(1, 300).Start();
                    deleteImage.Content = App.GetString("/Dialogs/CancelIconMod");
                    deleteImage.Visibility = Visibility.Visible;
                }
                catch { }
            }
        }

        private async Task<StorageFile> RescaleImage(StorageFile sourceFile, uint width, uint height)
        {
            var imageStream = await sourceFile.OpenReadAsync();
            var decoder = await BitmapDecoder.CreateAsync(imageStream);
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            StorageFile tempfile = await folder.CreateFileAsync("icon.png", CreationCollisionOption.OpenIfExists);
            using (var resizedStream = await tempfile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;
                encoder.BitmapTransform.ScaledWidth = width;
                encoder.BitmapTransform.ScaledHeight = height;
                await encoder.FlushAsync();
            }
            return tempfile;
        }
        private async Task<byte[]> filetobytes(Windows.Storage.StorageFile file)
        {
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
        private void deleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (deleteImage.Content.ToString() == App.GetString("/Dialogs/CancelIconMod"))
            {
                if (string.IsNullOrEmpty(LocalState.Guilds[guildId].Raw.Icon))
                    GuildIcon.ImageSource = null;
                else
                    GuildIcon.ImageSource = new BitmapImage(new Uri("https://cdn.discordapp.com/icons/" + LocalState.Guilds[guildId].Raw.Id + "/" + LocalState.Guilds[guildId].Raw.Icon + ".png"));
                base64img = null;
                deleteImage.Content = App.GetString("/Dialogs/Delete");
                DeletedImage = false;
            }
            else
            {
                DeletedImage = true;
                GuildIcon.ImageSource = null;
                deleteImage.Content = App.GetString("/Dialogs/CancelIconMod");
            }
        }

        private void NavigateToAudit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubPages.AuditLog), guildId);
        }

        private void GuildIconRect_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void GuildIconRect_Holding(object sender, HoldingRoutedEventArgs e)
        {

        }
    }
}
