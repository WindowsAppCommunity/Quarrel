using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Popups;
using System.ComponentModel;
using Windows.Security.Credentials;

using Quarrel.LocalModels;
using DiscordAPI.SharedModels;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using Quarrel.Classes;

namespace Quarrel
{
    public class Storage
    {
        public static Dictionary<string,byte[]> EncryptionKeys = new Dictionary<string, byte[]>();

        private static Dictionary<string, string> nrs2;

        private static bool deferralling = false; //"Deferalling", I'm basically shakespeare, right?
        public static void UpdateNotificationState(string id, string timestamp)
        {
            //return;
            if (!deferralling)
                UNSdeferralStart();
            
            if (nrs2.ContainsKey(id))
                nrs2[id] = timestamp;
            else
                nrs2.Add(id, timestamp);

            if (!deferralling)
                UNSdeferralEnd();
        }
        public static void UNSclear()
        {
           // return;
            nrs2 = new Dictionary<string, string>();
        }
        public static void UNSdeferralStart()
        {
           // return;
            deferralling = false;
            var ls = ApplicationData.Current.LocalSettings.Values;
            if (!ls.ContainsKey("NotificationStates"))
                ls.Add("NotificationStates", "{}");
            var nrs = ls["NotificationStates"];
            nrs2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(nrs.ToString());
        }
        public static void UNSdeferralEnd()
        {
            deferralling = false;
            ApplicationData.Current.LocalSettings.Values["NotificationStates"] = JsonConvert.SerializeObject(nrs2);
        }

        public static event EventHandler SettingsChangedHandler;

        public static void SettingsChanged()
        {
            SettingsChangedHandler?.Invoke(typeof(Storage), EventArgs.Empty);
        }

        public static void SaveAppSettings()
        {
            Task.Run(() =>
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                if (!SavedSettings.Containers.ContainsKey("settings"))
                {
                    SavedSettings.CreateContainer("settings", ApplicationDataCreateDisposition.Always);
                }
                StringWriter settingsWriter = new StringWriter();
                serializer.Serialize(settingsWriter, Settings);
                SavedSettings.Values["settings"] = settingsWriter.ToString();
            });
        }

        public static Settings Settings = new Settings();
        public static ApplicationDataContainer SavedSettings = ApplicationData.Current.LocalSettings;
        public static PasswordVault PasswordVault = new PasswordVault();
    }


    public enum Theme { Dark, Light, Windows, Discord }
    public enum CollapseOverride { None, Mention, Unread }
    public class Settings
    {
        //Tuple<string, User> ActiveUser;
        public int BackgroundTaskTime = 9;
        public bool LockChannels = false;
        public bool AutoHideChannels = true;
        public bool AutoHidePeople = false;
        public bool HighlightEveryone = true;
        public bool FriendsNotifyDMs = true;
        public bool FriendsNotifyFriendRequest = false;
        public bool GlowOnMention = true;
        public bool SoundNotifications = true;
        public bool DiscordSounds = true;
        //public bool FriendsNotifyIncoming = true;
        //public bool FriendsNotifyOutgoing = false;
        public bool Toasts = false;
        public bool Badge = true;
        public bool LiveTile = true;
        public double RespUiM = 400;
        public double RespUiL = 720;
        public double RespUiXl = 1000;
        public bool CustomBG = false;
        public string BGFilePath = "";
        public double MainOpacity = 0.9;
        public double SecondaryOpacity = 0.6;
        public double TertiaryOpacity = 0.4;
        public double CmdOpacity = 0.7;
        public bool ExpensiveRender = false;
        public bool OLED = false;
        //public bool DropShadowPresence = false;
        public bool ShowOfflineMembers = false;
        public bool DevMode = false;
        public bool DiscordLightTheme = false;
        public bool CompactMode = false;
        public Theme Theme = Theme.Dark;
        public bool AccentBrush = false; //If false use Blurple if true use System.AccentColor
        public bool DerivedColor = false;
        public bool Vibrate = false;
        public bool mutedChnEffectServer = false;
        public bool ShowWelcomeMessage = true;
        public bool ShowNoPermissionChannels = false;
        public bool HideMutedChannels = false;
        public bool Acrylics = false;
        //public bool VoiceChannels = false;
        public bool UseCompression = true;
        public bool RichPresence = true;
        public bool Scaling = false;
        //public bool VideoAd = false;
        //public bool GifsOnHover = true;
        //public bool AutoChannelSelect = !App.IsMobile;
        public string DateFormat = "M/d/yyyy";
        public string TimeFormat = "hh:mm";
        public string DefaultAccount = "";
        public string OutputDevice = "Default";
        public string InputDevice = "Default";
        public int MSGFontSize = 14;
        public bool ServerMuteIcons = true;
        public CollapseOverride collapseOverride = CollapseOverride.Unread;
        //public Dictionary<string, Message> savedMessages = new Dictionary<string, Message>();
        public int NoiseSensitivity = -40;
        public SerializableDictionary<string, string> SelectedChannels = new SerializableDictionary<string, string>();
        public bool BackgroundVoice = !App.IsMobile;
        public int StandardData = 0;
        public int MobileData = 1;

        #region NotificationSounds
        public bool MessageSound = true;
        public bool VoiceDCSound = true;
        public bool UserJoinSound = true;
        public bool UserLeaveSound = true;
        #endregion
    }

    [Flags]
    public enum DataSettings
    {
        TTL = 0x1,
        SmallIcons = 0x2
    }

    public class NetworkSettings
    {
        private DataSettings settings;

        private bool Get(DataSettings setting)
        {
            return (settings & setting) == setting;
        }

        private void Set(DataSettings setting, bool value)
        {
            if (value)
            {
                Add(setting);
            }
            else
            {
                Remove(setting);
            }
        }

        private void Add(DataSettings add)
        {
            settings |= add;
        }

        private void Remove(DataSettings del)
        {
            settings &= ~del;
        }

        public bool TTL
        {
            get => Get(DataSettings.TTL);
            set => Set(DataSettings.TTL, value);
        }

        private static int GetSettings()
        {
            var temp = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (temp.IsWwanConnectionProfile)
            {
                return Storage.Settings.MobileData;
            }
            else
            {
                return Storage.Settings.StandardData;
            }
        }

        public static bool GetTTL()
        {
            int settings = GetSettings();
            return (settings & (int)DataSettings.TTL) == (int)DataSettings.TTL;
        }

        public bool SmallIcons
        {
            get => Get(DataSettings.SmallIcons);
            set => Set(DataSettings.SmallIcons, value);
        }

        public static bool GetSmallIcons()
        {
            int settings = GetSettings();
            return (settings & (int)DataSettings.SmallIcons) == (int)DataSettings.SmallIcons;
        }

        public NetworkSettings(int settings)
        {
            this.settings = (DataSettings)settings;
        }

        public int GetInt()
        {
            return (int)settings;
        }
    }
}
