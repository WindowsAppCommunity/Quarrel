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
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Change change)
            {
                string format = ResourceLoader.GetForCurrentView("AuditLog").GetString(change.Key);
                
                if (string.IsNullOrEmpty(format))    
                    format = ResourceLoader.GetForCurrentView("AuditLog").GetString("UnknownChange");

                switch (change.Key)
                {
                    default:
                        return format.Replace("<change>", string.Format("**{0}**", change.Key))
                            .Replace("<value>", string.Format("**{0}**", change.NewValue)); ;
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
