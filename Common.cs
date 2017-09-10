using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Discord_UWP.SharedModels;
using Windows.ApplicationModel.Resources;

namespace Discord_UWP
{
    public static class Common
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static bool IsYesterday(DateTime dt)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            if (dt >= yesterday && dt < DateTime.Today)
                return true;
            return false;
        }

        // I added this function to make the date nicer to read and to make sure the formatting depends on the local one (not always the US one)
        public static string HumanizeDate(DateTime dt, DateTime? dtPrevious)
        {
            string result = "";
            if (dt.DayOfYear == DateTime.Now.DayOfYear && dt.Year == DateTime.Now.Year)
            {
                if (dtPrevious != null && dtPrevious.Value.DayOfYear == dt.DayOfYear && dtPrevious.Value.Year == dt.Year)
                { result = ""; }
                else
                { result = App.GetString("/Main/Today") + " "; }
            }
            else if (IsYesterday(dt))
            { result = App.GetString("/Main/Yesterday") + " "; }
            else
            {
                var localCulture = new CultureInfo(GlobalizationPreferences.Languages.First());
                result = dt.Date.ToString("d", localCulture) + " ";
            }

            result += " " + dt.ToString("HH:mm");

            return result;
        }
        public static SolidColorBrush GetSolidColorBrush(int alpha, int red, int green, int blue)
        {
            byte a = (byte)alpha;
            byte r = (byte)red;
            byte g = (byte)green;
            byte b = (byte)blue;
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static SolidColorBrush GetSolidColorBrush(Color color)
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
        public static SolidColorBrush IntToColor(int color)
        {
            byte a = (byte)(255);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static string HumanizeFileSize(ulong l)
        {
            long i = Convert.ToInt64(l);
            long absolute_i = (i < 0 ? -i : i);
            string suffix;
            double readable;
            if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            readable = (readable / 1024);
            return readable.ToString("0.### ") + suffix;
        }


        public static Uri AvatarUri(string s, string userid="", string suffix="")
        {
            if(String.IsNullOrEmpty(s))
                return new Uri("ms-appx:///Assets/DiscordAssets/default_avatar.png");
            else
                return new Uri("https://cdn.discordapp.com/avatars/"+userid+"/"+s+".jpg"+suffix);
        }

        public class Permissions
        {
            public Permissions()
            {

            }

            public Permissions GetPermissions(string guildId, IEnumerable<SharedModels.Role> roles)
            {
                foreach (SharedModels.Role role in roles)
                {
                    Perms.Permissions = Perms.Permissions | Convert.ToInt32(role.Permissions);
                }
                return this;
            }

            public Permissions GetPermissions(long input)
            {
                Perms = new PermissionsSave(Convert.ToInt32(input));
                return this;
            }

            public void AddOverwrites(IEnumerable<SharedModels.Overwrite> overwrites, string guild)
            {
                foreach (SharedModels.Overwrite overwrite in overwrites.TakeWhile(x => x.Type == "role" && Storage.Cache.Guilds[guild].Roles[x.Id].Name == "@everyone"))
                {
                    Perms.Permissions = Perms.Permissions &~ overwrite.Deny;
                    Perms.Permissions = Perms.Permissions | overwrite.Allow;
                }

                foreach (SharedModels.Overwrite overwrite in overwrites.TakeWhile(x => x.Type == "role"))
                {
                    if (Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(overwrite.Id))
                    {
                        Perms.Permissions = Perms.Permissions &~ overwrite.Deny;
                    }
                }

                foreach (SharedModels.Overwrite overwrite in overwrites.TakeWhile(x => x.Type == "role"))
                {
                    if (Storage.Cache.Guilds[guild].Members[Storage.Cache.CurrentUser.Raw.Id].Raw.Roles.Contains(overwrite.Id))
                    {
                        Perms.Permissions = Perms.Permissions | overwrite.Allow;
                    }
                }

                foreach (SharedModels.Overwrite overwrite in overwrites.TakeWhile(x => x.Type == "member"))
                {
                    if (Storage.Cache.CurrentUser.Raw.Id == overwrite.Id)
                    {
                        Perms.Permissions = Perms.Permissions &~ overwrite.Deny;
                        Perms.Permissions = Perms.Permissions | overwrite.Allow;
                    }
                }
            }

            public PermissionsSave Perms;
        }

        public struct PermissionsSave
        {
            public PermissionsSave(int perms)
            {
                Perms = perms;
            }

            public int Permissions
            {
                get { return Perms; }
                set { Perms = value; }
            }

            public bool CreateInstantInvite
            {
                get { return Convert.ToBoolean(Perms & 0x1); }
                set { Perms = value ? Perms | 0x1 : Perms & ~0x1; }
            }
            public bool KickMembers
            {
                get { return Convert.ToBoolean(Perms & 0x2); }
                set { Perms = value ? Perms | 0x2 : Perms & ~0x2; }
            }
            public bool BanMembers
            {
                get { return Convert.ToBoolean(Perms & 0x4); }
                set { Perms = value ? Perms | 0x4 : Perms & ~0x4; }
            }
            public bool Administrator
            {
                get { return Convert.ToBoolean(Perms & 0x8); }
                set { Perms = value ? Perms | 0x8 : Perms & ~0x8; }
            }
            public bool ManageChannels
            {
                get { return Convert.ToBoolean(Perms & 0x10); }
                set { Perms = value ? Perms | 0x10 : Perms & ~0x10; }
            }
            public bool ManangeGuild
            {
                get { return Convert.ToBoolean(Perms & 0x20); }
                set { Perms = value ? Perms | 0x20 : Perms & ~0x20; }
            }
            public bool AddReactions
            {
                get { return Convert.ToBoolean(Perms & 0x40); }
                set { Perms = value ? Perms | 0x40 : Perms & ~0x40; }
            }
            public bool ViewAuditLog
            {
                get { return Convert.ToBoolean(Perms & 0x80); }
                set { Perms = value ? Perms | 0x80 : Perms & ~0x80; }
            }
            public bool ReadMessages
            {
                get { return Convert.ToBoolean(Perms & 0x400); }
                set { Perms = value ? Perms | 0x400 : Perms & ~0x400; }
            }
            public bool SendMessages
            {
                get { return Convert.ToBoolean(Perms & 0x800); }
                set { Perms = value ? Perms | 0x800 : Perms & ~0x800; }
            }
            public bool SendTtsMessages
            {
                get { return Convert.ToBoolean(Perms & 0x1000); }
                set { Perms = value ? Perms | 0x1000 : Perms & ~0x1000; }
            }
            public bool ManageMessages
            {
                get { return Convert.ToBoolean(Perms & 0x2000); }
                set { Perms = value ? Perms | 0x2000 : Perms & ~0x2000; }
            }
            public bool EmbedLinks
            {
                get { return Convert.ToBoolean(Perms & 0x4000); }
                set { Perms = value ? Perms | 0x4000 : Perms & ~0x4000; }
            }
            public bool AttachFiles
            {
                get { return Convert.ToBoolean(Perms & 0x8000); }
                set { Perms = value ? Perms | 0x8000 : Perms & ~0x8000; }
            }
            public bool ReadMessageHistory
            {
                get { return Convert.ToBoolean(Perms & 0x10000); }
                set { Perms = value ? Perms | 0x10000 : Perms & ~0x10000; }
            }
            public bool MentionEveryone
            {
                get { return Convert.ToBoolean(Perms & 0x20000); }
                set { Perms = value ? Perms | 0x20000 : Perms & ~0x20000; }
            }
            public bool UseExternalEmojis
            {
                get { return Convert.ToBoolean(Perms & 0x40000); }
                set { Perms = value ? Perms | 0x40000 : Perms & ~0x40000; }
            }
            public bool Connect
            {
                get { return Convert.ToBoolean(Perms & 0x100000); }
                set { Perms = value ? Perms | 0x100000 : Perms & ~0x100000; }
            }
            public bool Speak
            {
                get { return Convert.ToBoolean(Perms & 0x200000); }
                set { Perms = value ? Perms | 0x200000 : Perms & ~0x200000; }
            }
            public bool MuteMembers
            {
                get { return Convert.ToBoolean(Perms & 0x400000); }
                set { Perms = value ? Perms | 0x400000 : Perms & ~0x400000; }
            }
            public bool DeafenMembers
            {
                get { return Convert.ToBoolean(Perms & 0x800000); }
                set { Perms = value ? Perms | 0x800000 : Perms & ~0x800000; }
            }
            public bool MoveMembers
            {
                get { return Convert.ToBoolean(Perms & 0x1000000); }
                set { Perms = value ? Perms | 0x1000000 : Perms & ~0x1000000; }
            }
            public bool UseVad
            {
                get { return Convert.ToBoolean(Perms & 0x2000000); }
                set { Perms = value ? Perms | 0x2000000 : Perms & ~0x2000000; }
            }
            public bool ChangeNickname
            {
                get { return Convert.ToBoolean(Perms & 0x4000000); }
                set { Perms = value ? Perms | 0x4000000 : Perms & ~0x4000000; }
            }
            public bool ManageNicknames
            {
                get { return Convert.ToBoolean(Perms & 0x8000000); }
                set { Perms = value ? Perms | 0x8000000 : Perms & ~0x8000000; }
            }
            public bool ManageRoles
            {
                get { return Convert.ToBoolean(Perms & 0x10000000); }
                set { Perms = value ? Perms | 0x10000000 : Perms & ~0x10000000; }
            }
            public bool ManageWebhooks
            {
                get { return Convert.ToBoolean(Perms & 0x20000000); }
                set { Perms = value ? Perms | 0x20000000 : Perms & ~0x20000000; }
            }
            public bool ManageEmojis
            {
                get { return Convert.ToBoolean(Perms & 0x40000000); }
                set { Perms = value ? Perms | 0x40000000 : Perms & ~0x40000000; }
            }

            int Perms;
        }
    }

    public class AsyncEvent<T>
        where T : class
    {
        private readonly object _subLock = new object();
        internal ImmutableArray<T> _subscriptions;

        public bool HasSubscribers => _subscriptions.Length != 0;
        public IReadOnlyList<T> Subscriptions => _subscriptions;

        public AsyncEvent()
        {
            _subscriptions = ImmutableArray.Create<T>();
        }

        public void Add(T subscriber)
        {
            Preconditions.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Add(subscriber);
        }
        public void Remove(T subscriber)
        {
            Preconditions.NotNull(subscriber, nameof(subscriber));
            lock (_subLock)
                _subscriptions = _subscriptions.Remove(subscriber);
        }
    }

    public static class EventExtensions
    {
        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> eventHandler)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke().ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> eventHandler, T arg)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> eventHandler, T1 arg1, T2 arg2)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3, T4>(this AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }
        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var subscribers = eventHandler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }
    }

    internal static class Preconditions
    {
        //Objects
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }
        public static void NotNull<T>(Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(name);
            else return new ArgumentNullException(name, msg);
        }

        //Strings
        public static void NotEmpty(string obj, string name, string msg = null) { if (obj.Length == 0) throw CreateNotEmptyException(name, msg); }
        public static void NotEmpty(Optional<string> obj, string name, string msg = null) { if (obj.IsSpecified && obj.Value.Length == 0) throw CreateNotEmptyException(name, msg); }
        public static void NotNullOrEmpty(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Length == 0) throw CreateNotEmptyException(name, msg);
        }
        public static void NotNullOrEmpty(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }
        public static void NotNullOrWhitespace(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
        }
        public static void NotNullOrWhitespace(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }

        private static ArgumentException CreateNotEmptyException(string name, string msg)
        {
            if (msg == null) return new ArgumentException("Argument cannot be blank", name);
            else return new ArgumentException(name, msg);
        }

        //Numerics
        public static void NotEqual(sbyte obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(byte obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(short obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ushort obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(int obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(uint obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(long obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ulong obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(sbyte? obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(byte? obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(short? obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ushort? obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(int? obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(uint? obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(long? obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ulong? obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<sbyte?> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<byte?> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<short?> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ushort?> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<int?> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<uint?> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<long?> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ulong?> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }

        private static ArgumentException CreateNotEqualException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value may not be equal to {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void AtLeast(sbyte obj, sbyte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(byte obj, byte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(short obj, short value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(ushort obj, ushort value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(int obj, int value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(uint obj, uint value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(long obj, long value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(ulong obj, ulong value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }

        private static ArgumentException CreateAtLeastException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be at least {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void GreaterThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(byte obj, byte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(short obj, short value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(ushort obj, ushort value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(int obj, int value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(uint obj, uint value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(long obj, long value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(ulong obj, ulong value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }

        private static ArgumentException CreateGreaterThanException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be greater than {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void AtMost(sbyte obj, sbyte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(byte obj, byte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(short obj, short value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(ushort obj, ushort value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(int obj, int value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(uint obj, uint value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(long obj, long value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(ulong obj, ulong value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }

        private static ArgumentException CreateAtMostException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be at most {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void LessThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(byte obj, byte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(short obj, short value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(ushort obj, ushort value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(int obj, int value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(uint obj, uint value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(long obj, long value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(ulong obj, ulong value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }

        private static ArgumentException CreateLessThanException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be less than {value}", name);
            else return new ArgumentException(msg, name);
        }
    }

    internal static class DateTimeUtils
    {
#if !UNIXTIME
        private const long UnixEpochTicks = 621_355_968_000_000_000;
        private const long UnixEpochSeconds = 62_135_596_800;
        private const long UnixEpochMilliseconds = 62_135_596_800_000;
#endif

        public static DateTimeOffset FromTicks(long ticks)
            => new DateTimeOffset(ticks, TimeSpan.Zero);
        public static DateTimeOffset? FromTicks(long? ticks)
            => ticks != null ? new DateTimeOffset(ticks.Value, TimeSpan.Zero) : (DateTimeOffset?)null;

        public static DateTimeOffset FromUnixSeconds(long seconds)
        {
#if UNIXTIME
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
#else
            long ticks = seconds * TimeSpan.TicksPerSecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }
        public static DateTimeOffset FromUnixMilliseconds(long milliseconds)
        {
#if UNIXTIME
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
#else
            long ticks = milliseconds * TimeSpan.TicksPerMillisecond + UnixEpochTicks;
            return new DateTimeOffset(ticks, TimeSpan.Zero);
#endif
        }

        public static long ToUnixSeconds(DateTimeOffset dto)
        {
#if UNIXTIME
            return dto.ToUnixTimeSeconds();
#else
            long seconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            return seconds - UnixEpochSeconds;
#endif
        }
        public static long ToUnixMilliseconds(DateTimeOffset dto)
        {
#if UNIXTIME
            return dto.ToUnixTimeMilliseconds();
#else
            long milliseconds = dto.UtcDateTime.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds - UnixEpochMilliseconds;
#endif
        }
    }

    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct Optional<T>
    {
        public static Optional<T> Unspecified => default(Optional<T>);
        private readonly T _value;

        /// <summary> Gets the value for this paramter. </summary>
        public T Value
        {
            get
            {
                if (!IsSpecified)
                    throw new InvalidOperationException("This property has no value set.");
                return _value;
            }
        }
        /// <summary> Returns true if this value has been specified. </summary>
        public bool IsSpecified { get; }

        /// <summary> Creates a new Parameter with the provided value. </summary>
        public Optional(T value)
        {
            _value = value;
            IsSpecified = true;
        }

        public T GetValueOrDefault() => _value;
        public T GetValueOrDefault(T defaultValue) => IsSpecified ? _value : defaultValue;

        public override bool Equals(object other)
        {
            if (!IsSpecified) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }
        public override int GetHashCode() => IsSpecified ? _value.GetHashCode() : 0;

        public override string ToString() => IsSpecified ? _value?.ToString() : null;
        private string DebuggerDisplay => IsSpecified ? (_value?.ToString() ?? "<null>") : "<unspecified>";

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);
        public static explicit operator T(Optional<T> value) => value.Value;
    }
    public static class Optional
    {
        public static Optional<T> Create<T>() => Optional<T>.Unspecified;
        public static Optional<T> Create<T>(T value) => new Optional<T>(value);

        public static T? ToNullable<T>(this Optional<T> val)
            where T : struct
            => val.IsSpecified ? val.Value : (T?)null;
    }
}
