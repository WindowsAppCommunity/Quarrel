using DiscordAPI.API.Guild.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Quarrel.Converters.AuditLog
{
    public class AuditLogEntryToMarkdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AuditLogEntry entry)
            {
                return ((AuditLogActionType)entry.ActionType).ToString();
            }
            return "Unknown action";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
