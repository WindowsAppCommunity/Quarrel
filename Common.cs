using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

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
                var localCulture = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages.First());
                result = dt.Date.ToString("d", localCulture) + " ";
            }

            result += "at " + dt.ToString("HH:mm");

            return result;
        }
        public static SolidColorBrush IntToColor(int color)
        {
            byte a = (byte)(255);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
        }
    }
}
