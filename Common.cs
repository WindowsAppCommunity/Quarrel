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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Xml.Linq;
using System.Xml;

namespace Discord_UWP
{
    class Common
    {
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

        public static SolidColorBrush GetSolidColorBrush(Color color)
        {
            byte a = color.A;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static bool IsYesterday(DateTime dt)
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            if (dt >= yesterday && dt < DateTime.Today)
                return true;
            return false;
        }
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

        public static Uri AvatarUri(string s, string userid = "", string suffix = "")
        {
            if (String.IsNullOrEmpty(s))
                return new Uri("ms-appx:///Assets/DiscordAssets/default_avatar.png");
            else
                return new Uri("https://cdn.discordapp.com/avatars/" + userid + "/" + s + ".jpg" + suffix);
        }

        public static ScrollViewer GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            {
                return o as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }
            return null;
        }
    }
}
