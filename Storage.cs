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
using Discord_UWP.CacheModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP
{
    class Storage
    {
        public static event EventHandler SettingsChangedHandler;

        public static void SettingsChanged()
        {
            SettingsChangedHandler?.Invoke(typeof(Storage), EventArgs.Empty);
        }

        public static async void Clear()
        {
            await ApplicationData.Current.ClearAsync();
        }

        public static void SaveUser()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            if (!SavedSettings.Containers.ContainsKey("user"))
            {
                SavedSettings.CreateContainer("user", ApplicationDataCreateDisposition.Always);
            }
            StringWriter settingsWriter = new StringWriter();
            serializer.Serialize(settingsWriter, Token);
            SavedSettings.Values["user"] = settingsWriter.ToString();
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
        public static async void SaveCache()
        {
            await Task.Run(async () =>
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TempCache));
                    StorageFile file =
                        await SavedData.CreateFileAsync("cache", CreationCollisionOption.ReplaceExisting);

                    StringWriter settingsWriter = new StringWriter();
                    try
                    {
                        serializer.Serialize(settingsWriter, new TempCache(Cache));
                        await FileIO.WriteTextAsync(file, settingsWriter.ToString());
                    }
                    catch
                    {

                    }
                }
                catch
                {

                }
            });

        }

        public static async void CacheImage(string id, string avatar, bool guild)
        {
            StorageFolder imageFolder;
            try
            {
                imageFolder = await SavedData.GetFolderAsync("images");
            }
            catch
            {
                imageFolder = await SavedData.CreateFolderAsync("images");
            }

            StorageFolder avatarFolder;

            if (guild)
            {
                try
                {
                    avatarFolder = await imageFolder.GetFolderAsync("users");
                }
                catch
                {
                    avatarFolder = await imageFolder.CreateFolderAsync("users");
                }

                StorageFile icon = await avatarFolder.CreateFileAsync(id + "-" + avatar + ".png", CreationCollisionOption.ReplaceExisting);
                //Set image
            }
            else
            {
                try
                {
                    avatarFolder = await imageFolder.GetFolderAsync("guilds");
                }
                catch
                {
                    avatarFolder = await imageFolder.CreateFolderAsync("guilds");
                }


                StorageFile icon = await avatarFolder.CreateFileAsync(id + "-" + avatar + ".png", CreationCollisionOption.ReplaceExisting);
                //Set image
            }
        }

        public static async void SaveMessages()
        {
            await Task.Run(async () =>
            {
                List<ChannelTimeSave> temp = new List<ChannelTimeSave>();
                foreach (KeyValuePair<string, string> keyvalue in Storage.RecentMessages)
                {
                    temp.Add(new ChannelTimeSave(keyvalue.Key, keyvalue.Value));
                }

                XmlSerializer serializer = new XmlSerializer(typeof(List<ChannelTimeSave>));
                StorageFile file = await SavedData.CreateFileAsync("messages", CreationCollisionOption.ReplaceExisting);

                StringWriter settingsWriter = new StringWriter();
                serializer.Serialize(settingsWriter, temp);
                try
                {
                    await FileIO.WriteTextAsync(file, settingsWriter.ToString());
                }
                catch
                {

                }
            });

        }
        public static async void SaveMutedChannels()
        {
            await Task.Run(async () =>
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                StorageFile file =
                    await SavedData.CreateFileAsync("mutedchannels", CreationCollisionOption.ReplaceExisting);

                StringWriter settingsWriter = new StringWriter();
                serializer.Serialize(settingsWriter, MutedChannels);
                try
                {
                    await FileIO.WriteTextAsync(file, settingsWriter.ToString());
                }
                catch
                {

                }
            });

            await Task.Run(async () =>
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                StorageFile file =
                    await SavedData.CreateFileAsync("mutedservers", CreationCollisionOption.ReplaceExisting);

                StringWriter settingsWriter = new StringWriter();
                serializer.Serialize(settingsWriter, MutedServers);
                try
                {
                    await FileIO.WriteTextAsync(file, settingsWriter.ToString());
                }
                catch
                {

                }
            });
        }


        public static Settings Settings = new Settings(); //this just represents the storage
        public static Dictionary<string, string> RecentMessages = new Dictionary<string, string>();
        public static List<string> MutedChannels = new List<string>();
        public static List<string> MutedServers = new List<string>();
        public static string Token; //this just reperesents the storage
        public static ApplicationDataContainer SavedSettings = ApplicationData.Current.LocalSettings;
        public static StorageFolder SavedData = ApplicationData.Current.LocalFolder;
        public static int Version = 0;
        public static int TillAd = 10;
        public static int TillAdConv = 50;
        public static Cache Cache = new Cache();
    }

    public class ChannelTimeSave
    {
        public ChannelTimeSave() { }
        public ChannelTimeSave(string ChannelId, string Msgid)
        {
            this.ChannelId = ChannelId;
            this.Msgid = Msgid;
        }

        public string ChannelId;
        public string Msgid;
    }

    public enum Theme { Dark, Light, Windows, Discord }
    public class Settings
    {
        public bool LockChannels = false;
        public bool AutoHideChannels = true;
        public bool AutoHidePeople = false;
        public bool HighlightEveryone = true;
        public bool FriendsNotifyDMs = true;
        public bool FriendsNotifyFriendRequest = false;
        //public bool FriendsNotifyIncoming = true;
        //public bool FriendsNotifyOutgoing = false;
        public bool Toasts = false;
        public double RespUiM = 569;
        public double RespUiL = 768;
        public double RespUiXl = 1024;
        public bool AppBarAtBottom = false;
        public bool ExpensiveRender = false;
        public bool ShowOfflineMembers = false;
        public bool DevMode = false;
        public bool DiscordLightTheme = false;
        public bool CompactMode = false;
        public Theme Theme = Theme.Dark;
        public string AccentBrush = Color.FromArgb(255, 114,137,218).ToHex();
        public string OnlineBursh = Color.FromArgb(255, 67, 181, 129).ToHex();
        public string IdleBrush = Color.FromArgb(255, 250, 166, 26).ToHex();
        public string DndBrush = Color.FromArgb(255, 240, 71, 71).ToHex();
        public string OfflineBrush = Color.FromArgb(255, 170, 170, 170).ToHex();
        public bool Vibrate = false;
    }
}
