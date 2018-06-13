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

using Discord_UWP.LocalModels;
using System.Collections;
using Windows.UI.Xaml.Data;
using Windows.Storage;
using Newtonsoft.Json;

namespace Discord_UWP
{
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            int color = (int)value;
            if (color == -1)
                return App.Current.Resources["Foreground"];
            else
                return  Common.IntToColor((int)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value; //ignore conversion, there's no need to convert from solidcolorbrush to int
        }
    }
    class Common
    {
        public static SolidColorBrush IntToColor(int color)
        {
            if(color != 0)
            {
                byte a = (byte)(255);
                byte r = (byte)(color >> 16);
                byte g = (byte)(color >> 8);
                byte b = (byte)(color >> 0);
                return new SolidColorBrush(Color.FromArgb(a, r, g, b));
            }
            else
            {
                return (SolidColorBrush)App.Current.Resources["Foreground"];
            }
        }

        public static double SnowflakeToTime(string id)
        {
            //returns unix time in ms
            if (String.IsNullOrEmpty(id)) return 0;
            return (double)((Convert.ToInt64(id) / (4194304)) + 1420070400000)/10;
        }

    public static SolidColorBrush DiscriminatorColor(string desc)
        {
            switch (Convert.ToInt32(desc) % 5)
            {
                case 0: //Blurple
                    return new SolidColorBrush(Color.FromArgb(255, 114, 137, 218));
                case 1: //Grey
                    return new SolidColorBrush(Color.FromArgb(255, 116, 127, 141));
                case 2: //Green
                    return new SolidColorBrush(Color.FromArgb(255, 67, 181, 129));
                case 3: //Yellow
                    return new SolidColorBrush(Color.FromArgb(255, 250, 166, 26));
                case 4: //Red
                    return new SolidColorBrush(Color.FromArgb(255, 250, 71, 71));
            }
            return new SolidColorBrush(Color.FromArgb(255, 114, 137, 218));
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

        public static string Capitalize(string entry)
        {
            return entry.Substring(0, 1).ToUpper() + entry.Substring(1);
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
                { result = App.GetString("/Main/Today") + " " + App.GetString("/Main/at"); }
            }
            else if (IsYesterday(dt))
            { result = App.GetString("/Main/Yesterday") + " " + App.GetString("/Main/at"); }
            else
            {
                result = dt.ToString(Storage.Settings.DateFormat);
            }

            result += " " + dt.ToString(Storage.Settings.TimeFormat);

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
                return new Uri("ms-appx:///Assets/DiscordIcon.png");
            else if (s.StartsWith("a_"))
                return new Uri("https://cdn.discordapp.com/avatars/" + userid + "/" + s + ".gif" + suffix);
            else 
                return new Uri("https://cdn.discordapp.com/avatars/" + userid + "/" + s + ".png" + suffix);
        }

        public static void RemoveScrollviewerClipping(DependencyObject o)
        {
            var sc = GetScrollContentPresenter(o);
            if (sc != null)
                sc.Clip = null;
        }

        private static ScrollContentPresenter GetScrollContentPresenter(DependencyObject o)
        {
            if (o is ScrollContentPresenter)
                return o as ScrollContentPresenter;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollContentPresenter(child);
                if (result == null)
                    continue;
                else
                    return result;
            }
            return null;
        }
        public static ScrollViewer GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer)
                return o as ScrollViewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result == null)
                    continue;
                else
                    return result;
            }
            return null;
        }
        public static async void LoadEmojiDawg()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/emojis.json"));
            string json = await FileIO.ReadTextAsync(file);
            Controls.EmojiControl.RootObject root = JsonConvert.DeserializeObject<Controls.EmojiControl.RootObject>(json);
            DawgSharp.DawgBuilder<string> emojis = new DawgSharp.DawgBuilder<string>();
            //Lord, forgive me for my sins:
            foreach(var emoji in root.activity)
                foreach(var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.flags)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.food)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.nature)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.objects)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.people)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.symbols)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            foreach (var emoji in root.travel)
                foreach (var name in emoji.names)
                    emojis.Insert(name, emoji.surrogates);
            App.EmojiDawg = emojis.BuildDawg();
            sw.Stop();
            Debug.WriteLine("Emoji Dawg took " + sw.ElapsedMilliseconds + "ms to build");
        }
        public static List<string> FindMentions(string message)
        {
            List<string> mentions = new List<string>();
            bool inMention = false;
            bool inDesc = false;
            bool inChannel = false;
            string cache = "";
            string descCache = "";
            string chnCache = "";
            foreach (char c in message)
            {
                if (inMention)
                {
                    if (c == '#' && !inDesc)
                    {
                        inDesc = true;
                    }
                    else if (c == '@')
                    {
                        inDesc = false;
                        cache = "";
                        descCache = "";
                    }
                    else if (inDesc)
                    {
                        if (Char.IsDigit(c))
                        {
                            descCache += c;
                        }
                        else
                        {
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                        if (descCache.Length == 4)
                        {
                            User mention = null;
                            if (App.CurrentGuildIsDM)
                            {
                                mention = LocalState.DMs[App.CurrentChannelId].Users
                               .Where(x => x.Username == cache && x.Discriminator == descCache).FirstOrDefault();
                            } else
                            {
                                GuildMember member = LocalState.Guilds[App.CurrentGuildId].members
                               .Where(x => x.Value.User.Username == cache && x.Value.User.Discriminator == descCache).FirstOrDefault().Value;
                                if (member != null)
                                {
                                    mention = member.User;
                                }
                            }
                            if (mention != null)
                            {
                                mentions.Add("@" + cache + "#" + descCache);
                            }
                            inMention = false;
                            inDesc = false;
                            cache = "";
                            descCache = "";
                        }
                    }
                    else
                    {
                        cache += c;
                    }
                }
                else if (inChannel)
                {
                    if (c == ' ')
                    {
                        inChannel = false;
                        chnCache = "";
                    } else
                    {
                        chnCache += c;
                        if (!App.CurrentGuildIsDM)
                        {
                            if (LocalState.Guilds[App.CurrentGuildId].channels.Values.FirstOrDefault(x => x.raw.Type != 4 && x.raw.Name == chnCache) != null)
                            {
                                mentions.Add("#" + chnCache);
                            }
                        }
                    }
                }
                else if (c == '@')
                {
                    inMention = true;
                } else if (c == '#')
                {
                    inChannel = true;
                }
            }
            return mentions;
        }
    }
}
