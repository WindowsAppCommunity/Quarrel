using DiscordAPI.API.Guild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class ChangeToMarkdownConverter : IValueConverter
    {
        #region Method

        public string GetFormat(Change change)
        {
            string append = (change.OldValue != null ? "Change" : "Set");
            string format = ResourceLoader.GetForCurrentView("AuditLog").GetString(change.Key + append);

            if (string.IsNullOrEmpty(format))
                format = ResourceLoader.GetForCurrentView("AuditLog").GetString("Generic" + append);

            return format;
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Change change)
            {
                string format = GetFormat(change);

                switch (change.Key)
                {
                    case "name":
                        if (change.NewValue != null)
                            format = format.Replace("<new>", change.NewValue.ToString());
                        if (change.OldValue != null)
                            format = format.Replace("<old>", change.OldValue.ToString());
                        return format;
                    case "nsfw":
                        if (change.NewValue != null)
                            format = format.Replace("<new>", (bool)change.NewValue ? "**NSFW**" : "**SFW**");
                        return format;
                    default:
                        format = format.Replace("<property>", change.Key);
                        if (change.NewValue != null)
                            format = format.Replace("<new>", change.NewValue.ToString());
                        if (change.OldValue != null)
                            format = format.Replace("<old>", change.OldValue.ToString());
                        return format;
                }
            }
            return "Unknown change";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
