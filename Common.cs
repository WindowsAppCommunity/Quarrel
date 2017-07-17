using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Discord_UWP.SharedModels;

namespace Discord_UWP
{
    public class Common
    {

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
                { result = "Today "; }
            }
            else if (IsYesterday(dt))
            { result = "Yesterday "; }
            else
            {
                var localCulture = new CultureInfo(GlobalizationPreferences.Languages.First());
                result = dt.Date.ToString("d", localCulture) + " ";
            }

            result += "at " + dt.ToString("HH:mm");

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
        public static string HumanizeFileSize(int i)
        {
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
}
