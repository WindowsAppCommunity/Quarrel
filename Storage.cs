using System;
using Windows.Storage;
using Windows.Security.Credentials;

using Discord_UWP.LocalModels;
using Discord_UWP.SharedModels;

namespace Discord_UWP
{
    public static class Storage
    {
        //public static Settings Settings = new Settings(); //this just represents the storage
        //public static ApplicationDataContainer SavedSettings = ApplicationData.Current.LocalSettings;
        public static LocalState State = new LocalState();
        public static PasswordVault PasswordVault = new PasswordVault();
    }

    public class Settings
    {
        Tuple<string, User> ActiveUser;
    }
}
