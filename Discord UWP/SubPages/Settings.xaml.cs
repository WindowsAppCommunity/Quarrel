using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Popups;
using Quarrel.Managers;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Quarrel.SubPages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
            App.SubpageCloseHandler += App_SubpageCloseHandler;
        }

        NetworkSettings standardNetwork;
        NetworkSettings mobileNetwork;


        private void App_SubpageCloseHandler(object sender, EventArgs e)
        {
            CloseButton_Click(null, null);
            App.SubpageCloseHandler -= App_SubpageCloseHandler;
        }

        private async void LoadSettings()
        {
            //TODO: Settings
            HighlightEveryone.IsChecked = Storage.Settings.HighlightEveryone;
            //Toasts.IsChecked = Storage.Settings.Toasts;
            LiveTile.IsChecked = Storage.Settings.LiveTile;
            Badge.IsChecked = Storage.Settings.Badge;
            Vibrate.IsChecked = Storage.Settings.Vibrate;
            FontSizeSlider.Value = Storage.Settings.MSGFontSize;
            CompactMode.IsChecked = Storage.Settings.CompactMode;
            FriendsNotifyDMs.IsChecked = Storage.Settings.FriendsNotifyDMs;
            FriendsNotifyFriendRequests.IsChecked = Storage.Settings.FriendsNotifyFriendRequest;
            //Storage.Settings.FriendsNotifyIncoming = (bool)FriendsNotifyIncomingFriendRequests.IsChecked;
            //Storage.Settings.FriendsNotifyOutgoing = (bool)FriendsNotifyOutgoingFriendRequests.IsChecked;
            RespUI_M.Value = Storage.Settings.RespUiM;
            RespUI_L.Value = Storage.Settings.RespUiL;
            RespUI_XL.Value = Storage.Settings.RespUiXl;
            //AppBarAtBottom_checkbox.IsChecked = Storage.Settings.AppBarAtBottom;
            ShowWelcome.IsChecked = Storage.Settings.ShowWelcomeMessage;
            ShowNoPermissionsChannels.IsChecked = Storage.Settings.ShowNoPermissionChannels;
            HideMutedChannels.IsChecked = Storage.Settings.HideMutedChannels;
            EnableAcrylic.IsChecked = Storage.Settings.Acrylics;
            EnableBackgroundVoice.IsChecked = Storage.Settings.BackgroundVoice;
            ExpensiveUI.IsChecked = Storage.Settings.ExpensiveRender;
            //DropShadowPresence.IsChecked = Storage.Settings.DropShadowPresence;
            UseCompression.IsChecked = Storage.Settings.UseCompression;
            RichPresence.IsChecked = Storage.Settings.RichPresence;
            if (App.IsXbox)
            {
                Scaling.Visibility = Visibility.Visible;
                Scaling.IsChecked = Storage.Settings.Scaling;
            }
            //VoiceChannels.IsChecked = Storage.Settings.VoiceChannels;
            //GifsOnHover.IsChecked = Storage.Settings.GifsOnHover;

            //NotificationSounds.IsChecked = Storage.Settings.SoundNotifications;
            //if (Storage.Settings.DiscordSounds)
            //{
            //    radio_DiscordSounds.IsChecked = true;
            //}
            //else
            //{
            //    radio_WindowsSounds.IsChecked = true;
            //}

            MentionGlow.IsChecked = Storage.Settings.GlowOnMention;
            ShowServerMute.IsChecked = Storage.Settings.ServerMuteIcons;

            MessageNotification.IsChecked = Storage.Settings.MessageSound;
            VoiceDCNotification.IsChecked = Storage.Settings.VoiceDCSound;
            UserJoinNotification.IsChecked = Storage.Settings.UserJoinSound;
            UserLeaveNotification.IsChecked = Storage.Settings.UserLeaveSound;

            if (Storage.Settings.BackgroundTaskTime == 0)
            {
                bgEnabler.IsOn = false;
                timeSlider.IsEnabled = false;
                timeSlider.Value = 9;
            }
            else
            {
                bgEnabler.IsOn = true;
                timeSlider.IsEnabled = true;
                timeSlider.Value = Storage.Settings.BackgroundTaskTime;
            }
            bgEnabler_Toggled(null, null);

            bgNotifyFriend.IsChecked = GetSetting("bgNotifyFriend");
            bgNotifyDM.IsChecked = GetSetting("bgNotifyDM");
            bgNotifyMention.IsChecked = bgNotifyMutedMention.IsEnabled = GetSetting("bgNotifyMention");
            bgNotifyMutedMention.IsChecked = GetSetting("bgNotifyMutedMention");

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("bgTaskLastrunStatus"))
            {
                string lastrunstatus = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["bgTaskLastrunStatus"];
                if (string.IsNullOrWhiteSpace(lastrunstatus))
                {
                    bgLastRuntimeStatus.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bgLastRuntimeStatus.Visibility = Visibility.Visible;
                    bgLastRuntimeStatus.Text = lastrunstatus;
                }
            }
            else
                bgLastRuntimeStatus.Visibility = Visibility.Collapsed;

            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("bgTaskLastrun"))
            {
                if (bgLastRuntimeStatus.Visibility == Visibility.Collapsed)
                    bgLastRuntime.Text = App.GetString("/Settings/BGTaskNoRun");
                else
                    bgLastRuntime.Text = App.GetString("/Settings/BGTaskNoSuccessfulRun");
            }
            else
            {
                var lastrun = Windows.Storage.ApplicationData.Current.LocalSettings.Values["bgTaskLastrun"];
                var status = GetSettingString("bgTaskLastrunStatus");

                if (lastrun.GetType() == typeof(long))
                {
                    var time = DateTimeOffset.FromUnixTimeSeconds((long)lastrun).ToLocalTime();
                    bgLastRuntime.Text = "The background task last ran succesfully ";
                    if (time.Date == DateTime.Now.Date)
                        bgLastRuntime.Text += "today at ";
                    else if (time.Date == DateTime.Now.Date.AddDays(-1))
                        bgLastRuntime.Text += "yesterday at ";
                    else
                        bgLastRuntime.Text += "on " + time.ToString("MM/dd/yyyy") + " at ";
                    bgLastRuntime.Text += time.ToString("HH:mm");
                }
            }

            if (Storage.Settings.AccentBrush)
                radioAccent_Windows.IsChecked = true;
            else
                radioAccent_Discord.IsChecked = true;

            DerviedColor.IsChecked = Storage.Settings.DerivedColor;


            switch (Storage.Settings.TimeFormat)
            {
                case "hh:mm":
                    TimeFormat.SelectedIndex = 0;
                    break;
                case "H:mm":
                    TimeFormat.SelectedIndex = 1;
                    break;
                case "hh:mm:ss tt":
                    TimeFormat.SelectedIndex = 2;
                    break;
                case "HH:mm:ss":
                    TimeFormat.SelectedIndex = 3;
                    break;
                default:
                    TimeFormat.SelectedIndex = 4;
                    CustomTimeF.Text = Storage.Settings.TimeFormat;
                    break;
            }

            if (TimeFormat.SelectedIndex == 4)
            {
                CustomTimeF.Visibility = Visibility.Visible;
            }
            else
            {
                CustomTimeF.Visibility = Visibility.Collapsed;
            }

            switch (Storage.Settings.DateFormat)
            {
                case "M/d/yyyy":
                    DateFormat.SelectedIndex = 0;
                    break;
                case "M/d/yy":
                    DateFormat.SelectedIndex = 1;
                    break;
                case "MM/dd/yy":
                    DateFormat.SelectedIndex = 2;
                    break;
                case "MMM/dd/yy":
                    DateFormat.SelectedIndex = 3;
                    break;
                default:
                    DateFormat.SelectedIndex = 4;
                    CustomDateF.Text = Storage.Settings.DateFormat;
                    break;
            }

            if (DateFormat.SelectedIndex == 4)
            {
                CustomDateF.Visibility = Visibility.Visible;
            }
            else
            {
                CustomDateF.Visibility = Visibility.Collapsed;
            }

            if (!(OLED.IsChecked = Storage.Settings.OLED).Value)
            {
                if (Storage.Settings.Theme == Theme.Dark)
                    radio_Dark.IsChecked = true;
                else if (Storage.Settings.Theme == Theme.Light)
                    radio_Light.IsChecked = true;
                else if (Storage.Settings.Theme == Theme.Windows)
                    radio_Windows.IsChecked = true;
                else if (Storage.Settings.Theme == Theme.Discord)
                    radio_Discord.IsChecked = true;
            }

            //if (!Storage.Settings.OLED)
            //{
            //    if (Storage.Settings.Theme == Theme.Dark)
            //        radio_Dark.IsChecked = true;
            //    else if (Storage.Settings.Theme == Theme.Light)
            //        radio_Light.IsChecked = true;
            //    else if (Storage.Settings.Theme == Theme.Windows)
            //        radio_Windows.IsChecked = true;
            //    else if (Storage.Settings.Theme == Theme.Discord)
            //        radio_Discord.IsChecked = true;
            //}

            //OLED.IsChecked = Storage.Settings.OLED;


            if (Storage.Settings.collapseOverride == CollapseOverride.None)
                NoOverride.IsChecked = true;
            else if (Storage.Settings.collapseOverride == CollapseOverride.Mention)
                OverrideMention.IsChecked = true;
            else if (Storage.Settings.collapseOverride == CollapseOverride.Unread)
                OverrideUnread.IsChecked = true;

            NoiseSensitivity.Value = Storage.Settings.NoiseSensitivity;

            //MainPanelBlur.Value = Storage.Settings.MainOpacity;
            //SecondaryPanelBlur.Value = Storage.Settings.SecondaryOpacity;
            //TertiaryPanelBlur.Value = Storage.Settings.TertiaryOpacity;
            //CommandBarBlur.Value = Storage.Settings.CmdOpacity;

            //CustomBGToggle.IsOn = Storage.Settings.CustomBG;
            //FilePath.Text = Storage.Settings.BGFilePath;


            //Output Devices
            OutputDevices.Items.Clear();
            var odevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioRender);
            OutputDevices.Items.Add(new ComboBoxItem() { Content = "Default", Tag = "Default" });
            OutputDevices.SelectedIndex = 0;
            int i = 1;
            foreach (var device in odevices)
            {
                OutputDevices.Items.Add(new ComboBoxItem() { Content = device.Name, Tag = device.Id, IsEnabled = device.IsEnabled });
                if (device.Id == Storage.Settings.OutputDevice)
                {
                    OutputDevices.SelectedIndex = i;
                }
                i++;
            }

            //Input Devices
            InputDevices.Items.Clear();
            var idevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioCapture);
            InputDevices.Items.Add(new ComboBoxItem() { Content = "Default", Tag = "Default" });
            InputDevices.SelectedIndex = 0;
            i = 1;
            foreach (var device in idevices)
            {
                InputDevices.Items.Add(new ComboBoxItem() { Content = device.Name, Tag = device.Id, IsEnabled = device.IsEnabled });
                if (device.Id == Storage.Settings.OutputDevice)
                {
                    InputDevices.SelectedIndex = i;
                }
                i++;
            }


            foreach (var language in ApplicationLanguages.ManifestLanguages)
            {
                var lang = new Windows.Globalization.Language(language);
                ComboBoxItem item = new ComboBoxItem();
                item.Content = UppercaseFirst(lang.NativeName);
                if (lang.NativeName != lang.DisplayName) item.Content += " (" + lang.DisplayName + ")";
                item.Tag = language;
                LanguageSelection.Items.Add(item);
            }

            if (string.IsNullOrWhiteSpace(ApplicationLanguages.PrimaryLanguageOverride))
            {
                LanguageSelection.SelectedIndex = 0;
            }
            else
            {
                foreach (var item in LanguageSelection.Items)
                {
                    if (((ComboBoxItem)item).Tag.ToString() == ApplicationLanguages.PrimaryLanguageOverride)
                    {
                        LanguageSelection.SelectedItem = item;
                        break;
                    }
                }
            }

            if (LanguageSelection.SelectedIndex == -1)
                LanguageSelection.SelectedIndex = 0;

            standardNetwork = new NetworkSettings(Storage.Settings.StandardData);
            mobileNetwork = new NetworkSettings(Storage.Settings.MobileData);

            TTLAttachments.IsChecked = standardNetwork.TTL;
            HideSIcons.IsChecked = standardNetwork.SmallIcons;
            TTLAttachmentsMD.IsChecked = mobileNetwork.TTL;
            HideSIconsMD.IsChecked = mobileNetwork.SmallIcons;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DerviedColor.Foreground = App.Current.RequestedTheme == ApplicationTheme.Dark ? DarkThemeAccentGradient : LightThemeAccentGradient;

            LoadSettings();
            
            if (await AudioManager.CreateInputDeviceNode(Storage.Settings.InputDevice))
            {
                AudioManager.InputRecieved += AudioManager_InputRecieved;
            } else
            {
                NoiseSensitivity.IsEnabled = false;
                InputDevices.IsEnabled = false;
                NoInputMessage.Visibility = Visibility.Visible;
            }

        }

        private async void AudioManager_InputRecieved(object sender, float[] e)
        {
            double decibels = 0f;
            foreach (var sample in e)
            {
                decibels += Math.Abs(sample);
            }
            decibels = 20 * Math.Log10(decibels / e.Length);
            if (!(Double.IsInfinity(decibels) || Double.IsNaN(decibels)))
            {
                await App.dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { NoiseVisual.Value = decibels; });
            }
        }

        private void ChangeSetting(string name, bool value)
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(name))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(name, value);
            else
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[name] = value;
        }
        private bool GetSetting(string name)
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(name))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(name, true);
            return (bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values[name];
        }
        private string GetSettingString(string name)
        {
            if (!Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(name))
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Add(name, "");
            return (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values[name];
        }
        private void rootgrid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private async void SaveUserSettings(object sender, RoutedEventArgs e)
        {
            //TODO: Settings
            Storage.Settings.HighlightEveryone = (bool)HighlightEveryone.IsChecked;
            //Storage.Settings.Toasts = (bool)Toasts.IsChecked;
            Storage.Settings.LiveTile = (bool)LiveTile.IsChecked;
            Storage.Settings.Badge = (bool)Badge.IsChecked;
            Storage.Settings.Vibrate = (bool)Vibrate.IsChecked;
            Storage.Settings.RespUiM = RespUI_M.Value;
            Storage.Settings.RespUiL = RespUI_L.Value;
            Storage.Settings.RespUiXl = RespUI_XL.Value;
            //Storage.Settings.AppBarAtBottom = (bool)AppBarAtBottom_checkbox.IsChecked;
            Storage.Settings.MSGFontSize = (int)FontSizeSlider.Value;
            Storage.Settings.CompactMode = (bool)CompactMode.IsChecked;
            Storage.Settings.FriendsNotifyDMs = (bool)FriendsNotifyDMs.IsChecked;
            Storage.Settings.FriendsNotifyFriendRequest = (bool)FriendsNotifyFriendRequests.IsChecked;
            //Storage.Settings.FriendsNotifyIncoming = (bool)FriendsNotifyIncomingFriendRequests.IsChecked;
            //Storage.Settings.FriendsNotifyOutgoing = (bool)FriendsNotifyOutgoingFriendRequests.IsChecked;
            
            Storage.Settings.AccentBrush = (bool)radioAccent_Windows.IsChecked;
            Storage.Settings.DerivedColor = (bool)DerviedColor.IsChecked;
            Storage.Settings.Acrylics = (bool)EnableAcrylic.IsChecked;
            Storage.Settings.BackgroundVoice = (bool)EnableBackgroundVoice.IsChecked;
            Storage.Settings.ExpensiveRender = (bool)ExpensiveUI.IsChecked;
            //Storage.Settings.DropShadowPresence = (bool)DropShadowPresence.IsChecked;
            Storage.Settings.ShowWelcomeMessage = (bool)ShowWelcome.IsChecked;
            Storage.Settings.ShowNoPermissionChannels = (bool)ShowNoPermissionsChannels.IsChecked;
            Storage.Settings.HideMutedChannels = (bool)HideMutedChannels.IsChecked;
            Storage.Settings.UseCompression = (bool)UseCompression.IsChecked;
            Storage.Settings.RichPresence = (bool)RichPresence.IsChecked;
            Storage.Settings.Scaling = (bool)Scaling.IsChecked;
            Storage.Settings.OLED = (bool)OLED.IsChecked;
            //Storage.Settings.VoiceChannels = (bool)VoiceChannels.IsChecked;
            //Storage.Settings.GifsOnHover = (bool)GifsOnHover.IsChecked;
            Storage.Settings.ServerMuteIcons = (bool)ShowServerMute.IsChecked;
            Storage.Settings.GlowOnMention = (bool)MentionGlow.IsChecked;

            Storage.Settings.MessageSound = (bool)MessageNotification.IsChecked;
            Storage.Settings.VoiceDCSound = (bool)VoiceDCNotification.IsChecked;
            Storage.Settings.UserJoinSound = (bool)UserJoinNotification.IsChecked;
            Storage.Settings.UserLeaveSound = (bool)UserLeaveNotification.IsChecked;

            //Storage.Settings.SoundNotifications = (bool)NotificationSounds.IsChecked;
            //Storage.Settings.DiscordSounds = (bool)radio_DiscordSounds.IsChecked;

            ApplicationLanguages.PrimaryLanguageOverride = ((ComboBoxItem)LanguageSelection.SelectedItem).Tag.ToString().Trim();

            if (bgEnabler.IsOn)
            {
                Storage.Settings.BackgroundTaskTime = (int)timeSlider.Value;
            }   
            else
            {
                Storage.Settings.BackgroundTaskTime = 0;
            }
            await Managers.BackgroundTaskManager.UpdateNotificationBGTask();
            ChangeSetting("bgNotifyFriend", (bool)bgNotifyFriend.IsChecked);
            ChangeSetting("bgNotifyDM", (bool)bgNotifyDM.IsChecked);
            ChangeSetting("bgNotifyMention", (bool)bgNotifyMention.IsChecked);
            ChangeSetting("bgNotifyMutedMention", (bool)bgNotifyMutedMention.IsChecked);

            switch (TimeFormat.SelectedIndex)
            {
                case 0:
                    Storage.Settings.TimeFormat = "hh:mm";
                    break;
                case 1:
                    Storage.Settings.TimeFormat = "H:mm";
                    break;
                case 2:
                    Storage.Settings.TimeFormat = "hh:mm:ss tt";
                    break;
                case 3:
                    Storage.Settings.TimeFormat = "HH:mm:ss";
                    break;
                default:
                    Storage.Settings.TimeFormat = CustomTimeF.Text;
                    break;
            }
            switch (DateFormat.SelectedIndex)
            {
                case 0:
                    Storage.Settings.DateFormat = "M/d/yyyy";
                    break;
                case 1:
                    Storage.Settings.DateFormat = "M/d/yy";
                    break;
                case 2:
                    Storage.Settings.DateFormat = "MM/dd/yy";
                    break;
                case 3:
                    Storage.Settings.DateFormat = "MMM/dd/yy";
                    break;
                default:
                    Storage.Settings.DateFormat = CustomDateF.Text;
                    break;
            }

            if ((bool)radio_Dark.IsChecked)
                Storage.Settings.Theme = Theme.Dark;
            else if ((bool)radio_Light.IsChecked)
                Storage.Settings.Theme = Theme.Light;
            else if ((bool)radio_Windows.IsChecked)
                Storage.Settings.Theme = Theme.Windows;
            else if ((bool)radio_Discord.IsChecked)
                Storage.Settings.Theme = Theme.Discord;

            if ((bool)NoOverride.IsChecked)
                Storage.Settings.collapseOverride = CollapseOverride.None;
            else if ((bool)OverrideMention.IsChecked)
                Storage.Settings.collapseOverride = CollapseOverride.Mention;
            else if ((bool)OverrideUnread.IsChecked)
                Storage.Settings.collapseOverride = CollapseOverride.Unread;

            //Storage.Settings.MainOpacity = MainPanelBlur.Value;
            //Storage.Settings.SecondaryOpacity = SecondaryPanelBlur.Value;
            //Storage.Settings.TertiaryOpacity = TertiaryPanelBlur.Value;
            //Storage.Settings.CmdOpacity = CommandBarBlur.Value;

            //Storage.Settings.CustomBG = CustomBGToggle.IsOn;
            //Storage.Settings.BGFilePath = FilePath.Text;

            Storage.Settings.NoiseSensitivity = (int)NoiseSensitivity.Value;

            Storage.Settings.OutputDevice = (OutputDevices.SelectedItem as ComboBoxItem).Tag.ToString();
            Storage.Settings.InputDevice = (InputDevices.SelectedItem as ComboBoxItem).Tag.ToString();


            standardNetwork.TTL = (bool)TTLAttachments.IsChecked;
            standardNetwork.SmallIcons = (bool)HideSIcons.IsChecked;
            mobileNetwork.TTL = (bool)TTLAttachmentsMD.IsChecked;
            mobileNetwork.SmallIcons = (bool)HideSIconsMD.IsChecked;

            Storage.Settings.StandardData = standardNetwork.GetInt();
            Storage.Settings.MobileData = mobileNetwork.GetInt();


            Storage.SaveAppSettings();
            Storage.SettingsChanged();
            CloseButton_Click(null, null);
        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        bool _ignoreRespUiChanges = false;
        private void RespUI_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!_ignoreRespUiChanges)
            {
                if (RespUI_L.Value < RespUI_M.Value) RespUI_L.Value = RespUI_M.Value;
                if (RespUI_XL.Value < RespUI_L.Value) RespUI_XL.Value = RespUI_L.Value;
            }
        }
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            _ignoreRespUiChanges = true;
            
            RespUI_M.Value = 569;
            RespUI_L.Value = 768;
            RespUI_XL.Value = 1024;
            _ignoreRespUiChanges = false;
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
            scale.CenterY = this.ActualHeight / 2;
            scale.CenterX = this.ActualWidth / 2;
            NavAway.Begin();
            AudioManager.LightDisposeInGraph();
            AudioManager.InputRecieved -= AudioManager_InputRecieved;
            App.SubpageClosed();
        }

        private async void CheckLogout(object sender, RoutedEventArgs e)
        {
            MessageDialog winnerAnounce = new MessageDialog(App.GetString("/Settings/VerifyLogout"));
            winnerAnounce.Commands.Add(new UICommand(App.GetString("/Settings/Logout"),
            new UICommandInvokedHandler(ConfirmLogout)));
            winnerAnounce.Commands.Add(new UICommand(App.GetString("/Dialogs/Cancel"),
            new UICommandInvokedHandler(CancelLogout)));
            await winnerAnounce.ShowAsync();
        }
        private void CancelLogout(IUICommand command)
        {

        }
        private void ConfirmLogout(IUICommand command)
        {
            App.LogOut();
        }

        private async void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            await App.RequestReset();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubPages.UserProfileCU));
          //  App.NavigateToProfile(LocalModels.LocalState.CurrentUser);
        }

        private void TimeFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomTimeF != null)
            {
                CustomTimeF.Visibility = Visibility.Collapsed;
                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0:
                        TimeExample.Text = (DateTime.Now.ToString("hh:mm"));
                        break;
                    case 1:
                        TimeExample.Text = (DateTime.Now.ToString("H:mm"));
                        break;
                    case 2:
                        TimeExample.Text = (DateTime.Now.ToString("hh:mm:ss tt"));
                        break;
                    case 3:
                        TimeExample.Text = (DateTime.Now.ToString("HH:mm:ss"));
                        break;
                    case 4:
                        CustomTimeF.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void DateFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomDateF != null)
            {
                CustomDateF.Visibility = Visibility.Collapsed;
                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0:
                        DateExample.Text = (DateTime.Now.ToString("M/d/yyyy"));
                        break;
                    case 1:
                        DateExample.Text = (DateTime.Now.ToString("M/d/yy"));
                        break;
                    case 2:
                        DateExample.Text = (DateTime.Now.ToString("MM/dd/yy"));
                        break;
                    case 3:
                        DateExample.Text = (DateTime.Now.ToString("MMM/dd/yyyy"));
                        break;
                    case 4:
                        CustomDateF.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private async void SelectFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //FilePath.Text = file.Path;
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            int minutes = Convert.ToInt32(timeSlider.Value * 5);
            string timeTxt = "";

            if (minutes > 60)
            {
                TimeSpan span = TimeSpan.FromMinutes(minutes);
                if (span.Days == 1)
                    timeTxt = "24h";
                else
                {
                    timeTxt = span.Hours + "h";
                    if (span.Minutes != 0)
                        timeTxt += span.Minutes + "min";
                }

            }
            else
            {
                timeTxt = minutes + "min";
            }
            sliderTime.Text = timeTxt;
        }

        private void bgEnabler_Toggled(object sender, RoutedEventArgs e)
        {
            if (bgEnabler.IsOn)
            {
                timeSlider.IsEnabled = true;
                bgNotifyDM.IsEnabled = true;
                bgNotifyFriend.IsEnabled = true;
                bgNotifyMention.IsEnabled = true;
                bgNotifyMutedMention.IsEnabled = true;
                RunEveryLabel.Opacity = 1;
                sliderTime.Opacity = 1;
                sliderTime.Foreground = (SolidColorBrush)Application.Current.Resources["Blurple"];
            }
            else
            {
                timeSlider.IsEnabled = false;
                bgNotifyDM.IsEnabled = false;
                bgNotifyFriend.IsEnabled = false;
                bgNotifyMention.IsEnabled = false;
                bgNotifyMutedMention.IsEnabled = false;
                timeSlider.Value = 9;
                RunEveryLabel.Opacity = 0.4;
                sliderTime.Opacity = 0.2;
                sliderTime.Foreground = (SolidColorBrush)Application.Current.Resources["InvertedBG"];
            }
        }

        private void PlaySound(object sender, RoutedEventArgs e)
        {
            //AudioManager.PlaySoundEffect((sender as Button).Tag.ToString(), /*(radio_DiscordSounds.IsChecked.Value) ? "discord" : "windows"*/ "quarrel");
            AudioManager.PlaySoundEffect((sender as Button).Tag.ToString());
        }

        private void BlurpleChecked(object sender, RoutedEventArgs e)
        {
            var blurple = Color.FromArgb(255, 114, 137, 218);
            App.Current.Resources["Blurple"] = new SolidColorBrush(blurple); //Set to Blurple default
            App.Current.Resources["BlurpleColor"] = blurple;

            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = blurple;
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = blurple;
        }

        private void AccentChecked(object sender, RoutedEventArgs e)
        {
            var blurple = (Color)Resources["SystemAccentColor"];
            App.Current.Resources["Blurple"] = new SolidColorBrush(blurple); //Set to Blurple default
            App.Current.Resources["BlurpleColor"] = blurple;

            ((App.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = (Color)Resources["SystemAccentColor"];
            ((App.Current.Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = (Color)Resources["SystemAccentColor"];
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            Storage.Settings = new Quarrel.Settings();
        }
    }
}
