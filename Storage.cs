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

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;
using Newtonsoft.Json;

namespace Discord_UWP
{
    public static class Storage
    {
        public static Dictionary<string,byte[]> EncryptionKeys = new Dictionary<string, byte[]>();
        public static void SaveEncryptionKeys()
        {
            Dictionary<string, string> SerializableEncryptionKeys = new Dictionary<string, string>();
            foreach(var key in EncryptionKeys)
            {
                SerializableEncryptionKeys.Add(key.Key, Convert.ToBase64String(key.Value));
            }
            string keys = JsonConvert.SerializeObject(SerializableEncryptionKeys);
            var eks = Storage.PasswordVault.RetrieveAll().Where(x => x.Resource == "encryptionKeys");
            foreach (var ek in eks)
                Storage.PasswordVault.Remove(ek);
            PasswordVault.Add(new PasswordCredential("encryptionKeys", "encryptionKeys", keys));
        }
        public static void RetrieveEncryptionKeys()
        {
            var ek = Storage.PasswordVault.RetrieveAll().FirstOrDefault(x => x.Resource == "encryptionKeys");
            if (ek != null)
            {
                ek.RetrievePassword();
                var SerializedKeys = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ek.Password);
                foreach (var key in SerializedKeys)
                {
                    EncryptionKeys.Add(key.Key, Convert.FromBase64String(key.Value));
                }
            }
        }
        public static void ClearEncryptionKeys()
        {
            var ek = Storage.PasswordVault.RetrieveAll().FirstOrDefault(x => x.Resource == "encryptionKeys");
                Storage.PasswordVault.Remove(ek);
            
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

        public static Settings Settings = new Settings(); //this just represents the storage
        public static ApplicationDataContainer SavedSettings = ApplicationData.Current.LocalSettings;
        public static LocalState State = new LocalState();
        public static PasswordVault PasswordVault = new PasswordVault();

    }

    public enum Theme { Dark, Light, Windows, Discord }
    public enum CollapseOverride { None, Mention, Unread }
    public class Settings
    {
        //Tuple<string, User> ActiveUser;
        public string lastVerison = "0";
        public bool LockChannels = false;
        public bool AutoHideChannels = true;
        public bool AutoHidePeople = false;
        public bool HighlightEveryone = true;
        public bool FriendsNotifyDMs = true;
        public bool FriendsNotifyFriendRequest = false;
        //public bool FriendsNotifyIncoming = true;
        //public bool FriendsNotifyOutgoing = false;
        public bool Toasts = false;
        public bool Badge = true;
        public bool LiveTile = true;
        public double RespUiM = 500;
        public double RespUiL = 1000;
        public double RespUiXl = 1500;
        public bool CustomBG = false;
        public string BGFilePath = "";
        public double MainOpacity = 0.9;
        public double SecondaryOpacity = 0.6;
        public double TertiaryOpacity = 0.4;
        public double CmdOpacity = 0.7;
        public bool ExpensiveRender = false;
        public bool ShowOfflineMembers = false;
        public bool DevMode = false;
        public bool DiscordLightTheme = false;
        public bool CompactMode = false;
        public Theme Theme = Theme.Dark;
        public bool AccentBrush = false; //If false use Blurple if true use System.AccentColor
        public bool Vibrate = false;
        public bool mutedChnEffectServer = false;
        public bool ShowWelcomeMessage = true;
        public bool EnableAcrylic = true;
        public bool VoiceChannels = false;
        public bool UseCompression = true;
        public bool VideoAd = false;
        public bool GifsOnHover = true;
        public string DateFormat = "M/d/yyyy";
        public string TimeFormat = "h:mm tt";
        public string DefaultAccount = "";
        public string OutputDevice = "Default";
        public CollapseOverride collapseOverride = CollapseOverride.Unread;
        //public Dictionary<string, Message> savedMessages = new Dictionary<string, Message>();
    }
}
